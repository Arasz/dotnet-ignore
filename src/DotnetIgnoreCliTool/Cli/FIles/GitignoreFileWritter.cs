using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Cli.FIles
{
    public class GitignoreFileWritter : IGitignoreFileWriter
    {
        private const string GitignoreFileName = ".gitignore";

        public async Task WriteToFileAsync(string destination, string content)
        {
            if (string.IsNullOrEmpty(destination))
            {
                destination = Directory.GetCurrentDirectory();
            }

            var path = Path.Combine(destination, GitignoreFileName);
            using (var fileStream = File.Create(path))
            {
                var convertedContent = Encoding.UTF8.GetBytes(content);
                await fileStream.WriteAsync(convertedContent);
            }
        }
    }
}