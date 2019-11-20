using DotnetIgnoreCliTool.Github.Models;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotnetIgnoreCliTool.Github.Services
{
    public class GitignoreService : IGitignoreService
    {
        private const string RepositoryOwner = "github";
        private readonly IGitHubClient _gitHubClient;
        private const string RepositoryName = "gitignore";
        private const string GitignoreFileName = ".gitignore";
        private readonly HttpClient _httpClient;

        public GitignoreService(IGitHubClient gitHubClient)
        {
            _gitHubClient = gitHubClient ?? throw new ArgumentNullException(nameof(gitHubClient));
            _httpClient = new HttpClient();
        }

        public GitignoreService()
        {
            _gitHubClient = new GitHubClient(new ProductHeaderValue("dotnet-ignore"));
            _httpClient = new HttpClient();
        }

        public async Task<IReadOnlyList<string>> GetAllIgnoreFilesNames()
        {
            IReadOnlyList<RepositoryContent> repositoryContents = await _gitHubClient.Repository.Content.GetAllContents(RepositoryOwner, RepositoryName);
            return repositoryContents
                .Where(content => content.Type.Value == ContentType.File)
                .Where(IsGitignoreFile)
                .Select(content => content.Name)
                .ToImmutableList();
        }

        private bool IsGitignoreFile(RepositoryContent content)
        {
            return content.Name.EndsWith(GitignoreFileName);
        }

        public async Task<GitignoreFile> GetIgnoreFile(string name)
        {
            bool IsRequestedGitignoreFile(RepositoryContent content)
            {
                var isExactlyEqual = string.Equals(content.Name, name, StringComparison.InvariantCultureIgnoreCase);

                if (!isExactlyEqual && IsGitignoreFile(content))
                {
                    return content.Name
                        .Replace(GitignoreFileName, "")
                        .Equals(name, StringComparison.InvariantCultureIgnoreCase);
                }

                return isExactlyEqual;
            }

            IReadOnlyList<RepositoryContent> repositoryContents = await _gitHubClient
                .Repository
                .Content
                .GetAllContents(RepositoryOwner, RepositoryName);

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