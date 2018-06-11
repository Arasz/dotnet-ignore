namespace DotnetIgnoreCliTool.Cli.Execution
{
    public interface ICommandHandlerExecutor
    {
        int Execute(string[] args);
    }
}