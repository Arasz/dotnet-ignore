using System.Collections.Immutable;
using CliTool.Github.Models;
using Octokit;

namespace CliTool.Github.Services;

public sealed class GithubGitignoreService(IGitHubClient gitHubClient, HttpClient httpClient) : IGitignoreService
{
    private const string RepositoryOwner = "github";
    private const string RepositoryName = "gitignore";
    private const string GitignoreFileName = ".gitignore";

    private IReadOnlyList<RepositoryContent>? _repositoryContents;

    public async Task<IReadOnlyList<string>> GetAllIgnoreFilesNames()
    {
        var repositoryContents = await GetRepositoryContents();
        return repositoryContents
            .Where(content => content.Type == ContentType.File)
            .Where(IsGitignoreFile)
            .Select(content => content.Name)
            .ToImmutableList();
    }

    public async Task<GitignoreFile?> GetIgnoreFile(string name)
    {
        var repositoryContents = await GetRepositoryContents();

        var gitignoreFile = repositoryContents
            .FirstOrDefault(content => content.Type == ContentType.File && IsRequestedGitignoreFile(content, name));

        if (gitignoreFile is null)
        {
            return null;
        }

        var fileDownloadResponse = await httpClient.GetAsync(gitignoreFile.DownloadUrl);
        fileDownloadResponse.EnsureSuccessStatusCode();
        var gitignoreFileContent = await fileDownloadResponse
            .Content
            .ReadAsStringAsync();

        return new GitignoreFile(gitignoreFile.Name, gitignoreFileContent);
    }

    private async Task<IReadOnlyList<RepositoryContent>> GetRepositoryContents()
    {
        return _repositoryContents ??=
            await gitHubClient.Repository.Content.GetAllContents(RepositoryOwner, RepositoryName);
    }

    private static bool IsGitignoreFile(RepositoryContent content) => content.Name.EndsWith(GitignoreFileName);

    private static bool IsRequestedGitignoreFile(RepositoryContent content, string name)
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
