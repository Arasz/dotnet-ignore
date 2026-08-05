using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliTool.Commands;
using CliTool.Github.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotnetIgnoreCliToolTests.Commands;

public class ListGitIgnoreFilesCommandTests
{
    private readonly Mock<IGitignoreService> _gitignoreServiceMock = new();

    [Fact]
    public async Task ShouldStripGitignoreSuffix_WhenShortenIsTrue()
    {
        // Arrange
        _gitignoreServiceMock
            .Setup(service => service.GetAllIgnoreFilesNames())
            .ReturnsAsync(new[] { "VisualStudio.gitignore", "Node.gitignore" });
        var command = new ListGitIgnoreFilesCommand(_gitignoreServiceMock.Object);

        // Act
        var output = CaptureConsoleOutput(() => command.ListGitIgnoreFiles(shorten: true));

        // Assert
        output.Should().Be(FormatLines("- VisualStudio", "- Node"));
    }

    [Fact]
    public async Task ShouldKeepGitignoreSuffix_WhenShortenIsFalse()
    {
        // Arrange
        _gitignoreServiceMock
            .Setup(service => service.GetAllIgnoreFilesNames())
            .ReturnsAsync(new[] { "VisualStudio.gitignore", "Node.gitignore" });
        var command = new ListGitIgnoreFilesCommand(_gitignoreServiceMock.Object);

        // Act
        var output = CaptureConsoleOutput(() => command.ListGitIgnoreFiles(shorten: false));

        // Assert
        output.Should().Be(FormatLines("- VisualStudio.gitignore", "- Node.gitignore"));
    }

    private static string FormatLines(params string[] lines) =>
        string.Concat(lines.Select(line => line + Environment.NewLine));

    private static string CaptureConsoleOutput(Action action)
    {
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        try
        {
            action();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        return stringWriter.ToString();
    }
}
