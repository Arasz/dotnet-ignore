using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Octokit;
using Xunit;

namespace DotnetIgnoreCliToolTests.Github
{
    public class GithubClientTests : UnitTestBase
    {
        [Fact]
        public async Task GetRepositoryShouldReturnRequestedGitRepository()
        {
            //Arrange
            var repositoryOwner = "github";
            var gitHubClient = new GitHubClient(new ProductHeaderValue("dotnet-ignore"));
            var repositoryName = "gitignore";

            //Act
            var repository = await gitHubClient.Repository.Get(repositoryOwner, repositoryName);

            //Assert

            repository.Name
                .Should()
                .Be(repositoryName);
        }

        [Fact]
        public async Task GetAllRepositoryContentsShouldReturnAllContentInRepository()
        {
            //Arrange
            var repositoryOwner = "github";
            var gitHubClient = new GitHubClient(new ProductHeaderValue("dotnet-ignore"));
            var repositoryName = "gitignore";

            //Act
            var repositoryContents = await gitHubClient.Repository.Content.GetAllContents(repositoryOwner, repositoryName);

            //Assert
            repositoryContents
                .Should()
                .NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RepositoryContentObjectShouldAllowDownloadGitignoreFile()
        {
            //Arrange
            var repositoryOwner = "github";
            var gitHubClient = new GitHubClient(new ProductHeaderValue("dotnet-ignore"));
            var repositoryName = "gitignore";
            var httpClient = new HttpClient();

            //Act
            var repositoryContents = await gitHubClient.Repository.Content.GetAllContents(repositoryOwner, repositoryName);
            var firstFile = repositoryContents
                .First(content => content.Type.Value == ContentType.File);

            var fileDownloadResponse = await httpClient.GetAsync(firstFile.DownloadUrl);
            var gitignoreFileContent = await fileDownloadResponse
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
            //Arrange
            var repositoryOwner = "github";
            var gitHubClient = new GitHubClient(new ProductHeaderValue("dotnet-ignore"));
            var repositoryName = "gitignore";
            var expectedFileName = "CUDA.gitignore";

            //Act
            var repositoryContents = await gitHubClient.Repository.Content.GetAllContents(repositoryOwner, repositoryName);
            var firstFile = repositoryContents
                .First(content => content.Type.Value == ContentType.File && content.Name == expectedFileName);

            //Assert
            firstFile
                .Name
                .Should()
                .Be(expectedFileName);
        }
    }
}