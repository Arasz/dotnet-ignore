using System.Collections.Generic;
using System.Threading.Tasks;
using DotnetIgnoreCliTool.Github.Models;
using DotnetIgnoreCliTool.Github.Services;

namespace DotnetIgnoreCliToolTests.Commands.Get.Stubs
{
    public class GithubServiceStub : IGitignoreGithubService
    {
        private readonly string _content;

        public GithubServiceStub(string content)
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