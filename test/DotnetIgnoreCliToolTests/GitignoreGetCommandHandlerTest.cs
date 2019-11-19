using System.Threading.Tasks;
using DotnetIgnoreCliTool.Cli;
using DotnetIgnoreCliTool.Cli.Commands.Get;
using DotnetIgnoreCliTool.Cli.Files;
using DotnetIgnoreCliTool.Github.Models;
using DotnetIgnoreCliTool.Github.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotnetIgnoreCliToolTests
{
    public class GitignoreGetCommandHandlerTest : UnitTestBase
    {
        [Fact]
        public async Task HandleShouldWriteGitignoreFileWhenSuccessful()
        {
            // Arrange
            const string gitIgnoreFileName = "test.gitignore";
            const string gitIgnoreContent = "SomeContent";
            const string destination = "SomeDest";

            var githubServiceMock = new Mock<IGitignoreGithubService>();
            githubServiceMock
               .Setup(service => service.GetIgnoreFile(gitIgnoreFileName))
               .ReturnsAsync(new GitignoreFile(gitIgnoreFileName, gitIgnoreContent))
               .Verifiable();

            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
               .Setup(writer => writer.WriteToFileAsync(destination, gitIgnoreContent))
               .Returns(Task.CompletedTask)
               .Verifiable();

            var handler = new GitignoreGetCommandHandler(githubServiceMock.Object, fileWriterMock.Object);

            var command = new GitignoreGetCommand(handler);
            command.NamesOption.Values.Add(gitIgnoreFileName);
            command.DestinationOption.Values.Add(destination);


            // Act
            var result = await handler.HandleCommandAsync(command);

            // Assert
            result
               .Should()
               .Be(ReturnCodes.Success);

            githubServiceMock.Verify(service => service.GetIgnoreFile(gitIgnoreFileName), Times.Once);
            fileWriterMock.Verify(writer => writer.WriteToFileAsync(destination, gitIgnoreContent), Times.Once);
        }
    }
}