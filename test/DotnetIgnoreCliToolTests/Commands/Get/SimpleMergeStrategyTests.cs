using System;
using System.Collections.Generic;
using System.Linq;
using CliTool.Github.Models;
using CliTool.Merge;
using FluentAssertions;
using Xunit;

namespace DotnetIgnoreCliToolTests.Commands.Get
{
    public class SimpleMergeStrategyTests : UnitTestBase
    {
        [Fact]
        public void MergeShouldReturnOnlyFileWhenOnlyOneExist()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            var file = new GitignoreFile("name", "content");
            var files = new[]
            {
                file
            };

            // Act
            var result = mergeStrategy.Merge(files);

            // Assert
            result
               .Should()
               .Be(file);
        }
        
        [Fact]
        public void MergeShouldReturnMergedContentFromMultipleFiles()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            var files = new[]
            {
                new GitignoreFile("n1", "testContent\n222e2F#$"),
                new GitignoreFile("n2", "differentContent\n\nfdfdf\nfe")
            };

            // Act
            var result = mergeStrategy.Merge(files);

            // Assert
            result
               .Content
               .Should()
               .ContainAll(files.Select(file => file.Content));
        }
        
        [Fact]
        public void MergeShouldThrowWhenNullCollectionIsGiven()
        {
            // Arrange
            var mergeStrategy = new SimpleMergeStrategy();
            IReadOnlyCollection<GitignoreFile> files = null;

            // Act
            Action mergeAction = () => mergeStrategy.Merge(files);

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
               .OnlyContain(contentLine => contentLine.StartsWith("#"));
        }
    }
}