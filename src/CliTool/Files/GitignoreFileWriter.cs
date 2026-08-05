namespace CliTool.Files
{
    public class GitignoreFileWriter : IGitignoreFileWriter
    {
        private const string GitignoreFileName = ".gitignore";

        public async Task WriteToFileAsync(string destination, string content)
        {
            var path = Path.Combine(destination, GitignoreFileName);
            await File.WriteAllTextAsync(path, content);
        }
    }
}
