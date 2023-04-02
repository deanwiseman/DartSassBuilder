using System.IO;
using Xunit;

namespace DartSassBuilder.ExcludeTests;

// This project is configured to run DartSassBuilder in DartSassBuilder.DirectoryTests.csproj excluding foo & bar directories
public class ExcludeTests : IClassFixture<ExcludeTestsFixture>
{
        private readonly ExcludeTestsFixture _fixture;

        public ExcludeTests(ExcludeTestsFixture fixture)
	{
            _fixture = fixture;
        }

	[Fact]
	public void ExcludeFooFilesTest()
	{
		var fooFile = Path.Join(_fixture.FileDirectory, "foo/foo.css");
		var barFile = Path.Join(_fixture.FileDirectory, "bar/bar.css");
		var testFile = Path.Join(_fixture.FileDirectory, "test.css");

		Assert.False(File.Exists(fooFile)); // excluded foo
		Assert.False(File.Exists(barFile)); // excluded bar
		Assert.True(File.Exists(testFile));

		_fixture.MarkFilesForDeletion(testFile);
	}
}
