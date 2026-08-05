using System.IO;
using System.Threading.Tasks;
using CliTool.Files;
using FluentAssertions;
using Xunit;

namespace DotnetIgnoreCliToolTests.Files;

public class GitignoreFileWriterTests
{
    [Fact]
    public async Task ShouldWriteGitignoreFileWithContent_ToGivenDestination()
    {
        // Arrange
        const string expectedContent = "some gitignore content";
        var writer = new GitignoreFileWriter();
        var tempDirectory = Directory.CreateTempSubdirectory();

        try
        {
            // Act
            await writer.WriteToFileAsync(tempDirectory.FullName, expectedContent);

            // Assert
            var writtenContent = await File.ReadAllTextAsync(
                Path.Combine(tempDirectory.FullName, ".gitignore"),
                TestContext.Current.CancellationToken);
            writtenContent.Should().Be(expectedContent);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }
}
