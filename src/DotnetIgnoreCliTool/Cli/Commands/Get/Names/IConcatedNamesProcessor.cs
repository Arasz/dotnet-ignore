using System.Collections.Generic;

namespace DotnetIgnoreCliTool.Cli.Commands.Get.Names
{
    public interface IConcatedNamesProcessor
    {
        string Separator { get; }
        IReadOnlyCollection<string> Process(string fileNames);
    }
}