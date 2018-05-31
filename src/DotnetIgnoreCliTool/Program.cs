using DotnetIgnoreCliTool.Cli;
using PowerArgs;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await Args.InvokeActionAsync<CommandLineEntryPoint>(args);
        }
    }
}