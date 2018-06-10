using DotnetIgnoreCliTool.Cli.FIles;
using DotnetIgnoreCliTool.Github.Models;
using DotnetIgnoreCliTool.Github.Services;
using McMaster.Extensions.CommandLineUtils;
using PowerArgs;
using System;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.Commands
{
    public class GitignoreGetCommandHandler : ICommandHandler
    {
        private const string CommandName = "get";

        [Argument(0, Description = ".gitignore file name case insensitive. Accepts short and full version of the name", ShowInHelpText = true)]
        public string Name { get; set; }

        [Argument(1, Description = "Destination directory where gitignore will be saved. If not provided execution directory will be used", ShowInHelpText = true)]
        public string Destination { get; set; }

        private readonly IGitignoreGithubService _githubService;
        private readonly IGitignoreFileWriter _gitignoreFileWriter;

        public GitignoreGetCommandHandler(IGitignoreGithubService githubService, IGitignoreFileWriter gitignoreFileWriter)
        {
            _githubService = githubService ?? throw new ArgumentNullException(nameof(githubService));
            _gitignoreFileWriter = gitignoreFileWriter ?? throw new ArgumentNullException(nameof(gitignoreFileWriter));
        }

        public bool CanHandle(string command) => string.Equals(CommandName, command, StringComparison.InvariantCultureIgnoreCase);

        public async Task ExecuteAsync()
        {
            GitignoreFile gitgnoreFile = await _githubService.GetIgnoreFile(Name);

            if (GitignoreFile.Empty == gitgnoreFile)
            {
                throw new ArgException($"Name {Name} is not correct .gitignore file name");
            }

            await _gitignoreFileWriter.WriteToFileAsync(Destination, gitgnoreFile.Content);
        }
    }
}