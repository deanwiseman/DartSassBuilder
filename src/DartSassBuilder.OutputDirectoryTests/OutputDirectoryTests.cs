using System.IO;
using Xunit;

namespace DartSassBuilder.OutputDirectoryTests
{
    public class OutputDirectoryTests
    {
		private readonly string _fileDirectory;

		public OutputDirectoryTests()
		{
			_fileDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
		}

		[Fact]
		public void FileExistsTest()
		{
			var fooFile = Path.Join(_fileDirectory, "output-folder/test-indented-format.css");

			Assert.True(File.Exists(fooFile));

			File.Delete(fooFile);
		}
	}
}