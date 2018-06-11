using DotnetIgnoreCliTool.Github.Services;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.Commands.List
{
    public class GitignoreListCommandHandler : IApplicationCommandHandler<GitignoreListCommand>
    {
        private readonly IGitignoreGithubService _githubService;
        private readonly IConsole _console;

        public GitignoreListCommandHandler(IGitignoreGithubService githubService, IConsole console)
        {
            _githubService = githubService ?? throw new ArgumentNullException(nameof(githubService));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public async Task<int> HandleCommandAsync(GitignoreListCommand command)
        {
            IReadOnlyList<string> gitignoreFilesNames = await _githubService.GetAllIgnoreFilesNames();

            if (command.ShortOption.HasValue())
            {
                gitignoreFilesNames = gitignoreFilesNames
                    .Select(fileName => fileName.Replace(".gitignore", ""))
                    .ToImmutableList();
            }

            foreach (var gitignoreFileName in gitignoreFilesNames)
            {
                _console.WriteLine($"- {gitignoreFileName}");
            }

            return ReturnCodes.Success;
        }
    }
}