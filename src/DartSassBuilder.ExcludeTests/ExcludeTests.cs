using System.IO;

using TestBaseLib;

using Xunit;

namespace DartSassBuilder.ExcludeTests
{

    /// <summary>
    /// Test class for running DartSassBuilder with the exclude option for folders "foo" and "bar".
    /// </summary>
    public class ExcludeTests : TestBase
    {
        public ExcludeTests() : base(null, "-e foo bar")
        {
        }

        [Fact]
        public void ExcludeFooFilesTest()
        {
            var fooFile = Path.Join(TestRoot, "foo/foo.css");
            var barFile = Path.Join(TestRoot, "bar/bar.css");
            var testFile = Path.Join(TestRoot, "test.css");

            Assert.False(File.Exists(fooFile)); // excluded foo
            Assert.False(File.Exists(barFile)); // excluded bar

            Assert.True(File.Exists(testFile));
        }
    }
}
