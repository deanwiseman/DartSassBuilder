using System.IO;

using TestBaseLib;

using Xunit;

namespace DartSassBuilder.DirectoryTests
{
    /// <summary>
    /// Test class for running DartSassBuilder within the "logs" folder.
    /// </summary>
    public class DirectoryTests : TestBase
    {
        private const string targetFolder = "./logs";
        public DirectoryTests(): base(targetFolder)
        {
        }

        [Fact]
        public void ExplicitDirectoryTest()
        {
            var barFile = Path.Join(TestRoot, "foo/bar.css");
            var binFile = Path.Join(TestRoot, "logs/bin/bin-file.css");
            var logsFile = Path.Join(TestRoot, "logs/logs-file.css");

            Assert.False(File.Exists(barFile)); // not in ./logs directory
            Assert.False(File.Exists(binFile)); // inside ./logs, but excluded by default nested bin folder
            Assert.True(File.Exists(logsFile)); // inside ./logs

            File.Delete(logsFile);
        }

        [Fact]
        public void IncludedDirectoryTest()
        {
            var dialogsFile = Path.Join(TestRoot, "logs/dialogs/dialog-file.css");

            Assert.True(File.Exists(dialogsFile)); // inside ./logs/dialogs directory, but included as it doesn't explicitly match "logs"

            File.Delete(dialogsFile);
        }
    }
}
