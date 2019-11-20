using System.Collections.Generic;
using System.Threading.Tasks;
using DotnetIgnoreCliTool.Github.Models;
using DotnetIgnoreCliTool.Github.Services;

namespace DotnetIgnoreCliToolTests.Commands.Get.Stubs
{
    public class GitignoreServiceStub : IGitignoreService
    {
        private readonly string _content;

        public GitignoreServiceStub(string content)
        {
            _content = content ?? string.Empty;
        }
        
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