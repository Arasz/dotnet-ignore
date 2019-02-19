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
            GitignoreFile gitIgnoreFile = await _githubService.GetIgnoreFile(command.NameOption.Value());

            if (GitignoreFile.Empty == gitIgnoreFile)
            {
                throw new ArgException($"Name {command.NameOption.Value()} is not correct .gitignore file name");
            }

            await _gitignoreFileWriter.WriteToFileAsync(command.DestinationOption.Value(), gitIgnoreFile.Content);

            return ReturnCodes.Success;
        }
    }
}