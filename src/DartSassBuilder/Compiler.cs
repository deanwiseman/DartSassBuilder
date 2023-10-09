using DartSassHost.Helpers;

using JavaScriptEngineSwitcher.V8;

namespace DartSassBuilder
{
    public class Compiler
    {
        public Compiler(ConsoleLogger? logger = null)
        {
            Logger = logger ?? new ConsoleLogger();
        }

        private ConsoleLogger Logger { get; }

        public async Task Compile(GenericOptions options)
        {
            switch (options)
            {
                case DirectoryOptions directory:
                {
                    Logger.Default(line: $"Sass compile directory: {directory.Directory}");

                    await CompileDirectoriesAsync(directory.Directory,
                                                  directory.ExcludedDirectories,
                                                  options.SassCompilationOptions);

                    Logger.Default(line: "Sass files compiled");
                }
                break;
                case FilesOptions file:
                {
                    Logger.Default(line: $"Sass compile files");

                    await CompileFilesAsync(file.Files, options.SassCompilationOptions);

                    Logger.Default(line: "Sass files compiled");
                }
                break;
                default:
                    throw new NotImplementedException("Invalid commandline option parsing");
            }
        }

        private async Task CompileFilesAsync(IEnumerable<string> sassFiles, CompilationOptions compilationOptions)
        {
            try
            {
                using var sassCompiler = new SassCompiler(() => new V8JsEngineFactory().CreateEngine());

                foreach (var file in sassFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Name.StartsWith('_'))
                    {
                        Logger.Debug($"Skipping: {fileInfo.FullName}");
                        continue;
                    }

                    Logger.Debug($"Processing: {fileInfo.FullName}");

                    var result = sassCompiler.CompileFile(file, options: compilationOptions);

                    var newFile = fileInfo.FullName.Replace(fileInfo.Extension, ".css");

                    if (File.Exists(newFile) && result.CompiledContent.ReplaceLineEndings() == (await File.ReadAllTextAsync(newFile)).ReplaceLineEndings())
                        continue;

                    await File.WriteAllTextAsync(newFile, result.CompiledContent);
                }
            }
            catch (SassCompilerLoadException e)
            {
                Logger.Error(line: "During loading of Sass compiler an error occurred. See details:");
                Logger.Error();
                Logger.Error(line: SassErrorHelpers.GenerateErrorDetails(e));
            }
            catch (SassCompilationException e)
            {
                Logger.Error(line: "During compilation of SCSS code an error occurred. See details:");
                Logger.Error();
                Logger.Error(line: SassErrorHelpers.GenerateErrorDetails(e));
            }
            catch (SassException e)
            {
                Logger.Error(line: "During working of Sass compiler an unknown error occurred. See details:");
                Logger.Error();
                Logger.Error(line: SassErrorHelpers.GenerateErrorDetails(e));
            }
        }

        private async Task CompileDirectoriesAsync(string directory, IEnumerable<string> excludedDirectories, CompilationOptions compilationOptions)
        {
            var sassFiles = Directory.EnumerateFiles(directory)
                .Where(file => file.EndsWith(".scss", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".sass", StringComparison.OrdinalIgnoreCase));

            await CompileFilesAsync(sassFiles, compilationOptions);

            var subDirectories = Directory.EnumerateDirectories(directory);
            foreach (var subDirectory in subDirectories)
            {
                var directoryName = new DirectoryInfo(subDirectory).Name;
                if (excludedDirectories.Any(dir => string.Equals(dir, directoryName, StringComparison.OrdinalIgnoreCase)))
                    continue;

                await CompileDirectoriesAsync(subDirectory, excludedDirectories, compilationOptions);
            }
        }



    }
}