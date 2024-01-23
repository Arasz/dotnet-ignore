using System.Collections.Immutable;
using CliTool.FIles;
using CliTool.Github.Models;
using CliTool.Github.Services;
using CliTool.Merge;
using Cocona;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

const string splitSeparator = ",";

var consoleAppBuilder = CoconaApp.CreateBuilder();

consoleAppBuilder.Services
    .AddSingleton<IGitignoreService, GithubGitignoreService>()
    .AddSingleton<IGitignoreFileWriter, GitignoreFileWriter>()
    .AddSingleton<IMergeStrategy, SimpleMergeStrategy>();

var consoleApp = consoleAppBuilder.Build();

consoleApp.AddCommand("get", async ([Argument(Description =
        $"""
         Git ignore files names, case insensitive, separated by "{splitSeparator}". Accepts file names with and without .gitignore part. When multiple names are given files are merged into one result file.
         """)]
    string names,
    [Option('d', Description = "Destination directory where a .gitignore file will be saved. If not provided execution directory will be used as a default value")]
    string? destination,
    IGitignoreService gitignoreService,
    IMergeStrategy mergeStrategy,
    IGitignoreFileWriter gitignoreFileWriter) =>
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

    return;

    async Task<GitignoreFile> GetGitIgnoreFileFromGithub(string providedNames)
    {
        var processedNames = providedNames.Split(splitSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var gitIgnoreFiles = await DownloadAllGitIgnoreFiles(processedNames);

        EnsureAllGitIgnoreFilesWhereDownloaded(gitIgnoreFiles, processedNames);

        return mergeStrategy.Merge(gitIgnoreFiles);
    }

    async Task<GitignoreFile[]> DownloadAllGitIgnoreFiles(IEnumerable<string> processedNames)
    {
        var gitIgnoreFilesTasks = processedNames
            .Select(gitignoreService.GetIgnoreFile)
            .ToArray();

        await Task.WhenAll(gitIgnoreFilesTasks);

        return gitIgnoreFilesTasks
            .Select(task => task.Result)
            .ToArray();
    }


    static void EnsureAllGitIgnoreFilesWhereDownloaded(IReadOnlyCollection<GitignoreFile> gitIgnoreFiles,
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
});

consoleApp.AddCommand("list", async ([Option('s', Description = "Prints files names without .gitignore part")] bool shorten, IGitignoreService gitignoreService) =>
{
    var gitignoreFilesNames = await gitignoreService.GetAllIgnoreFilesNames();

    if (shorten)
    {
        gitignoreFilesNames = gitignoreFilesNames
            .Select(fileName => fileName.Replace(".gitignore", ""))
            .ToImmutableList();
    }

    foreach (var gitignoreFileName in gitignoreFilesNames)
    {
        Console.WriteLine($"- {gitignoreFileName}");
    }

    return 0;
});

consoleApp.Run();