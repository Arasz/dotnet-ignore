using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotnetIgnoreCliTool.Github.Models;

namespace DotnetIgnoreCliTool.Cli.Commands.Get.Merge
{
    public sealed class SimpleGitIgnoreFileMerger : IGitIgnoreFileMerger
    {
        private const string MergedFileName = "merged.gitignore";

        public GitignoreFile Merge(IReadOnlyCollection<GitignoreFile> gitignoreFiles)
        {
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
                stringBuilder.AppendLine($"# {gitignoreFile.Name}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(gitignoreFile.Content);
            }

            return new GitignoreFile(MergedFileName, stringBuilder.ToString());
        }
    }
}