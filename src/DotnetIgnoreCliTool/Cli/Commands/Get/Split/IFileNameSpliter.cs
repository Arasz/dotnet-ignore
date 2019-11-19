using System.Collections.Generic;

namespace DotnetIgnoreCliTool.Cli.Commands.Get.Split
{
    public interface IFileNameSpliter
    {
        string Separator { get; }
        IReadOnlyCollection<string> Split(string fileNames);
    }
}