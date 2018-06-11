using McMaster.Extensions.CommandLineUtils;
using System;

namespace DotnetIgnoreCliTool.Cli.Commands.List
{
    public sealed class GitignoreListCommand : CommandLineApplication
    {
        private readonly IApplicationCommandHandler<GitignoreListCommand> _commandHandler;

        public CommandOption ShortOption { get; set; }

        private const string CommandName = "list";

        public GitignoreListCommand(IApplicationCommandHandler<GitignoreListCommand> commandHandler)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));

            ConfigureCommandLineApplication();
        }

        private void ConfigureCommandLineApplication()
        {
            Name = CommandName;
            ShortOption = Option("-s | --short", "Prints files names without .gitignore", CommandOptionType.NoValue);
            OnExecute(() => _commandHandler.HandleCommandAsync(this));

            ThrowOnUnexpectedArgument = false;
        }
    }
}