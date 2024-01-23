using System;
using System.Threading.Tasks;
using DotnetIgnoreCliTool.Cli;
using DotnetIgnoreCliTool.Cli.Commands.Get;
using DotnetIgnoreCliTool.Cli.FIles;
using DotnetIgnoreCliTool.Github.Models;
using DotnetIgnoreCliTool.Github.Services;
using DotnetIgnoreCliToolTests.Commands.Get.Extensions;
using DotnetIgnoreCliToolTests.Commands.Get.Stubs;
using FluentAssertions;
using McMaster.Extensions.CommandLineUtils;
using Moq;
using PowerArgs;
using Xunit;

namespace DotnetIgnoreCliToolTests.Commands.Get
{
    public class GitignoreGetCommandHandlerTest : UnitTestBase
    {
        [Fact]
        public async Task HandleShouldWriteGitignoreFileWhenSuccessful()
        {
            // Arrange
            const string gitignoreFilename = "test.gitignore";
            const string gitignoreContent = "test content # ./[*]";
            const string destination = "C:/test";

            var githubServiceStub = new GitignoreServiceStub(gitignoreContent);

            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
               .Setup(writer => writer.WriteToFileAsync(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.CompletedTask)
               .Verifiable();

            var (command, handler) = CreateCommandAndHandler(githubServiceStub, fileWriterMock.Object);

            command.InitOptions(gitignoreFilename, destination);


            // Act
            var result = await handler.HandleCommandAsync(command);

            // Assert
            result
               .Should()
               .Be(ReturnCodes.Success);
            
            fileWriterMock.Verify(writer => writer.WriteToFileAsync(destination, gitignoreContent), Times.Once);
        }
        
        [Fact]
        public async Task HandleShouldWriteMergedGitignoreFileWhenCalledWithMultipleNames()
        {
            // Arrange
            const string gitignoreFilenames = "test.gitignore,test2.gitignore"; // Separator as in split stub
            const string gitignoreContent = "test content # ./[*]";
            const string destination = "C:/test";
            const string expectedContent = gitignoreContent + gitignoreContent; // As in merge stub

            var githubServiceStub = new GitignoreServiceStub(gitignoreContent);

            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
               .Setup(writer => writer.WriteToFileAsync(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.CompletedTask)
               .Verifiable();

            var (command, handler) = CreateCommandAndHandler(githubServiceStub, fileWriterMock.Object);

            command.InitOptions(gitignoreFilenames, destination);


            // Act
            var result = await handler.HandleCommandAsync(command);

            // Assert
            result
               .Should()
               .Be(ReturnCodes.Success);
            
            fileWriterMock.Verify(writer => writer.WriteToFileAsync(destination, expectedContent), Times.Once);
        }


        [Fact]
        public async Task HandleShouldThrowArgExceptionWhenNoFileForNameWasFound()
        {
            // Arrange
            const string gitignoreFilename = "non-existing.gitignore";
            const string destination = "C:/test";

            var githubServiceMock = new Mock<IGitignoreService>();
            githubServiceMock
               .Setup(service => service.GetIgnoreFile(gitignoreFilename))
               .ReturnsAsync(GitignoreFile.Empty)
               .Verifiable();

            var fileWriterMock = new Mock<IGitignoreFileWriter>();

            var (command, handler) = CreateCommandAndHandler(githubServiceMock.Object, fileWriterMock.Object);

            command.InitOptions(gitignoreFilename, destination);

            // Act
            Func<Task> handleCall = () => handler.HandleCommandAsync(command);

            // Assert
            await handleCall
               .Should()
               .ThrowAsync<ArgException>();
            githubServiceMock.Verify(service => service.GetIgnoreFile(gitignoreFilename), Times.Once);
        }

        [Fact]
        public async Task HandleShouldThrowArgExceptionWhenFileWriterFailed()
        {
            // Arrange
            const string gitignoreFilename = "test.gitignore";
            const string gitignoreContent = "test content # ./[*]";
            const string destination = "C::::D/test";

            var githubServiceMock = new Mock<IGitignoreService>();
            githubServiceMock
               .Setup(service => service.GetIgnoreFile(It.IsAny<string>()))
               .ReturnsAsync(new GitignoreFile(gitignoreFilename, gitignoreContent))
               .Verifiable();

            var fileWriterMock = new Mock<IGitignoreFileWriter>();
            fileWriterMock
               .Setup(writer => writer.WriteToFileAsync(destination, It.IsAny<string>()))
               .Throws(new ArgException(string.Empty))
               .Verifiable();

            var (command, handler) = CreateCommandAndHandler(githubServiceMock.Object, fileWriterMock.Object);

            command.InitOptions(gitignoreFilename, destination);

            // Act
            Func<Task> handleCall = () => handler.HandleCommandAsync(command);

            // Assert
            await handleCall
               .Should()
               .ThrowAsync<ArgException>();

            githubServiceMock.Verify(service => service.GetIgnoreFile(gitignoreFilename), Times.Once);
            fileWriterMock.Verify(writer => writer.WriteToFileAsync(destination, It.IsAny<string>()), Times.Once);
        }

        private static (GitignoreGetCommand command, GitignoreGetCommandHandler handler) CreateCommandAndHandler(
            IGitignoreService service,
            IGitignoreFileWriter gitignoreFileWriter)
        {
            var fileNameSplitterStub = new ConcatedNamesProcessorStub();
            var mergeStrategyStub = new MergeStrategyStub();

            var handler = new GitignoreGetCommandHandler(
                service,
                fileNameSplitterStub,
                mergeStrategyStub,
                gitignoreFileWriter);

            var command = new GitignoreGetCommand(handler, fileNameSplitterStub)
            {
            };

            return (command, handler);
        }
    }
}