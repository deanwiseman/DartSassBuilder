using System.IO;
using Xunit;

namespace DartSassBuilder.Tests;

public class FileTests : IClassFixture<FileTestsFixture>
{
    private readonly FileTestsFixture _fixture;

    public FileTests(FileTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData("test-new-format.css")]
    [InlineData("test-indented-format.css")]
    public void FileExistsTest(string cssFileName)
    {
        var cssFilePath = Path.Join(_fixture.FileDirectory, cssFileName);

        Assert.True(File.Exists(cssFilePath));

        _fixture.MarkFilesForDeletion(cssFilePath);
    }

    [Theory]
    [InlineData("", "_ignored.css")]
    [InlineData("bin", "bin-file.css")]
    [InlineData("logs", "logs-file.css")]
    [InlineData("node_modules", "app.css")]
    [InlineData("obj", "obj-file.css")]
    public void ExcludedFilesTest(string subFolder, string cssFileName)
    {
        var cssFilePath = Path.Join(_fixture.FileDirectory, subFolder, cssFileName);

        Assert.False(File.Exists(cssFilePath));
    }
}