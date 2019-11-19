using McMaster.Extensions.CommandLineUtils;
using System;
using DotnetIgnoreCliTool.Cli.Commands.Get.Split;

namespace DotnetIgnoreCliTool.Cli.Commands.Get
{
    public sealed class GitignoreGetCommand : CommandLineApplication
    {
        private readonly IApplicationCommandHandler<GitignoreGetCommand> _commandHandler;
        private readonly IFileNameSpliter _fileNameSpliter;

        private const string CommandName = "get";

        public CommandOption NamesOption { get; private set; }

        public CommandOption DestinationOption { get; private set; }

        public GitignoreGetCommand(IApplicationCommandHandler<GitignoreGetCommand> commandHandler,
            IFileNameSpliter fileNameSpliter)
        {
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _fileNameSpliter = fileNameSpliter ?? throw new ArgumentNullException(nameof(fileNameSpliter));

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
            var description = $".gitignore file names case insensitive separated by \"{_fileNameSpliter.Separator}\"." +
                              " Accepts short and full version of the name. When multiple names are given merged " +
                              "result file is created";

            return Option("-n | --names",
                description,
                CommandOptionType.SingleValue,
                option => { option.IsRequired(); });
        }

        private CommandOption CreateDestinationOption()
        {
            const string description = "Destination directory where gitignore will be saved. If not provided " +
                                       "execution directory will be used";

            return Option("-d | --destination",
                description,
                CommandOptionType.SingleValue);
        }
    }
}