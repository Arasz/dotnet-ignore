using System.Collections.Generic;
using System.Linq;
using CliTool.Github.Models;
using CliTool.Merge;

namespace DotnetIgnoreCliToolTests.Commands.Get.Stubs
{
    public class MergeStrategyStub : IMergeStrategy
    {
        public GitignoreFile Merge(IReadOnlyCollection<GitignoreFile> gitignoreFiles, bool minimizeFileSize = false)
        {
            var mergedContent = gitignoreFiles
                .Select(file => file.Content)
                .Aggregate((content, accum) => accum + content);

            return new GitignoreFile("MERGED", mergedContent);
        }
    }
}