using CliTool.Arguments;
using CliTool.FIles;
using CliTool.Github.Models;
using CliTool.Github.Services;
using CliTool.Merge;
using CommunityToolkit.Diagnostics;

namespace CliTool.Commands;

public sealed class GetGitIgnoreFileCommand(
    IGitignoreService gitignoreService,
    IMergeStrategy mergeStrategy,
    IGitignoreFileWriter gitignoreFileWriter)
{
    public async Task GetGitIgnoreFile(string names, string? destination, bool minimizeFileSize)
    {
        var gitIgnoreFile = await GetGitIgnoreFileFromGithub(names, minimizeFileSize);

        try
        {
            if (string.IsNullOrWhiteSpace(destination))
            {
                destination = Environment.CurrentDirectory;
            }

            await gitignoreFileWriter.WriteToFileAsync(destination, gitIgnoreFile.Content);
        }
        catch (Exception e)
        {
            ThrowHelper.ThrowArgumentException($"Destination path {destination} is invalid or there were " +
                                               "problem with file access", e);
        }
    }

    private async Task<GitignoreFile> GetGitIgnoreFileFromGithub(string providedNames, bool minimizeFileSize)
    {
        var processedNames = NamesSplitStrategy.Split(providedNames);
        
        var gitIgnoreFiles =  await Task.WhenAll(processedNames.Select(gitignoreService.GetIgnoreFile));

        EnsureAllGitIgnoreFilesWhereDownloaded(gitIgnoreFiles, processedNames);

        return mergeStrategy.Merge(gitIgnoreFiles, minimizeFileSize);
    }

    private static void EnsureAllGitIgnoreFilesWhereDownloaded(IReadOnlyCollection<GitignoreFile> gitIgnoreFiles,
        IEnumerable<string> names)
    {
        var isAnyEmptyFile = gitIgnoreFiles
            .Any(file => file == GitignoreFile.Empty);

        if (!isAnyEmptyFile)
        {
            return;
        }

        var exceptionMessageForFailedFiles = gitIgnoreFiles
            .Zip(names, (file, name) => (file, name))
            .Where(tuple => tuple.file == GitignoreFile.Empty)
            .Select(tuple => $"Name {tuple.name} is not correct .gitignore file name {Environment.NewLine}")
            .Aggregate(string.Empty, (msg, aggregate) => aggregate + msg);

        ThrowHelper.ThrowArgumentException(exceptionMessageForFailedFiles);
    }
}