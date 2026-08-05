using System;
using System.Collections.Generic;
using System.Linq;
using CliTool.Github.Models;
using CliTool.Merge;
using FluentAssertions;
using Xunit;

namespace DotnetIgnoreCliToolTests.Commands.Get
{
    public class SimpleMergeStrategyTests
    {
        [Fact]
        public void ShouldContainCreatedFileMessage_WhenOnlyOneFileExists()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            var file = new GitignoreFile("name", $"Some random content {Guid.NewGuid()}");
            var files = new[]
            {
                file
            };

            // Act
            var gitignoreFile = mergeStrategy.Merge(files);

            // Assert
            gitignoreFile.Should().NotBeNull();
            gitignoreFile.Name.Should().Be(SimpleMergeStrategy.MergedFileName);
            gitignoreFile.Content.Should().StartWith(SimpleMergeStrategy.CreatedFileMessage).And.Contain(file.Content);
        }

        [Fact]
        public void MergeShouldReturnMergedContentFromMultipleFiles()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            var files = new[]
            {
                new GitignoreFile("n1", "testContent\n222e2F#$"), new GitignoreFile("n2", "differentContent\n\nfdfdf\nfe")
            };

            // Act
            var gitignoreFile = mergeStrategy.Merge(files);

            // Assert
            gitignoreFile.Should().NotBeNull();
            gitignoreFile.Name.Should().Be(SimpleMergeStrategy.MergedFileName);
            gitignoreFile
                .Content
                .Should()
                .StartWith(SimpleMergeStrategy.CreatedFileMessage)
                .And
                .ContainAll(files.Select(file => file.Content));
        }

        [Fact]
        public void MergeShouldThrowWhenNullCollectionIsGiven()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            IReadOnlyCollection<GitignoreFile>? files = null;

            // Act
            Action mergeAction = () => mergeStrategy.Merge(files!);

            // Assert
            mergeAction
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void MergeShouldReturnFileWithOnlyCommentsWhenNoGitignoreFileWasPresent()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            var files = ArraySegment<GitignoreFile>.Empty;

            // Act
            var result = mergeStrategy.Merge(files);

            // Assert
            result
                .Content
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Should()
                .OnlyContain(contentLine => contentLine.StartsWith('#'));
        }

        [Fact]
        public void HeaderShouldContainDeterministicUtcTimestamp()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            var files = new[] { new GitignoreFile("n1", "content") };

            // Act
            var result = mergeStrategy.Merge(files);

            // Assert
            result
                .Content
                .Split(Environment.NewLine)[0]
                .Should()
                .MatchRegex(@"^# Git ignore file created by dotnet-ignore on \d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$");
        }

        [Fact]
        public void MinimizedMergeShouldRemoveComments()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            var files = new[] { new GitignoreFile("n1", "# a comment\nreal-rule") };

            // Act
            var result = mergeStrategy.Merge(files, minimizeFileSize: true);

            // Assert
            result.Content.Should().NotContain("# a comment").And.Contain("real-rule");
        }

        [Fact]
        public void MinimizedMergeShouldRemoveEmptyLines()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            var files = new[] { new GitignoreFile("n1", "\n\nrule1\n\n\nrule2\n") };

            // Act
            var result = mergeStrategy.Merge(files, minimizeFileSize: true);

            // Assert
            result.Content.Should().Contain("rule1").And.Contain("rule2").And.NotContain("\n\n");
        }

        [Fact]
        public void MinimizedMergeShouldRemoveDuplicateLines()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            var files = new[] { new GitignoreFile("n1", "rule1\nrule2\nrule1") };

            // Act
            var result = mergeStrategy.Merge(files, minimizeFileSize: true);

            // Assert
            result
                .Content
                .Split(Environment.NewLine)
                .Where(line => line == "rule1")
                .Should()
                .ContainSingle();
        }

        [Fact]
        public void MinimizedMergeShouldAnnounceWhatWasRemoved()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            var files = new[] { new GitignoreFile("n1", "rule1") };

            // Act
            var result = mergeStrategy.Merge(files, minimizeFileSize: true);

            // Assert
            result
                .Content
                .Should()
                .Contain("# Minimized file size. Comments, duplicated lines and empty lines were removed.");
        }
    }
}