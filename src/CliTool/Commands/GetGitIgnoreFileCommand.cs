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
    public async Task GetGitIgnoreFile(string names, string? destination)
    {
        var gitIgnoreFile = await GetGitIgnoreFileFromGithub(names);

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

    private async Task<GitignoreFile> GetGitIgnoreFileFromGithub(string providedNames)
    {
        var processedNames = NamesSplitStrategy.Split(providedNames);

        var gitIgnoreFiles = await DownloadAllGitIgnoreFiles(processedNames);

        EnsureAllGitIgnoreFilesWhereDownloaded(gitIgnoreFiles, processedNames);

        return mergeStrategy.Merge(gitIgnoreFiles);
    }

    private async Task<GitignoreFile[]> DownloadAllGitIgnoreFiles(IEnumerable<string> processedNames)
    {
        var gitIgnoreFilesTasks = processedNames
            .Select(gitignoreService.GetIgnoreFile)
            .ToArray();

        await Task.WhenAll(gitIgnoreFilesTasks);

        return gitIgnoreFilesTasks
            .Select(task => task.Result)
            .ToArray();
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