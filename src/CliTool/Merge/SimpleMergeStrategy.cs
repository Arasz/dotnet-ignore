using System.Text;
using CliTool.Github.Models;

namespace CliTool.Merge
{
    public sealed class SimpleMergeStrategy : IMergeStrategy
    {
        public const string CreatedFileMessage = "# Git ignore file created by dotnet-ignore";
        public const string MergedFileName = ".gitignore";
        private const string CommentLineMarker = "#";

        public GitignoreFile Merge(IReadOnlyCollection<GitignoreFile> gitignoreFiles, bool removeComments = false)
        {
            ArgumentNullException.ThrowIfNull(gitignoreFiles);

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"{CreatedFileMessage} on {DateTime.Now:f}");
            stringBuilder.AppendLine($"# Created from {gitignoreFiles.Count} .gitignore files:");
            stringBuilder.AppendLine($"# {string.Join(", ", gitignoreFiles.Select(file => file.Name))}");
            stringBuilder.AppendLine();

            var ignoreFileLines = new HashSet<string>();

            foreach (var gitignoreFile in gitignoreFiles)
            {
                foreach (var ignoreLine in gitignoreFile.Content.Split(Environment.NewLine))
                {
                    if (removeComments && ignoreLine.StartsWith(CommentLineMarker))
                    {
                        continue;
                    }

                    if (ignoreFileLines.Add(ignoreLine))
                    {
                        stringBuilder.AppendLine(ignoreLine);
                    }
                }
            }

            return new GitignoreFile(MergedFileName, stringBuilder.ToString());
        }
    }
}