using System.Text;
using CliTool.Github.Models;

namespace CliTool.Merge
{
    public sealed class SimpleMergeStrategy : IMergeStrategy
    {
        private const string MergedFileName = "merged.gitignore";

        public GitignoreFile Merge(IReadOnlyCollection<GitignoreFile> gitignoreFiles)
        {
            if (gitignoreFiles is null)
            {
                throw new ArgumentNullException(nameof(gitignoreFiles));
            }

            if (gitignoreFiles.Count == 1)
            {
                return gitignoreFiles.First();
            }

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"# Git ignore file created by dotnet-ignore on {DateTime.Now:f}");
            stringBuilder.AppendLine($"# Merged from {gitignoreFiles.Count} .gitignore files");
            stringBuilder.AppendLine();

            foreach (var gitignoreFile in gitignoreFiles)
            {
                stringBuilder.AppendLine($"# [[{gitignoreFile.Name}]]");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(gitignoreFile.Content);
            }

            return new GitignoreFile(MergedFileName, stringBuilder.ToString());
        }
    }
}