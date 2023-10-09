using System.IO;

using TestBaseLib;

using Xunit;

namespace DartSassBuilder.ExcludeTests
{
	// This project is configured to run DartSassBuilder in DartSassBuilder.DirectoryTests.csproj excluding foo & bar directories
	public class ExcludeTests : TestBase
	{
		public ExcludeTests(): base(null, "-e foo bar")
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

			File.Delete(testFile);
		}
	}
}
