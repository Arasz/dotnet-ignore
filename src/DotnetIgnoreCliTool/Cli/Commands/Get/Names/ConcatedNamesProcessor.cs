using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetIgnoreCliTool.Cli.Commands.Get.Names
{
    public sealed class ConcatedNamesProcessor : IConcatedNamesProcessor
    {
        private const string SplitSeparator = ",";

        public string Separator => SplitSeparator;

        public IReadOnlyCollection<string> Process(string fileNames)
        {
            if (fileNames is null)
            {
                throw new ArgumentNullException(nameof(fileNames));
            }

            return fileNames
               .Split(SplitSeparator, StringSplitOptions.RemoveEmptyEntries)
               .Select(name => name.Trim())
               .ToArray();
        }
    }
}