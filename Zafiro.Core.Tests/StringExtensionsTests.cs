using FluentAssertions;
using Xunit;

namespace Zafiro.Core.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ExtractFileName()
        {
            var fileName = "This is a filename c:\\windows\\system32\\sample.inf inside a text";
            
            var filename = StringExtensions.ExtractFileName(fileName);
            filename.Should().Be("c:\\windows\\system32\\sample.inf");
        }

        [Fact]
        public void ExtractFileNames()
        {
            var fileName = "dasdasd   c:\\windows\\pepito.txt d:\\windows\\something cool.txt blah blah blah";
            
            var filenames = StringExtensions.ExtractFileNames(fileName);
            filenames.Should().BeEquivalentTo("d:\\windows\\something cool.txt", "c:\\windows\\pepito.txt");
        }
    }
}
