using System;
using System.Collections.Generic;

namespace DotnetIgnoreCliTool.Cli.Commands.Get.Split
{
    public sealed class DefaultFileNameSpliter : IFileNameSpliter
    {
        private const string SplitSeparator = ",";

        public string Separator => SplitSeparator;

        public IReadOnlyCollection<string> Split(string fileNames)
        {
            if (fileNames is null)
            {
                throw new ArgumentNullException(nameof(fileNames));
            }

            return fileNames.Split(SplitSeparator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}