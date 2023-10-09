using DartSassHost.Helpers;

using JavaScriptEngineSwitcher.V8;

namespace DartSassBuilder
{
    class Program
    {
        static Task Main(string[] args)
        {
            return Parser.ParseArguments<DirectoryOptions, FilesOptions>(args)
                .WithNotParsed(e => Environment.Exit(1))
                .WithParsedAsync(async o =>
                {
                    if (o is not GenericOptions opt)
                    {
                        throw new NotImplementedException("Invalid commandline option parsing");
                    }

                    var compiler = new Compiler(opt);
                    var logger = new ConsoleLogger(opt.OutputLevel);

                    switch (o)
                    {
                        case DirectoryOptions directory:
                        {
                            logger.Log($"Sass compile directory: {directory.Directory}");

                            await compiler.CompileDirectoriesAsync(directory.Directory, directory.ExcludedDirectories);

                            logger.Log("Sass files compiled");
                        }
                        break;
                        case FilesOptions file:
                        {
                            logger.Log($"Sass compile files");

                            await compiler.CompileFilesAsync(file.Files);

                            logger.Log("Sass files compiled");
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

        private static Parser Parser => new(config =>
            {
                config.CaseInsensitiveEnumValues = true;
                config.AutoHelp = true;
                config.HelpWriter = Console.Out;
            });
    }


    public class Compiler
    {
        public Compiler(GenericOptions options, ConsoleLogger? logger = null)
        {
            Options = options;
            Logger = logger ?? new ConsoleLogger(options.OutputLevel);
        }

        public GenericOptions Options { get; }
        private ConsoleLogger Logger { get; }

        public async Task CompileFilesAsync(IEnumerable<string> sassFiles)
        {
            try
            {
                using var sassCompiler = new SassCompiler(() => new V8JsEngineFactory().CreateEngine());

                foreach (var file in sassFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Name.StartsWith('_'))
                    {
                        Logger.Verbose($"Skipping: {fileInfo.FullName}");
                        continue;
                    }

                    Logger.Verbose($"Processing: {fileInfo.FullName}");

                    var result = sassCompiler.CompileFile(file, options: Options.SassCompilationOptions);

                    var newFile = fileInfo.FullName.Replace(fileInfo.Extension, ".css");

                    if (File.Exists(newFile) && result.CompiledContent.ReplaceLineEndings() == (await File.ReadAllTextAsync(newFile)).ReplaceLineEndings())
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



    }

    public class ConsoleLogger
    {

        public ConsoleLogger(OutputLevel outputLevel = OutputLevel.Default)
        {
            OutputLevel = outputLevel;
        }

        private OutputLevel OutputLevel { get; }

        public void Log(string line, OutputLevel level = OutputLevel.Default)
        {
            if (level >= OutputLevel)
            {
                Console.WriteLine(line);
            }
        }

        public void Verbose(string line)
        {
            Log(line, OutputLevel.Verbose);
        }
    }
}