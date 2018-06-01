using PowerArgs;

namespace DotnetIgnoreCliTool.Cli.Args
{
    [TabCompletion]
    public class ListGitignoreArgs
    {
        [ArgDescription("prints file names without .gitignore")]
        [ArgShortcut("-s")]
        public bool Short { get; set; }
    }
}