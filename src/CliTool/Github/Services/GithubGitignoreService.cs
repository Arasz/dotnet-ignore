using System.Collections.Immutable;
using CliTool.Github.Models;
using Octokit;

namespace CliTool.Github.Services
{
    public class GithubGitignoreService : IGitignoreService
    {
        private const string RepositoryOwner = "github";
        private readonly IGitHubClient _gitHubClient;
        private const string RepositoryName = "gitignore";
        private const string GitignoreFileName = ".gitignore";
        private readonly HttpClient _httpClient;

        public GithubGitignoreService(IGitHubClient gitHubClient)
        {
            _gitHubClient = gitHubClient ?? throw new ArgumentNullException(nameof(gitHubClient));
            _httpClient = new HttpClient();
        }

        public GithubGitignoreService()
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

        private static bool IsGitignoreFile(RepositoryContent content) => content.Name.EndsWith(GitignoreFileName);

        public async Task<GitignoreFile> GetIgnoreFile(string name)
        {
            var repositoryContents = await _gitHubClient
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
            var gitignoreFileContent = await fileDownloadResponse
                .Content
                .ReadAsStringAsync();

            return new GitignoreFile(gitignoreFile.Name, gitignoreFileContent);

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
        }
    }
}