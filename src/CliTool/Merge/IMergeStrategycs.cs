using CliTool.Github.Models;

namespace CliTool.Merge
{
    public interface IMergeStrategy
    {
        GitignoreFile Merge(IReadOnlyCollection<GitignoreFile> gitignoreFiles);
    }
}