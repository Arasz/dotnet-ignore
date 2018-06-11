using DotnetIgnoreCliTool.Cli.FIles;
using DotnetIgnoreCliTool.Github.Models;
using DotnetIgnoreCliTool.Github.Services;
using McMaster.Extensions.CommandLineUtils;
using PowerArgs;
using System;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.Commands
{
    public sealed class GitignoreGetCommandHandler : CommandLineApplicationBase, ICommandHandler
    {
        private const string CommandName = "get";

        public CommandOption NameOption { get; set; }

        public CommandOption DestinationOption { get; set; }

        private readonly IGitignoreGithubService _githubService;
        private readonly IGitignoreFileWriter _gitignoreFileWriter;

        public GitignoreGetCommandHandler(IGitignoreGithubService githubService, IGitignoreFileWriter gitignoreFileWriter)
        {
            _githubService = githubService ?? throw new ArgumentNullException(nameof(githubService));
            _gitignoreFileWriter = gitignoreFileWriter ?? throw new ArgumentNullException(nameof(gitignoreFileWriter));

            ConfigureCommandLineApplication();
        }

        protected override void ConfigureCommandLineApplication()
        {
            Name = CommandName;
            NameOption = Option("-n | --name",
                ".gitignore file name case insensitive. Accepts short and full version of the name",
                CommandOptionType.SingleValue,
                option => { option.IsRequired(); });

            DestinationOption = Option("-d | --destination",
                "Destination directory where gitignore will be saved. If not provided execution directory will be used",
                CommandOptionType.SingleValue);

            OnExecute((Func<Task<int>>)HandleCommandAsync);
        }

        public async Task<int> HandleCommandAsync()
        {
            GitignoreFile gitgnoreFile = await _githubService.GetIgnoreFile(NameOption.Value());

            if (GitignoreFile.Empty == gitgnoreFile)
            {
                throw new ArgException($"Name {NameOption.Value()} is not correct .gitignore file name");
            }

            await _gitignoreFileWriter.WriteToFileAsync(DestinationOption.Value(), gitgnoreFile.Content);

            return 0;
        }
    }
}