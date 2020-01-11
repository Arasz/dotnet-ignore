using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;

namespace DotnetIgnoreCliTool.Cli.Execution
{
    public sealed class ApplicationCommandsExecutor : CommandLineApplication, IApplicationCommandExecutor
    {
        private readonly IEnumerable<CommandLineApplication> _subcommands;

        public ApplicationCommandsExecutor(IEnumerable<CommandLineApplication> commands)
        {
            _subcommands = commands ?? throw new ArgumentNullException(nameof(commands));

            ConfigureCommandLineApplication();
        }

        private void ConfigureCommandLineApplication()
        {
            Name = "ignore";
            this.VersionOptionFromAssemblyAttributes(typeof(ApplicationCommandsExecutor).Assembly);

            Commands.AddRange(_subcommands);

            Conventions.UseDefaultConventions();

            ThrowOnUnexpectedArgument = false;
        }

        int IApplicationCommandExecutor.Execute(string[] args)
        {
            var selectedCommand = Parse(args).SelectedCommand;

            if (selectedCommand.IsShowingInformation)
            {
                return ReturnCodes.Success;
            }

            if (selectedCommand.Name == Name)
            {
                ShowHint();
                return ReturnCodes.Error;
            }

            return selectedCommand.Execute(args);
        }
    }
}