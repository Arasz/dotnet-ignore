using CliTool.Github.Models;

namespace CliTool.Github.Services
{
    public interface IGitignoreService
    {
        Task<IReadOnlyList<string>> GetAllIgnoreFilesNames();

        Task<GitignoreFile> GetIgnoreFile(string name);
    }
}