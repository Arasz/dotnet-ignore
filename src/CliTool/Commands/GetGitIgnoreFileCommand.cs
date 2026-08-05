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

        if (string.IsNullOrWhiteSpace(destination))
        {
            destination = Environment.CurrentDirectory;
        }

        await gitignoreFileWriter.WriteToFileAsync(destination, gitIgnoreFile.Content);
    }

    private async Task<GitignoreFile> GetGitIgnoreFileFromGithub(string providedNames, bool minimizeFileSize)
    {
        var processedNames = NamesSplitStrategy.Split(providedNames);

        var gitIgnoreFiles = await Task.WhenAll(processedNames.Select(gitignoreService.GetIgnoreFile));

        var downloadedFiles = EnsureAllGitIgnoreFilesWereDownloaded(gitIgnoreFiles, processedNames);

        return mergeStrategy.Merge(downloadedFiles, minimizeFileSize);
    }

    private static GitignoreFile[] EnsureAllGitIgnoreFilesWereDownloaded(
        IReadOnlyCollection<GitignoreFile?> gitIgnoreFiles,
        IEnumerable<string> names)
    {
        var notFoundNames = gitIgnoreFiles
            .Zip(names, (file, name) => (file, name))
            .Where(tuple => tuple.file is null)
            .Select(tuple => tuple.name)
            .ToArray();

        if (notFoundNames.Length > 0)
        {
            var exceptionMessage = string.Concat(notFoundNames
                .Select(name => $"Name {name} is not a correct .gitignore file name{Environment.NewLine}"));
            ThrowHelper.ThrowArgumentException(exceptionMessage);
        }

        return gitIgnoreFiles.Select(file => file!).ToArray();
    }
}
