namespace CliTool.Files
{
    public interface IGitignoreFileWriter
    {
        Task WriteToFileAsync(string destination, string content);
    }
}