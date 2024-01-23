namespace CliTool.Github.Models
{
    public sealed class GitignoreFile(string name, string content)
    {
        public string Name { get; } = name;
        public string Content { get; } = content;
        public static GitignoreFile Empty { get; } = new(string.Empty, string.Empty);
    }
}