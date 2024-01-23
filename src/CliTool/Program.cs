using System.Collections.Immutable;
using CliTool.Arguments;
using CliTool.Commands;
using CliTool.FIles;
using CliTool.Github.Services;
using CliTool.Merge;
using Cocona;
using Microsoft.Extensions.DependencyInjection;



var consoleAppBuilder = CoconaApp.CreateBuilder();

consoleAppBuilder.Services
    .AddTransient<IGitignoreService, GithubGitignoreService>()
    .AddTransient<IGitignoreFileWriter, GitignoreFileWriter>()
    .AddTransient<IMergeStrategy, SimpleMergeStrategy>()
    .AddTransient<GetGitIgnoreFileCommand>()
    .AddTransient<ListGitIgnoreFilesCommand>();

var consoleApp = consoleAppBuilder.Build();

consoleApp.AddCommand("get", ([Argument(Description =
            $"""
             Git ignore files names, case insensitive, separated by "{NamesSplitStrategy.SplitSeparator}". Accepts file names with and without .gitignore part. When multiple names are given files are merged into one result file.
             """)]
        string names,
        [Option('d', Description = "Destination directory where a .gitignore file will be saved. If not provided execution directory will be used as a default value")]
        string? destination,
        GetGitIgnoreFileCommand command) => Task.FromResult(command.GetGitIgnoreFile(names, destination)))
    .WithDescription("Build .gitignore file from files specified by the names parameter");

consoleApp.AddCommand("list",
        ([Option('s', Description = "Prints files names without .gitignore part")] bool shorten, ListGitIgnoreFilesCommand command) => command.ListGitIgnoreFiles(shorten))
    .WithDescription("Lists all available .gitignore files");

consoleApp.Run();