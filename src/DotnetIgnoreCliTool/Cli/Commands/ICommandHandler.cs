using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.Commands
{
    public interface ICommandHandler
    {
        bool CanHandle(string command);

        Task ExecuteAsync();
    }
}