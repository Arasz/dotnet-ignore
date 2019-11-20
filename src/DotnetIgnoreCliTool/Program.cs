using DotnetIgnoreCliTool.Cli;
using DotnetIgnoreCliTool.Cli.Commands;
using DotnetIgnoreCliTool.Cli.Commands.Get;
using DotnetIgnoreCliTool.Cli.Commands.List;
using DotnetIgnoreCliTool.Cli.Execution;
using DotnetIgnoreCliTool.Cli.Files;
using DotnetIgnoreCliTool.Github.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using DotnetIgnoreCliTool.Cli.Commands.Get.Merge;
using DotnetIgnoreCliTool.Cli.Commands.Get.Split;

namespace DotnetIgnoreCliTool
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var servicesProvider = new ServiceCollection()
               .AddSingleton<CommandLineApplication, GitignoreGetCommand>()
               .AddSingleton<IApplicationCommandHandler<GitignoreGetCommand>, GitignoreGetCommandHandler>()
               .AddSingleton<CommandLineApplication, GitignoreListCommand>()
               .AddSingleton<IApplicationCommandHandler<GitignoreListCommand>, GitignoreListCommandHandler>()
               .AddSingleton<IApplicationCommandExecutor, ApplicationCommandsExecutor>()
               .AddSingleton<IGitignoreGithubService, GitignoreGithubService>()
               .AddSingleton<IGitignoreFileWriter, GitignoreFileWriter>()
               .AddSingleton<IConsole, PhysicalConsole>()
               .AddSingleton<IFileNameSpliter, DefaultFileNameSpliter>()
               .AddSingleton<IMergeStrategy, SimpleMergeStrategy>()
               .BuildServiceProvider();

            var executor = servicesProvider.GetRequiredService<IApplicationCommandExecutor>();
            var console = servicesProvider.GetRequiredService<IConsole>();

            try
            {
                return executor.Execute(args);
            }
            catch (Exception e)
            {
                console.WriteLine(e.Message);
                return ReturnCodes.Error;
            }
        }
    }
}