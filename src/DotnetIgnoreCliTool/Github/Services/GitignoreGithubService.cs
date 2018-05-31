using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Github.Services
{
    public class GitignoreGithubService : IGitignoreGithubService
    {
        private const string RepositoryOwner = "gtihub";
        private readonly GitHubClient _gitHubClient;
        private const string RepositoryName = "gitignore";
        private readonly HttpClient _httpClient;

        public GitignoreGithubService()
        {
            _gitHubClient = new GitHubClient(new ProductHeaderValue("dotnet-ignore"));
            _httpClient = new HttpClient();
        }

        public async Task<IReadOnlyList<string>> GetAllIgnoreFilesNames()
        {
            IReadOnlyList<RepositoryContent> repositoryContents = await _gitHubClient.Repository.Content.GetAllContents(RepositoryOwner, RepositoryName);
            return repositoryContents
                .Where(content => content.Type.Value == ContentType.File)
                .Select(content => content.Name)
                .ToImmutableList();
        }

        public async Task<GitignoreFile> GetIgnoreFile(string name)
        {
            bool IsRequestedGitignoreFile(RepositoryContent content)
            {
                var isExactlyEqual = string.Equals(content.Name, name, StringComparison.InvariantCultureIgnoreCase);

                const string gitignoreFileName = ".gitignore";
                if (!isExactlyEqual && name.Contains(gitignoreFileName))
                {
                    return content.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase);
                }

                return isExactlyEqual;
            }

            IReadOnlyList<RepositoryContent> repositoryContents = await _gitHubClient.Repository.Content.GetAllContents(RepositoryOwner, RepositoryName);
            var gitignoreFile = repositoryContents
                .FirstOrDefault(content => content.Type.Value == ContentType.File && IsRequestedGitignoreFile(content));

            if (gitignoreFile is null)
            {
                return GitignoreFile.Empty;
            }

            var fileDownloadResponse = await _httpClient.GetAsync(gitignoreFile.DownloadUrl);
            string gitignoreFileContent = await fileDownloadResponse
                .Content
                .ReadAsStringAsync();

            return new GitignoreFile(gitignoreFile.Name, gitignoreFileContent);
        }
    }
}