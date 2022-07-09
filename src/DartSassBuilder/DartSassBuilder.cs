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
    public class DartSassBuilder
    {
        private readonly Parser _parser;

        private GenericOptions _options;

        public DartSassBuilder(Parser parser)
        {
            _parser = parser;
        }

        public async Task ParseArgumentsAsync(string[] args)
        {
            await _parser
               .ParseArguments<DirectoryOptions, FilesOptions>(args)
               .WithNotParsed(e => Environment.Exit(1))
               .WithParsedAsync(async options =>
               {
                   switch (options)
                   {
                       case DirectoryOptions directoryOptions:
                           {
                               _options = directoryOptions;

                               WriteLine($"Sass compile directory: {directoryOptions.Directory}");

                               await CompileDirectoriesAsync(directoryOptions.Directory, directoryOptions.ExcludedDirectories);

                               WriteLine("Sass files compiled");
                           }
                           break;
                       case FilesOptions fileOptions:
                           {
                               _options = fileOptions;

                               WriteLine($"Sass compile files");

                               await CompileFilesAsync(fileOptions.Files);

                               WriteLine("Sass files compiled");
                           }
                           break;
                       default:
                           throw new NotImplementedException("Invalid commandline option parsing");
                   }
               });
        }

        public async Task CompileDirectoriesAsync(string directory, IEnumerable<string> excludedDirectories)
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

        public async Task CompileFilesAsync(IEnumerable<string> sassFiles)
        {
            try
            {
                using var sassCompiler = new SassCompiler();

                foreach (var file in sassFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Name.StartsWith("_"))
                    {
                        WriteVerbose($"Skipping: {fileInfo.FullName}");
                        continue;
                    }

                    WriteVerbose($"Processing: {fileInfo.FullName}");

                    var result = sassCompiler.CompileFile(file, options: _options.SassCompilationOptions);

                    var newFile = fileInfo.FullName.Replace(fileInfo.Extension, ".css");

                    if (File.Exists(newFile) && result.CompiledContent == await File.ReadAllTextAsync(newFile))
                        continue;

                    await File.WriteAllTextAsync(newFile, result.CompiledContent);
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

        public void WriteLine(string line)
        {
            if (_options.OutputLevel >= OutputLevel.Default)
            {
                Console.WriteLine(line);
            }
        }

        public void WriteVerbose(string line)
        {
            if (_options.OutputLevel >= OutputLevel.Verbose)
            {
                Console.WriteLine(line);
            }
        }
    }
}
