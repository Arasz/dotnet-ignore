using DotnetIgnoreCliTool.Github.Services;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.Commands
{
    public sealed class GitignoreListCommandHandler : CommandLineApplicationBase, ICommandHandler
    {
        public CommandOption ShortOption { get; set; }

        private readonly IGitignoreGithubService _githubService;
        private readonly IConsole _console;

        public GitignoreListCommandHandler(IGitignoreGithubService githubService, IConsole console)
        {
            _githubService = githubService ?? throw new ArgumentNullException(nameof(githubService));
            _console = console ?? throw new ArgumentNullException(nameof(console));

            ConfigureCommandLineApplication();
        }

        protected override void ConfigureCommandLineApplication()
        {
            Name = CommandName;
            ShortOption = Option("-s | --short", "Prints files names without .gitignore", CommandOptionType.NoValue);
            OnExecute((Func<Task<int>>)HandleCommandAsync);
        }

        public async Task<int> HandleCommandAsync()
        {
            IReadOnlyList<string> gitignoreFilesNames = await _githubService.GetAllIgnoreFilesNames();

            if (ShortOption.HasValue())
            {
                gitignoreFilesNames = gitignoreFilesNames
                    .Select(fileName => fileName.Replace(".gitignore", ""))
                    .ToImmutableList();
            }

            foreach (var gitignoreFileName in gitignoreFilesNames)
            {
                _console.WriteLine($"- {gitignoreFileName}");
            }

            return 0;
        }

        public const string CommandName = "list";
    }
}