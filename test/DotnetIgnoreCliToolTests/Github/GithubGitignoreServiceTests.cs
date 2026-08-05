using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CliTool.Github.Models;
using CliTool.Github.Services;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Octokit;
using Xunit;

namespace DotnetIgnoreCliToolTests.Github;

public class GithubGitignoreServiceTests
{
    private const string RepositoryOwner = "github";
    private const string RepositoryName = "gitignore";

    private readonly Mock<IGitHubClient> _gitHubClientMock = new();
    private readonly Mock<IRepositoryContentsClient> _contentsClientMock = new();

    public GithubGitignoreServiceTests()
    {
        var repositoryClientMock = new Mock<IRepositoriesClient>();
        _gitHubClientMock.Setup(client => client.Repository).Returns(repositoryClientMock.Object);
        repositoryClientMock.Setup(client => client.Content).Returns(_contentsClientMock.Object);
    }

    [Fact]
    public async Task ShouldReturnFileContent_WhenNameMatchesExactly()
    {
        // Arrange
        const string expectedContent = "visual studio rules";
        SetupRepositoryContents(CreateContent("VisualStudio.gitignore", "vs-url"));
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(expectedContent)
        });

        // Act
        var result = await service.GetIgnoreFile("VisualStudio.gitignore");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("VisualStudio.gitignore");
        result.Content.Should().Be(expectedContent);
    }

    [Fact]
    public async Task ShouldReturnFileContent_WhenNameWithoutGitignoreSuffixWasGiven()
    {
        // Arrange
        SetupRepositoryContents(CreateContent("VisualStudio.gitignore", "vs-url"));
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("visual studio rules")
        });

        // Act
        var result = await service.GetIgnoreFile("VisualStudio");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("VisualStudio.gitignore");
    }

    [Fact]
    public async Task ShouldReturnFileContent_WhenNameDiffersInCase()
    {
        // Arrange
        SetupRepositoryContents(CreateContent("VisualStudio.gitignore", "vs-url"));
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("visual studio rules")
        });

        // Act
        var result = await service.GetIgnoreFile("visualstudio");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("VisualStudio.gitignore");
    }

    [Fact]
    public async Task ShouldReturnNull_WhenNoFileMatchesTheGivenName()
    {
        // Arrange
        SetupRepositoryContents(CreateContent("VisualStudio.gitignore", "vs-url"));
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var result = await service.GetIgnoreFile("NonExisting");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ShouldFetchRepositoryContentsOnlyOnce_WhenCalledMultipleTimes()
    {
        // Arrange
        SetupRepositoryContents(CreateContent("VisualStudio.gitignore", "vs-url"));
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("visual studio rules")
        });

        // Act
        await service.GetIgnoreFile("VisualStudio");
        await service.GetIgnoreFile("VisualStudio");

        // Assert
        _contentsClientMock.Verify(
            client => client.GetAllContents(RepositoryOwner, RepositoryName),
            Times.Once);
    }

    [Fact]
    public async Task ShouldReturnOnlyGitignoreFileNames()
    {
        // Arrange
        SetupRepositoryContents(
            CreateContent("VisualStudio.gitignore", "vs-url"),
            CreateContent("README.md", "readme-url"),
            new RepositoryContent("docs", "docs", "sha", 1, ContentType.Dir, null!, null!, null!, null!, null!, null!, null!, null!));
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var result = await service.GetAllIgnoreFilesNames();

        // Assert
        result.Should().BeEquivalentTo(new[] { "VisualStudio.gitignore" });
    }

    private static RepositoryContent CreateContent(string name, string downloadUrl) =>
        new(
            name,
            name,
            "sha",
            1,
            ContentType.File,
            $"https://raw.githubusercontent.com/github/gitignore/main/{downloadUrl}",
            "url",
            "git-url",
            "html-url",
            null!,
            null!,
            null!,
            null!);

    private void SetupRepositoryContents(params RepositoryContent[] contents)
    {
        _contentsClientMock
            .Setup(client => client.GetAllContents(RepositoryOwner, RepositoryName))
            .ReturnsAsync((IReadOnlyList<RepositoryContent>)contents);
    }

    private GithubGitignoreService CreateService(HttpResponseMessage downloadResponse)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(downloadResponse);

        return new GithubGitignoreService(_gitHubClientMock.Object, new HttpClient(handlerMock.Object));
    }
}
