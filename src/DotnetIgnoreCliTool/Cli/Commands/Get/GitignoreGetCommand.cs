using McMaster.Extensions.CommandLineUtils;
using System;

namespace DotnetIgnoreCliTool.Cli.Commands.Get
{
    public sealed class GitignoreGetCommand : CommandLineApplication
    {
        private readonly IApplicationCommandHandler<GitignoreGetCommand> _commandHandler;

        private const string CommandName = "get";

        public CommandOption NameOption { get; set; }

        public CommandOption DestinationOption { get; set; }

        public GitignoreGetCommand(IApplicationCommandHandler<GitignoreGetCommand> commandHandler)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));

            ConfigureCommandLineApplication();
        }

        private void ConfigureCommandLineApplication()
        {
            Name = CommandName;
            NameOption = Option("-n | --name",
                ".gitignore file name case insensitive. Accepts short and full version of the name",
                CommandOptionType.SingleValue,
                option =>
                {
                    option.IsRequired();
                });

            DestinationOption = Option("-d | --destination",
                "Destination directory where gitignore will be saved. If not provided execution directory will be used",
                CommandOptionType.SingleValue);

            OnExecute(() => _commandHandler.HandleCommandAsync(this));

            ThrowOnUnexpectedArgument = false;
        }
    }
}