using System.Collections.Generic;
using DotnetIgnoreCliTool.Github.Models;

namespace DotnetIgnoreCliTool.Cli.Commands.Get.Merge
{
    public interface IMergeStrategy
    {
        GitignoreFile Merge(IReadOnlyCollection<GitignoreFile> gitignoreFiles);
    }
}