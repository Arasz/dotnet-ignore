using DotnetIgnoreCliTool.Cli.Commands.Get;

namespace DotnetIgnoreCliToolTests.Commands.Get
{
    public static class GitignoreGetCommandTestExtensions
    {
        public static void InitOptions(this GitignoreGetCommand command, string name, string destination)
        {
            command.NamesOption.Values.Add(name);
            command.DestinationOption.Values.Add(destination);
        }
    }
}