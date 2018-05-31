using DotnetIgnoreCliTool.Github.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Github.Services
{
    public interface IGitignoreGithubService
    {
        Task<IReadOnlyList<string>> GetAllIgnoreFilesNames();

        Task<GitignoreFile> GetIgnoreFile(string name);
    }
}