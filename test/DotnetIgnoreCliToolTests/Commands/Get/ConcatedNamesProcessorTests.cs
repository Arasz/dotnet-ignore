using System;
using DotnetIgnoreCliTool.Cli.Commands.Get.Names;
using FluentAssertions;
using Xunit;

namespace DotnetIgnoreCliToolTests.Commands.Get
{
    public class ConcatedNamesProcessorTests : UnitTestBase
    {
        [Theory]
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
        public void ProcessShouldReturnCollectionOfStringsWhenSeparatorIsPresent(string names, string[] expectedNames)
        {
            // Arrange
            var spliter = new ConcatedNamesProcessor();

            // Act
            var resultNames = spliter.Process(names);

            // Assert
            resultNames
               .Should()
               .BeEquivalentTo(expectedNames);
        }

        [Fact]
        public void ProcessShouldThrowWhenFileNamesIsNull()
        {
            // Arrange
            string nullNames = null;
            var spliter = new ConcatedNamesProcessor();

            // Act
            Action splitCall = () => spliter.Process(nullNames);

            // Assert
            splitCall
               .Should()
               .Throw<ArgumentNullException>();
        }
    }
}