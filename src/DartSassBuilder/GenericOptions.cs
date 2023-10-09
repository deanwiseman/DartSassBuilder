namespace DartSassBuilder
{
    public abstract class GenericOptions
    {
        [Option('l', "level", Required = false, HelpText = "Specify the level of output (Debug, Information, None)")]
        public OutputLevel OutputLevel { get; set; } = OutputLevel.Default;

        public CompilationOptions SassCompilationOptions { get; } = new CompilationOptions()
        {
            OutputStyle = OutputStyle.Compressed
        };

        [Option("outputstyle", Required = false, HelpText = "Specify the style of output (compressed, compact, nested, expanded)")]
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


}
