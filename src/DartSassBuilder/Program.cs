using CommandLine;
using DartSassHost;
using DartSassHost.Helpers;
using JavaScriptEngineSwitcher.V8;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DartSassBuilder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var parser = new Parser(config =>
            {
                config.CaseInsensitiveEnumValues = true;
                config.AutoHelp = true;
                config.HelpWriter = Console.Out;
            });

            await parser.ParseArguments<DirectoryOptions, FilesOptions>(args)
                .WithNotParsed(e => Environment.Exit(1))
                .WithParsedAsync(async o =>
                {
                    switch (o)
                    {
                        case DirectoryOptions directoryOptions:
                            {
                                var program = new Program(directoryOptions);

                                program.WriteLine($"Sass compile directory: {directoryOptions.Directory}");

                                await program.CompileDirectoriesAsync(directoryOptions.Directory, directoryOptions.ExcludedDirectories, directoryOptions.OutputPath);

                                program.WriteLine("Sass files compiled");
                            }
                            break;
                        case FilesOptions fileOptions:
                            {
                                var program = new Program(fileOptions);

                                program.WriteLine($"Sass compile files");

                                await program.CompileFilesAsync(fileOptions.Files, "output-path");

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

        async Task CompileDirectoriesAsync(string directory, IEnumerable<string> excludedDirectories, string outputPath)
        {
            var sassFiles = Directory.EnumerateFiles(directory)
                .Where(file => file.EndsWith(".scss", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".sass", StringComparison.OrdinalIgnoreCase));

            await CompileFilesAsync(sassFiles, outputPath);

            var subDirectories = Directory.EnumerateDirectories(directory);
            foreach (var subDirectory in subDirectories)
            {
                var directoryName = new DirectoryInfo(subDirectory).Name;
                if (excludedDirectories.Any(dir => string.Equals(dir, directoryName, StringComparison.OrdinalIgnoreCase)))
                    continue;

                await CompileDirectoriesAsync(subDirectory, excludedDirectories, outputPath);
            }
        }

        async Task CompileFilesAsync(IEnumerable<string> sassFiles, string outputPath)
        {
            try
            {
                using var sassCompiler = new SassCompiler(new V8JsEngineFactory());

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

                    // this technically specifies the new output path
                    // i.e: the current path of the file but with the extention renamed to "CSS"
                    // for output path support: we want the exact filename, but with the path as specified in output path

                    if (!string.IsNullOrWhiteSpace(outputPath))
                    {
                        //var path = Path.Combine(Directory.GetCurrentDirectory(), outputPath);
                        if (Directory.Exists(outputPath))
                        {
                            var dirInfo = new DirectoryInfo(outputPath);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    //var newFilePath = fileInfo.FullName.Replace(fileInfo.Extension, ".css");
                    var newFilePath = Path.Combine(new DirectoryInfo(outputPath).FullName, fileInfo.Name.Replace(fileInfo.Extension, ".css"));
                    Debug.Assert(string.IsNullOrEmpty(newFilePath));

                    if (File.Exists(newFilePath) && result.CompiledContent == await File.ReadAllTextAsync(newFilePath))
                        continue;

                    await File.WriteAllTextAsync(newFilePath, result.CompiledContent);
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

        void WriteLine(string line)
        {
            if (Options.OutputLevel >= OutputLevel.Default)
            {
                Console.WriteLine(line);
            }
        }

        void WriteVerbose(string line)
        {
            if (Options.OutputLevel >= OutputLevel.Verbose)
            {
                Console.WriteLine(line);
            }
        }
    }
}
