using DotnetIgnoreCliTool.Cli.Commands;
using DotnetIgnoreCliTool.Cli.FIles;
using DotnetIgnoreCliTool.Cli.Output;
using DotnetIgnoreCliTool.Github.Services;
using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli
{
    public class CommandLineEntryPoint
    {
        [Argument(0)]
        public string CommandName { get; set; }

        private Task OnExecuteAsync()
        {
            if (string.IsNullOrEmpty(CommandName))
            {
                //Print help
            }

            var commands = new List<ICommandHandler>
            {
                new GitignoreGetCommandHandler(_githubService, _gitignoreFileWriter),
                new GitignoreListCommandHandler(_githubService, _output)
            };

            var requestedCommand = commands
                .FirstOrDefault(handler => handler.CanHandle(CommandName));

            if (requestedCommand is null)
            {
                // Print wrong command name prompt
                return Task.CompletedTask;
            }

            return requestedCommand.ExecuteAsync();
        }

        private readonly IOutput _output;
        private readonly IGitignoreGithubService _githubService;
        private readonly IGitignoreFileWriter _gitignoreFileWriter;

        public CommandLineEntryPoint()
        {
            _output = new ConsoleOutput();
            _githubService = new GitignoreGithubService();
            _gitignoreFileWriter = new GitignoreFileWritter();
        }

        //TODO Merge action - merges given gitignore files into one file
    }
}