using PowerArgs;

namespace DotnetIgnoreCliTool.Cli.Args
{
    public class ListGitignoreArgs
    {
        [ArgDescription("prints file names without .gitignore")]
        [ArgShortcut("-s")]
        public bool Short { get; set; }
    }
}