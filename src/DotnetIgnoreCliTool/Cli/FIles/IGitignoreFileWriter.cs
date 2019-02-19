using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.Files
{
    public interface IGitignoreFileWriter
    {
        Task WriteToFileAsync(string destination, string content);
    }
}