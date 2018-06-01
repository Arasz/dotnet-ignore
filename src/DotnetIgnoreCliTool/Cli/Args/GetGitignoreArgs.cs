using PowerArgs;

namespace DotnetIgnoreCliTool.Cli.Args
{
    [TabCompletion]
    public class GetGitignoreArgs
    {
        [ArgRequired(PromptIfMissing = true)]
        [ArgDescription(".gitignore file name case insensitive")]
        [ArgShortcut("-n")]
        [ArgExample("VisualStudio.gitignore", "complete file name")]
        [ArgExample("visualstudio", "only enviroment part of file name")]
        public string Name { get; set; }

        [ArgDescription("destination directory where gitignore will be saved. If not provided execution directory will be used")]
        [ArgShortcut("-d")]
        [ArgExample("C:/", "example directory path")]
        [ArgExistingDirectory]
        public string Destination { get; set; }
    }
}