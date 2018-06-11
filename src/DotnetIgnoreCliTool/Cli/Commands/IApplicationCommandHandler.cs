using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.Commands
{
    public interface IApplicationCommandHandler<in TCommand>
        where TCommand : CommandLineApplication
    {
        Task<int> HandleCommandAsync(TCommand command);
    }
}