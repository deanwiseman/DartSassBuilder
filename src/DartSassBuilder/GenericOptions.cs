using System.Collections.Generic;
using CommandLine;
using DartSassHost;

namespace DartSassBuilder
{
    public abstract class GenericOptions
    {
        [Option('l', "level", Required = false, HelpText = "Specify the level of output (silent, default, verbose)")]
        public OutputLevel OutputLevel { get; set; } = OutputLevel.Default;

        [Option('i', "include-paths", Required = false, HelpText = "List of paths that library can look in to attempt to resolve @import declarations. Overrides the default.", Default = new[] { "node_modules" })]
        public IEnumerable<string> IncludePaths { get; set; }

        public CompilationOptions SassCompilationOptions { get; } = new CompilationOptions()
        {
            OutputStyle = OutputStyle.Compressed
        };

        [Option("outputstyle", Required = false, HelpText = "Specify the style of output (compressed, condensed, nested, expanded)")]
        public OutputStyle OutputStyle
        {
            get
            {
                return SassCompilationOptions.OutputStyle;
            }
            set
            {
                SassCompilationOptions.OutputStyle = value;
            }
        }
    }

    public enum OutputLevel
    {
        Silent,
        Default,
        Verbose
    }
}
