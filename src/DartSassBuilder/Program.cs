using CommandLine;
using DartSassHost;
using DartSassHost.Helpers;
using JavaScriptEngineSwitcher.V8;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DartSassBuilder;

class Program
{
    static async Task Main(string[] args)
    {
        var parser = new Parser(config => {
            config.CaseInsensitiveEnumValues = true;
            config.AutoHelp = true;
            config.HelpWriter = Console.Out;
        });

        await parser.ParseArguments<DirectoryOptions, FilesOptions>(args)
            .WithNotParsed(e => Environment.Exit(1))
            .WithParsedAsync(async o => {
                switch (o)
                {
                    case DirectoryOptions directory:
                        {
                            var program = new Program(directory);

                            program.WriteLine($"Sass compile directory: {directory.Directory}");

                            await program.CompileDirectoriesAsync(directory.Directory, directory.ExcludedDirectories);

                            program.WriteLine("Sass files compiled");
                        }
                        break;
                    case FilesOptions file:
                        {
                            var program = new Program(file);
                            program.WriteLine($"Sass compile files");

                            await program.CompileFilesAsync(file.Files);

                            program.WriteLine("Sass files compiled");
                        }
                        break;
                    default:
                        throw new NotImplementedException("Invalid commandline option parsing");
                }
            });
    }

    public GenericOptions Options { get; }

    public Program(GenericOptions options)
    {
        Options = options;
    }

    private async Task CompileDirectoriesAsync(string directory, IEnumerable<string> excludedDirectories)
    {
        var sassFiles = Directory.EnumerateFiles(directory)
            .Where(file => file.EndsWith(".scss", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".sass", StringComparison.OrdinalIgnoreCase));

        await CompileFilesAsync(sassFiles);

        var subDirectories = Directory.EnumerateDirectories(directory);
        foreach (var subDirectory in subDirectories)
        {
            var directoryName = new DirectoryInfo(subDirectory).Name;
            if (excludedDirectories.Any(dir => string.Equals(dir, directoryName, StringComparison.OrdinalIgnoreCase)))
                continue;

            await CompileDirectoriesAsync(subDirectory, excludedDirectories);
        }
    }

    private async Task CompileFilesAsync(IEnumerable<string> sassFiles)
    {
        try
        {
            using var sassCompiler = new SassCompiler(() => new V8JsEngineFactory().CreateEngine());

            foreach (var file in sassFiles)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Name.StartsWith("_"))
                {
                    WriteVerbose($"Skipping: {fileInfo.FullName}");
                    continue;
                }

                WriteVerbose($"Processing: {fileInfo.FullName}");

                var result = sassCompiler.CompileFile(file, options: Options.SassCompilationOptions);

                var newFile = fileInfo.FullName.Replace(fileInfo.Extension, ".css");

                if (File.Exists(newFile) && result.CompiledContent.ReplaceLineEndings() == (await File.ReadAllTextAsync(newFile)).ReplaceLineEndings())
                    continue;

                await WriteFileAsync(fileInfo, result.CompiledContent, Options.OutputPath);
            }
        }
        catch (SassCompilerLoadException e)
        {
            Console.WriteLine("During loading of Sass compiler an error occurred. See details:");
            Console.WriteLine();
            Console.WriteLine(SassErrorHelpers.GenerateErrorDetails(e));
        }
        catch (SassCompilationException e)
        {
            Console.WriteLine("During compilation of SCSS code an error occurred. See details:");
            Console.WriteLine();
            Console.WriteLine(SassErrorHelpers.GenerateErrorDetails(e));
        }
        catch (SassException e)
        {
            Console.WriteLine("During working of Sass compiler an unknown error occurred. See details:");
            Console.WriteLine();
            Console.WriteLine(SassErrorHelpers.GenerateErrorDetails(e));
        }
    }

    private async Task WriteFileAsync(FileInfo file, string compiledContent, string outputPath = default)
    {
        var filePathToWrite = GetAbsoluteFilePath(file, outputPath);

        await File.WriteAllTextAsync(filePathToWrite, compiledContent);
    }

    private static string GetAbsoluteFilePath(FileInfo file, string outputPath = null)
    {
        // if no output path specified, write the file as is in current directory
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return Path.ChangeExtension(file.FullName, ".css");
        }

        // if an output path is specified, get the absolute file path to the directory to write to
        var absoluteOutputPath = Path.GetFullPath(outputPath);
        return Path.Combine(absoluteOutputPath, Path.ChangeExtension(file.Name, ".css"));
    }

    private void WriteLine(string line)
    {
        if (Options.OutputLevel >= OutputLevel.Default)
        {
            Console.WriteLine(line);
        }
    }

    private void WriteVerbose(string line)
    {
        if (Options.OutputLevel >= OutputLevel.Verbose)
        {
            Console.WriteLine(line);
        }
    }
}
