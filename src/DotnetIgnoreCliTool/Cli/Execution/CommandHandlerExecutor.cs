using DotnetIgnoreCliTool.Cli.Commands;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetIgnoreCliTool.Cli.Execution
{
    public sealed class CommandHandlerExecutor : CommandLineApplicationBase, ICommandHandlerExecutor
    {
        private readonly IEnumerable<ICommandHandler> _handlers;

        private const int ExecutionErrorCode = 1;

        public CommandHandlerExecutor(IEnumerable<ICommandHandler> commandHandlers)
        {
            _handlers = commandHandlers ?? throw new ArgumentNullException(nameof(commandHandlers));

            ConfigureCommandLineApplication();
        }

        protected override void ConfigureCommandLineApplication()
        {
            Commands.AddRange(_handlers.Cast<CommandLineApplication>());

            Conventions.UseDefaultConventions();
        }

        int ICommandHandlerExecutor.Execute(string[] args)
        {
            var selectedCommand = Parse(args).SelectedCommand;

            if (selectedCommand.Parent is null)
            {
                ShowHint();

                return ExecutionErrorCode;
            }

            var subcommandArgs = args
                .Skip(1)
                .ToArray();

            return selectedCommand.Execute(subcommandArgs);
        }
    }
}