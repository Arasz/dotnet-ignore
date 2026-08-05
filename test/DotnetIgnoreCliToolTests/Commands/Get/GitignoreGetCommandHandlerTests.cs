using System;
using System.IO;
using System.Threading.Tasks;
using CliTool.Commands;
using CliTool.Files;
using CliTool.Github.Models;
using CliTool.Github.Services;
using DotnetIgnoreCliToolTests.Commands.Get.Stubs;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotnetIgnoreCliToolTests.Commands.Get
{
    public class GitignoreGetCommandHandlerTest
    {
        [Fact]
        public async Task ShouldWriteGitignoreFile_WhenSingleGitignoreFileNameWasGiven()
        {
            // Arrange
            const string gitignoreFilename = "test.gitignore";
            const string gitignoreContent = "test content # ./[*]";
            const string destination = "C:/test";

            var mergeStrategyStub = new MergeStrategyStub();
            var githubServiceStub = new GitignoreServiceStub(gitignoreContent);
            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
                .Setup(writer => writer.WriteToFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var command = new GetGitIgnoreFileCommand(githubServiceStub, mergeStrategyStub, fileWriterMock.Object);

            // Act
            var executeCommandAction = async () => await command.GetGitIgnoreFile(gitignoreFilename, destination, false);

            // Assert
            await executeCommandAction.Should().NotThrowAsync();

            fileWriterMock.Verify(writer => writer.WriteToFileAsync(destination, gitignoreContent), Times.Once);
        }

        [Fact]
        public async Task ShouldWriteMergedGitignoreFile_WhenCalledWithMultipleNames()
        {
            // Arrange
            const string gitignoreFilenames = "test.gitignore,test2.gitignore"; // Separator as in split stub
            const string gitignoreContent = "test content # ./[*]";
            const string destination = "C:/test";
            const string expectedContent = gitignoreContent + gitignoreContent; // As in merge stub

            var mergeStrategyStub = new MergeStrategyStub();
            var githubServiceStub = new GitignoreServiceStub(gitignoreContent);
            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
                .Setup(writer => writer.WriteToFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var command = new GetGitIgnoreFileCommand(githubServiceStub, mergeStrategyStub, fileWriterMock.Object);

            // Act
            var executeCommandAction = async () => await command.GetGitIgnoreFile(gitignoreFilenames, destination, false);

            // Assert
            await executeCommandAction.Should().NotThrowAsync();

            fileWriterMock.Verify(writer => writer.WriteToFileAsync(destination, expectedContent), Times.Once);
        }


        [Fact]
        public async Task ShouldThrowArgException_WhenNoFileForNameWasFound()
        {
            // Arrange
            const string gitignoreFilename = "non-existing.gitignore";
            const string destination = "C:/test";

            var mergeStrategyStub = new MergeStrategyStub();
            var githubServiceMock = new Mock<IGitignoreService>();
            githubServiceMock
                .Setup(service => service.GetIgnoreFile(gitignoreFilename))
                .ReturnsAsync((GitignoreFile?)null)
                .Verifiable();
            var fileWriterMock = new Mock<IGitignoreFileWriter>();

            var command = new GetGitIgnoreFileCommand(githubServiceMock.Object, mergeStrategyStub, fileWriterMock.Object);

            // Act
            var executeCommandAction = async () => await command.GetGitIgnoreFile(gitignoreFilename, destination, false);

            // Assert
            await executeCommandAction.Should().ThrowAsync<ArgumentException>();

            githubServiceMock.Verify(service => service.GetIgnoreFile(gitignoreFilename), Times.Once);
        }

        [Fact]
        public async Task ShouldPropagateIOException_WhenFileWriterFails()
        {
            // Arrange
            const string gitignoreFilename = "test.gitignore";
            const string gitignoreContent = "test content # ./[*]";
            const string destination = "C:/test";

            var mergeStrategyStub = new MergeStrategyStub();
            var githubServiceMock = new Mock<IGitignoreService>();
            githubServiceMock
                .Setup(service => service.GetIgnoreFile(It.IsAny<string>()))
                .ReturnsAsync(new GitignoreFile(gitignoreFilename, gitignoreContent));
            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
                .Setup(writer => writer.WriteToFileAsync(destination, It.IsAny<string>()))
                .Throws(new IOException("disk full"));

            var command = new GetGitIgnoreFileCommand(githubServiceMock.Object, mergeStrategyStub, fileWriterMock.Object);

            // Act
            var executeCommandAction = async () => await command.GetGitIgnoreFile(gitignoreFilename, destination, false);

            // Assert
            await executeCommandAction.Should().ThrowAsync<IOException>();
        }

        [Fact]
        public async Task ShouldPropagateUnauthorizedAccessException_WhenFileWriterFails()
        {
            // Arrange
            const string gitignoreFilename = "test.gitignore";
            const string gitignoreContent = "test content # ./[*]";
            const string destination = "C:/test";

            var mergeStrategyStub = new MergeStrategyStub();
            var githubServiceMock = new Mock<IGitignoreService>();
            githubServiceMock
                .Setup(service => service.GetIgnoreFile(It.IsAny<string>()))
                .ReturnsAsync(new GitignoreFile(gitignoreFilename, gitignoreContent));
            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
                .Setup(writer => writer.WriteToFileAsync(destination, It.IsAny<string>()))
                .Throws(new UnauthorizedAccessException("access denied"));

            var command = new GetGitIgnoreFileCommand(githubServiceMock.Object, mergeStrategyStub, fileWriterMock.Object);

            // Act
            var executeCommandAction = async () => await command.GetGitIgnoreFile(gitignoreFilename, destination, false);

            // Assert
            await executeCommandAction.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task ShouldWriteToCurrentDirectory_WhenDestinationIsNotProvided()
        {
            // Arrange
            const string gitignoreFilename = "test.gitignore";
            const string gitignoreContent = "test content # ./[*]";

            var mergeStrategyStub = new MergeStrategyStub();
            var githubServiceMock = new Mock<IGitignoreService>();
            githubServiceMock
                .Setup(service => service.GetIgnoreFile(It.IsAny<string>()))
                .ReturnsAsync(new GitignoreFile(gitignoreFilename, gitignoreContent));
            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
                .Setup(writer => writer.WriteToFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var command = new GetGitIgnoreFileCommand(githubServiceMock.Object, mergeStrategyStub, fileWriterMock.Object);

            // Act
            await command.GetGitIgnoreFile(gitignoreFilename, null, false);

            // Assert
            fileWriterMock.Verify(writer => writer.WriteToFileAsync(Environment.CurrentDirectory, gitignoreContent), Times.Once);
        }
    }
}