using System;
using System.Collections.Generic;
using DotnetIgnoreCliTool.Cli.Commands.Get.Split;

namespace DotnetIgnoreCliToolTests.Stubs
{
    public class FileNameSpliterStub : IFileNameSpliter
    {
        public string Separator => ",";

        public IReadOnlyCollection<string> Split(string fileNames)
        {
            return fileNames.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}