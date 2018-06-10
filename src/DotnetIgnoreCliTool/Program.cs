using DotnetIgnoreCliTool.Cli;
using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool
{
    internal class Program
    {
        public static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<CommandLineEntryPoint>(args);
    }
}