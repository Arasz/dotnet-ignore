using System.Collections.Generic;
using System.Threading.Tasks;
using CliTool.Github.Models;
using CliTool.Github.Services;

namespace DotnetIgnoreCliToolTests.Commands.Get.Stubs
{
    public class GitignoreServiceStub(string content) : IGitignoreService
    {
        private readonly string _content = content ?? string.Empty;

        public Task<IReadOnlyList<string>> GetAllIgnoreFilesNames()
        {
            throw new System.NotImplementedException();
        }

        public Task<GitignoreFile> GetIgnoreFile(string name)
        {
            return Task.FromResult(new GitignoreFile(name, _content));
        }
    }
}