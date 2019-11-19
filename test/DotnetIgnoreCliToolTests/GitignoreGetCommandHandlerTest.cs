using System;
using System.Threading.Tasks;
using DotnetIgnoreCliTool.Cli;
using DotnetIgnoreCliTool.Cli.Commands.Get;
using DotnetIgnoreCliTool.Cli.Files;
using DotnetIgnoreCliTool.Github.Models;
using DotnetIgnoreCliTool.Github.Services;
using FluentAssertions;
using Moq;
using PowerArgs;
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
            const string gitIgnoreContent = "test content # ./[*]";
            const string destination = "C:/test";

            var githubServiceMock = new Mock<IGitignoreGithubService>();
            githubServiceMock
               .Setup(service => service.GetIgnoreFile(It.IsAny<string>()))
               .ReturnsAsync<string, IGitignoreGithubService, GitignoreFile>(
                    fileName => new GitignoreFile(fileName, gitIgnoreContent)
                )
               .Verifiable();

            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
               .Setup(writer => writer.WriteToFileAsync(It.IsAny<string>(), It.IsAny<string>()))
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

        [Fact]
        public async Task HandleShouldThrowArgExceptionWhenNoFileForNameWasFound()
        {
            // Arrange
            const string gitIgnoreFileName = "non-existing.gitignore";
            const string destination = "C:/test";

            var githubServiceMock = new Mock<IGitignoreGithubService>();
            githubServiceMock
               .Setup(service => service.GetIgnoreFile(gitIgnoreFileName))
               .ReturnsAsync(GitignoreFile.Empty)
               .Verifiable();

            var fileWriterMock = new Mock<IGitignoreFileWriter>();

            var handler = new GitignoreGetCommandHandler(githubServiceMock.Object, fileWriterMock.Object);

            var command = new GitignoreGetCommand(handler);
            command.NamesOption.Values.Add(gitIgnoreFileName);
            command.DestinationOption.Values.Add(destination);

            // Act
            Func<Task> handleCall = () => handler.HandleCommandAsync(command);

            // Assert
            await handleCall
               .Should()
               .ThrowAsync<ArgException>();
            githubServiceMock.Verify(service => service.GetIgnoreFile(gitIgnoreFileName), Times.Once);
        }

        [Fact]
        public async Task HandleShouldThrowArgExceptionWhenFileWriterFailed()
        {
            // Arrange
            const string gitIgnoreFileName = "test.gitignore";
            const string gitIgnoreContent = "test content # ./[*]";
            const string destination = "C::::D/test";

            var githubServiceMock = new Mock<IGitignoreGithubService>();
            githubServiceMock
               .Setup(service => service.GetIgnoreFile(It.IsAny<string>()))
               .ReturnsAsync(new GitignoreFile(gitIgnoreFileName, gitIgnoreContent))
               .Verifiable();

            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
               .Setup(writer => writer.WriteToFileAsync(destination, It.IsAny<string>()))
               .Throws(new ArgException(string.Empty))
               .Verifiable();

            var handler = new GitignoreGetCommandHandler(githubServiceMock.Object, fileWriterMock.Object);

            var command = new GitignoreGetCommand(handler);
            command.NamesOption.Values.Add(gitIgnoreFileName);
            command.DestinationOption.Values.Add(destination);


            // Act
            Func<Task> handleCall = () => handler.HandleCommandAsync(command);

            // Assert
            await handleCall
               .Should()
               .ThrowAsync<ArgException>();

            githubServiceMock.Verify(service => service.GetIgnoreFile(gitIgnoreFileName), Times.Once);
            fileWriterMock.Verify(writer => writer.WriteToFileAsync(destination, It.IsAny<string>()), Times.Once);
        }
    }
}