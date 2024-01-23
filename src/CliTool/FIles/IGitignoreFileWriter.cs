namespace CliTool.FIles
{
    public interface IGitignoreFileWriter
    {
        Task WriteToFileAsync(string destination, string content);
    }
}