using System;
using System.IO;

using TestBaseLib;

using Xunit;

namespace DartSassBuilder.Tests
{
	/// <summary>
    /// Test class for running DartSassBuilder within the "test-files" folder.
    /// </summary>
	public class FileTests : TestBase
	{
		private const string targetFolder = "test-files";

		public FileTests() : base(targetFolder)
		{
		}

        [Theory]
        [InlineData("test-new-format.css")]
        [InlineData("test-indented-format.css")]
        public void FileExistsTest(string cssFileName)
        {
            var cssFilePath = Path.Join(TestRoot, targetFolder, cssFileName);

            Assert.True(File.Exists(cssFilePath));

            File.Delete(cssFilePath);
        }

		[Theory]
		[InlineData("", "_ignored.css")]
		[InlineData("bin", "bin-file.css")]
		[InlineData("logs", "logs-file.css")]
		[InlineData("node_modules", "app.css")]
		[InlineData("obj", "obj-file.css")]
		public void ExcludedFilesTest(string subFolder, string cssFileName)
		{
			var cssFilePath = Path.Join(TestRoot, subFolder, cssFileName);

			Assert.False(File.Exists(cssFilePath));
		}
	}
}
