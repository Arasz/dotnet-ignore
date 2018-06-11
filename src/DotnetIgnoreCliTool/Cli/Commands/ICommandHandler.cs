using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.Commands
{
    public interface ICommandHandler
    {
        Task<int> HandleCommandAsync();
    }
}