using DotnetIgnoreCliTool.Github.Services;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.Commands
{
    public class GitignoreListCommandHandler : ICommandHandler
    {
        [Option(CommandOptionType.NoValue, Description = "Prints files names without .gitignore")]
        public string Short { get; set; }

        private readonly IGitignoreGithubService _githubService;
        private readonly IConsole _console;

        public GitignoreListCommandHandler(IGitignoreGithubService githubService, IConsole console)
        {
            _githubService = githubService ?? throw new ArgumentNullException(nameof(githubService));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public bool CanHandle(string command) => string.Equals(CommandName, command, StringComparison.InvariantCultureIgnoreCase);

        public const string CommandName = "list";

        public async Task ExecuteAsync()
        {
            IReadOnlyList<string> gitignoreFilesNames = await _githubService.GetAllIgnoreFilesNames();

            if (!string.IsNullOrEmpty(Short))
            {
                gitignoreFilesNames = gitignoreFilesNames
                    .Select(fileName => fileName.Replace(".gitignore", ""))
                    .ToImmutableList();
            }

            foreach (var gitignoreFileName in gitignoreFilesNames)
            {
                _console.WriteLine($"- {gitignoreFileName}");
            }
        }
    }
}