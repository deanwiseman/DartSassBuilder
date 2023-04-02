using System.IO;
using Xunit;

namespace DartSassBuilder.DirectoryTests;

public class DirectoryTests : IClassFixture<DirectoryTestsFixture>
{
    private readonly DirectoryTestsFixture _fixture;

    public DirectoryTests(DirectoryTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ExplicitDirectoryTest()
    {
        var barFile = Path.Join(_fixture.FileDirectory, "foo/bar.css");
        var binFile = Path.Join(_fixture.FileDirectory, "logs/bin/bin-file.css");
        var logsFile = Path.Join(_fixture.FileDirectory, "logs/logs-file.css");

        Assert.False(File.Exists(barFile)); // not in ./logs directory
        Assert.False(File.Exists(binFile)); // inside ./logs, but excluded by default nested bin folder
        Assert.True(File.Exists(logsFile)); // inside ./logs

        _fixture.MarkFilesForDeletion(logsFile);
    }

    [Fact]
    public void IncludedDirectoryTest()
    {
        var dialogsFile = Path.Join(_fixture.FileDirectory, "logs/dialogs/dialog-file.css");

        Assert.True(File.Exists(dialogsFile)); // inside ./logs/dialogs directory, but included as it doesn't explicitly match "logs"

        _fixture.MarkFilesForDeletion(dialogsFile);
    }
}
