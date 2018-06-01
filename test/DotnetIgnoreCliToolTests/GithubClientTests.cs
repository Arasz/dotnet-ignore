using FluentAssertions;
using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DotnetIgnoreCliToolTests
{
    public class GithubClientTests : UnitTestBase
    {
        [Fact]
        public async Task GetRepositoryShouldReturnRequestedGitReposiotry()
        {
            //Arange
            var repositoryOwner = "github";
            var gitHubClient = new GitHubClient(new ProductHeaderValue("dotnet-ignore"));
            var repositoryName = "gitignore";

            //Act
            Repository repository = await gitHubClient.Repository.Get(repositoryOwner, repositoryName);

            //Assert

            repository.Name
                .Should()
                .Be(repositoryName);
        }

        [Fact]
        public async Task GetAllRepositoryContentsShouldReturnAllContentInRepository()
        {
            //Arange
            var repositoryOwner = "github";
            var gitHubClient = new GitHubClient(new ProductHeaderValue("dotnet-ignore"));
            var repositoryName = "gitignore";

            //Act
            IReadOnlyList<RepositoryContent> repositoryContents = await gitHubClient.Repository.Content.GetAllContents(repositoryOwner, repositoryName);

            //Assert
            repositoryContents
                .Should()
                .NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RepositoryContentObjectShouldAllowDownloadGitignoreFile()
        {
            //Arange
            var repositoryOwner = "github";
            var gitHubClient = new GitHubClient(new ProductHeaderValue("dotnet-ignore"));
            var repositoryName = "gitignore";
            var httpClient = new HttpClient();

            //Act
            IReadOnlyList<RepositoryContent> repositoryContents = await gitHubClient.Repository.Content.GetAllContents(repositoryOwner, repositoryName);
            RepositoryContent firstFile = repositoryContents
                .First(content => content.Type.Value == ContentType.File);

            HttpResponseMessage fileDownloadResponse = await httpClient.GetAsync(firstFile.DownloadUrl);
            string gitignoreFileContent = await fileDownloadResponse
                .Content
                .ReadAsStringAsync();

            //Assert
            gitignoreFileContent
                .Should()
                .NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RepositoryContentObjectWithSpecifiedContentPathShouldHaveExpectedName()
        {
            //Arange
            var repositoryOwner = "github";
            var gitHubClient = new GitHubClient(new ProductHeaderValue("dotnet-ignore"));
            var repositoryName = "gitignore";
            var expectedFileName = "CUDA.gitignore";

            //Act
            IReadOnlyList<RepositoryContent> repositoryContents = await gitHubClient.Repository.Content.GetAllContents(repositoryOwner, repositoryName);
            RepositoryContent firstFile = repositoryContents
                .First(content => content.Type.Value == ContentType.File && content.Name == expectedFileName);

            //Assert
            firstFile
                .Name
                .Should()
                .Be(expectedFileName);
        }
    }
}