namespace DotnetIgnoreCliTool.Cli.Execution
{
    public interface IApplicationCommandExecutor
    {
        int Execute(string[] args);
    }
}