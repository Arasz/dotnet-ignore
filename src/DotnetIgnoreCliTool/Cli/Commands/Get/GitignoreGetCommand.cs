using McMaster.Extensions.CommandLineUtils;
using System;
using DotnetIgnoreCliTool.Cli.Commands.Get.Names;

namespace DotnetIgnoreCliTool.Cli.Commands.Get
{
    public sealed class GitignoreGetCommand : CommandLineApplication
    {
        private readonly IApplicationCommandHandler<GitignoreGetCommand> _commandHandler;
        private readonly IConcatedNamesProcessor _concatedNamesProcessor;

        private const string CommandName = "get";

        public CommandOption NamesOption { get; private set; }

        public CommandOption DestinationOption { get; private set; }

        public GitignoreGetCommand(IApplicationCommandHandler<GitignoreGetCommand> commandHandler,
            IConcatedNamesProcessor concatedNamesProcessor)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _concatedNamesProcessor = concatedNamesProcessor ?? throw new ArgumentNullException(nameof(concatedNamesProcessor));

            ConfigureCommandLineApplication();
        }

        private void ConfigureCommandLineApplication()
        {
            Name = CommandName;
            NamesOption = CreateNamesOption();
            DestinationOption = CreateDestinationOption();

            OnExecute(() => _commandHandler.HandleCommandAsync(this));

            ThrowOnUnexpectedArgument = false;
        }


        private CommandOption CreateNamesOption()
        {
            var description = $".gitignore file names case insensitive separated by \"{_concatedNamesProcessor.Separator}\"." +
                              " Accepts short and full version of the name. When multiple names are given merged " +
                              "result file is created";

            return Option("-n | --names",
                description,
                CommandOptionType.SingleValue,
                option => { option.IsRequired(); });
        }

        private CommandOption CreateDestinationOption()
        {
            const string description = "Destination directory where gitignore will be saved. If not provided execution directory will be used";

            return Option("-d | --destination",
                description,
                CommandOptionType.SingleValue);
        }
    }
}