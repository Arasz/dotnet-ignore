using System;
using CliTool.Arguments;
using FluentAssertions;
using Xunit;

namespace DotnetIgnoreCliToolTests.Commands.Get
{
    public class ConcatedNamesProcessorTests : UnitTestBase
    {
        [Theory]
        [InlineData("a", new[]
        {
            "a"
        })]
        [InlineData("a,b", new[]
        {
            "a", "b"
        })]
        [InlineData("a.ignore,b.ignore", new[]
        {
            "a.ignore", "b.ignore"
        })]
        [InlineData("a,b,  c, d:){_-&^..>", new[]
        {
            "a", "b", "c", "d:){_-&^..>" //trimmed
        })]
        [InlineData("a,b,", new[]
        {
            "a", "b"
        })]
        public void ShouldReturnCollectionOfStrings_WhenSeparatorIsPresent(string names, string[] expectedNames)
        {
            // Arrange
            // Act
            var resultNames = NamesSplitStrategy.Split(names);

            // Assert
            resultNames
                .Should()
                .BeEquivalentTo(expectedNames);
        }

        [Fact]
        public void ShouldThrow_WhenFileNamesIsNull()
        {
            // Arrange
            string nullNames = null;

            // Act
            Action splitCall = () => NamesSplitStrategy.Split(nullNames);

            // Assert
            splitCall
                .Should()
                .Throw<NullReferenceException>();
        }
    }
}