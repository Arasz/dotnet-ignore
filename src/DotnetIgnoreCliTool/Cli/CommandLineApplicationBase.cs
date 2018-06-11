using McMaster.Extensions.CommandLineUtils;

namespace DotnetIgnoreCliTool.Cli
{
    public abstract class CommandLineApplicationBase : CommandLineApplication
    {
        protected abstract void ConfigureCommandLineApplication();
    }
}