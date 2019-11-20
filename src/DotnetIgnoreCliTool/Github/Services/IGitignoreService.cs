using DotnetIgnoreCliTool.Github.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Github.Services
{
    public interface IGitignoreService
    {
        Task<IReadOnlyList<string>> GetAllIgnoreFilesNames();

        Task<GitignoreFile> GetIgnoreFile(string name);
    }
}