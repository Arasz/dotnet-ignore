using DotnetIgnoreCliTool.Cli.Files;
using DotnetIgnoreCliTool.Github.Models;
using DotnetIgnoreCliTool.Github.Services;
using PowerArgs;
using System;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.Commands.Get
{
    public class GitignoreGetCommandHandler : IApplicationCommandHandler<GitignoreGetCommand>
    {
        private readonly IGitignoreGithubService _githubService;
        private readonly IGitignoreFileWriter _gitignoreFileWriter;

        public GitignoreGetCommandHandler(IGitignoreGithubService githubService, IGitignoreFileWriter gitignoreFileWriter)
        {
            _githubService = githubService ?? throw new ArgumentNullException(nameof(githubService));
            _gitignoreFileWriter = gitignoreFileWriter ?? throw new ArgumentNullException(nameof(gitignoreFileWriter));
        }

        public async Task<int> HandleCommandAsync(GitignoreGetCommand command)
        {
            var names = command.NamesOption.Value();
            var gitIgnoreFile = await _githubService.GetIgnoreFile(names);

            if (GitignoreFile.Empty == gitIgnoreFile)
            {
                throw new ArgException($"Name {names} is not correct .gitignore file name");
            }

            var destination = command.DestinationOption.Value();
            await _gitignoreFileWriter.WriteToFileAsync(destination, gitIgnoreFile.Content);

            return ReturnCodes.Success;
        }
    }
}