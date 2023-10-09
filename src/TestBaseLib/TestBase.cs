using Xunit;

namespace TestBaseLib;

/// <summary>
/// Base class for async lifetime tests.
/// This class is responsible for running the DartSassBuilder program with the given command line arguments.
/// </summary>
public abstract class TestBase : IAsyncLifetime
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestBase"/> class.
    /// </summary>
    /// <remarks>
    /// The <param name="dirParam"> parameter should be the directory option that would be passed on the cmd"
    /// </remarks>
    /// <param name="dirParam">Directory path where the test should be run.</param>
    /// <param name="args">Command line arguments for the test.</param>
    protected TestBase(string? dirParam = null, params string[] args)
    {
        var argsList = args.ToList();

        TestRoot = GetProjectDirectory();

        Args = argsList.Prepend(GetProjectDirectory(dirParam))
                       .Append("-l Debug").ToArray();
    }

    /// <summary>
    /// Gets the root directory of the test project.
    /// </summary>
    protected string TestRoot { get; }

    /// <summary>
    /// Arguments passed by the test that will be forwarded to the DartSassBuilder program.
    /// </summary>
    private string[] Args { get; }

    /// <summary>
    /// Initializes the test before execution.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task InitializeAsync()
    {
        Console.WriteLine("Running Program for tests...");
        var splitArgs = Args.SelectMany(a => a.Split(' '));
        return DartSassBuilder.Program.Main(splitArgs.ToArray());
    }

    /// <summary>
    /// Cleans up the test after its execution.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task DisposeAsync()
    {
        CleanTestFiles();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes all .css files generated in the test directory.
    /// </summary>
    private void CleanTestFiles()
    {
        foreach (var cssFile in Directory.EnumerateFiles(TestRoot,
                                                         "*.css",
                                                         SearchOption.AllDirectories))
        {
            File.Delete(cssFile);
        }
    }

    /// <summary>
    /// Gets the project directory with an optional subdirectory.
    /// </summary>
    /// <param name="subdir">A subdirectory of the project.</param>
    /// <returns>The project directory path.</returns>
    protected static string GetProjectDirectory(string? subdir = null)
    {
        var dir = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName
        ?? Environment.CurrentDirectory;

        if (subdir is not null)
        {
            dir = Path.Combine(dir, subdir);
        }

        return dir;
    }
}
