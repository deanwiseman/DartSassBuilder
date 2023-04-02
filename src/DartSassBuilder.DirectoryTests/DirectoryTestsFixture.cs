using CommandLine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DartSassBuilder.DirectoryTests;

// Fixture runs inside DartSassBuilder.DirectoryTests/logs
public class DirectoryTestsFixture : IAsyncLifetime
{
    private readonly DartSassBuilder _dsb;
    public string FileDirectory { get; } = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
    private List<string> _filesForDeletion = new List<string>();
    private bool _isInitialized = false;

    public DirectoryTestsFixture()
    {
        _dsb = new DartSassBuilder(new Parser());
    }

    public async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            await _dsb.ParseArgumentsAsync(new[] { Path.Join(FileDirectory, "logs") });

            _isInitialized = true;
        }
    }

    public void MarkFilesForDeletion(params string[] filePath)
    {
        _filesForDeletion.AddRange(filePath);
    }

    public Task DisposeAsync()
    {
        foreach (var file in _filesForDeletion)
        {
            if (File.Exists(file))
                File.Delete(file);
        }

        return Task.CompletedTask;
    }
}
