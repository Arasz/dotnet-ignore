using PowerArgs;

namespace DotnetIgnoreCliTool.Cli.Models
{
    [TabCompletion]
    public class GetGitignoreArgs
    {
        [ArgRequired(PromptIfMissing = true)]
        [ArgDescription(".gitignore file name case insensitive")]
        [ArgShortcut("-n")]
        [ArgExample("VisualStudio.gitignore", "Complete file name")]
        [ArgExample("visualstudio", "Only enviroment part of file name")]
        public string Name { get; set; }

        [ArgDescription("Destination directory where gitignore will be saved")]
        [ArgShortcut("-d")]
        [ArgExample("C:/", "Example directory path")]
        [ArgExistingDirectory]
        public string Destination { get; set; }
    }
}