using System;
using System.Collections.Generic;
using DotnetIgnoreCliTool.Cli.Commands.Get.Names;

namespace DotnetIgnoreCliToolTests.Commands.Get.Stubs
{
    public class ConcatedNamesProcessorStub : IConcatedNamesProcessor
    {
        public string Separator => ",";

        public IReadOnlyCollection<string> Process(string fileNames)
        {
            return fileNames.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}