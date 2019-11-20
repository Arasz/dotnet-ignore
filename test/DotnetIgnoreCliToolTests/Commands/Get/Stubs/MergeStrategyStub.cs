using System.Collections.Generic;
using System.Linq;
using DotnetIgnoreCliTool.Cli.Commands.Get.Merge;
using DotnetIgnoreCliTool.Github.Models;

namespace DotnetIgnoreCliToolTests.Stubs
{
    public class MergeStrategyStub : IMergeStrategy
    {
        public GitignoreFile Merge(IReadOnlyCollection<GitignoreFile> gitignoreFiles)
        {
            var mergedContent = gitignoreFiles
               .Select(file => file.Content)
               .Aggregate((content, accum) => accum + content);

            return new GitignoreFile("MERGED", mergedContent);
        }
    }
}