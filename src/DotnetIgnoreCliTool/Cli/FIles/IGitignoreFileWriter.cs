using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.FIles
{
    public interface IGitignoreFileWriter
    {
        Task WriteToFileAsync(string destination, string content);
    }
}