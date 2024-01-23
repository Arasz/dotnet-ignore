using System.Text;
using CliTool.Github.Models;

namespace CliTool.Merge
{
    public sealed class SimpleMergeStrategy : IMergeStrategy
    {

        public const string CreatedFileMessage = "# Git ignore file created by dotnet-ignore";
        public const string MergedFileName = ".gitignore";
        private const string CommentLineMarker = "#";
        private const string NewLineSeparator = "\n";

        public GitignoreFile Merge(IReadOnlyCollection<GitignoreFile> gitignoreFiles, bool minimizeFileSize = false)
        {
            ArgumentNullException.ThrowIfNull(gitignoreFiles);

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("#");
            stringBuilder.AppendLine($"{CreatedFileMessage} on {DateTime.Now:g}");
            if (minimizeFileSize)
            {
                stringBuilder.AppendLine("# Minimized file size. Comments, duplicated lines and empty lines where removed.");
            }

            stringBuilder.AppendLine($"# Created from {gitignoreFiles.Count} .gitignore files: {string.Join(", ", gitignoreFiles.Select(file => file.Name))}");
            stringBuilder.AppendLine("#");

            if (minimizeFileSize)
            {
                return MergeWithFileSizeMinimalization(gitignoreFiles, stringBuilder);
            }

            foreach (var gitignoreFile in gitignoreFiles)
            {
                stringBuilder.AppendLine(gitignoreFile.Content);
            }

            return new GitignoreFile(MergedFileName, stringBuilder.ToString());
        }

        private static GitignoreFile MergeWithFileSizeMinimalization(IEnumerable<GitignoreFile> gitignoreFiles, StringBuilder stringBuilder)
        {
            var ignoreFileLines = new HashSet<string>();

            foreach (var gitignoreFile in gitignoreFiles)
            {
                foreach (var fileLine in gitignoreFile.Content.Split(NewLineSeparator, StringSplitOptions.TrimEntries))
                {
                    if (fileLine.StartsWith(CommentLineMarker) || string.IsNullOrWhiteSpace(fileLine))
                    {
                        continue;
                    }

                    if (ignoreFileLines.Add(fileLine))
                    {
                        stringBuilder.AppendLine(fileLine);
                    }
                }
            }

            return new GitignoreFile(MergedFileName, stringBuilder.ToString());
        }
    }
}