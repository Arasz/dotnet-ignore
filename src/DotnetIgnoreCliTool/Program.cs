using DotnetIgnoreCliTool.Cli.Commands;
using DotnetIgnoreCliTool.Cli.Execution;
using DotnetIgnoreCliTool.Cli.FIles;
using DotnetIgnoreCliTool.Github.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotnetIgnoreCliTool
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var servicesProvider = new ServiceCollection()
                .AddSingleton<ICommandHandler, GitignoreGetCommandHandler>()
                .AddSingleton<ICommandHandler, GitignoreListCommandHandler>()
                .AddSingleton<ICommandHandlerExecutor, CommandHandlerExecutor>()
                .AddSingleton<IGitignoreGithubService, GitignoreGithubService>()
                .AddSingleton<IGitignoreFileWriter, GitignoreFileWritter>()
                .AddSingleton<IConsole, PhysicalConsole>()
                .BuildServiceProvider();

            return RunCommandsWithExecutor(args, servicesProvider.GetRequiredService<ICommandHandlerExecutor>());
        }

        private static int RunCommandsWithExecutor(string[] args, ICommandHandlerExecutor executor)
        {
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));

            return executor.Execute(args);
        }
    }
}