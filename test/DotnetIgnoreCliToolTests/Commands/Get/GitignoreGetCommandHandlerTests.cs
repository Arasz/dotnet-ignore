using System;
using System.Threading.Tasks;
using CliTool.Commands;
using CliTool.FIles;
using CliTool.Github.Models;
using CliTool.Github.Services;
using DotnetIgnoreCliToolTests.Commands.Get.Stubs;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotnetIgnoreCliToolTests.Commands.Get
{
    public class GitignoreGetCommandHandlerTest : UnitTestBase
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
                .ReturnsAsync(GitignoreFile.Empty)
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
        public async Task ShouldThrowArgException_WhenFileWriterFailed()
        {
            // Arrange
            const string gitignoreFilename = "test.gitignore";
            const string gitignoreContent = "test content # ./[*]";
            const string destination = "C::::D/test";

            var mergeStrategyStub = new MergeStrategyStub();
            var githubServiceMock = new Mock<IGitignoreService>();
            githubServiceMock
                .Setup(service => service.GetIgnoreFile(It.IsAny<string>()))
                .ReturnsAsync(new GitignoreFile(gitignoreFilename, gitignoreContent))
                .Verifiable();
            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
                .Setup(writer => writer.WriteToFileAsync(destination, It.IsAny<string>()))
                .Throws(new ArgumentException(string.Empty))
                .Verifiable();

            var command = new GetGitIgnoreFileCommand(githubServiceMock.Object, mergeStrategyStub, fileWriterMock.Object);

            // Act
            var executeCommandAction = async () => await command.GetGitIgnoreFile(gitignoreFilename, destination, false);

            // Assert
            await executeCommandAction.Should().ThrowAsync<ArgumentException>();

            githubServiceMock.Verify(service => service.GetIgnoreFile(gitignoreFilename), Times.Once);
            fileWriterMock.Verify(writer => writer.WriteToFileAsync(destination, It.IsAny<string>()), Times.Once);
        }
    }
}