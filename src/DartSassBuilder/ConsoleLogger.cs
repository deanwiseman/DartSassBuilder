namespace DartSassBuilder
{
    public enum OutputLevel
    {
        Trace,
        Debug,
        Default,
        Warning,
        Error,
        Critical,
        None,
    }

    public class ConsoleLogger
    {
        public ConsoleLogger(OutputLevel outputLevel = OutputLevel.Default)
        {
            OutputLevel = outputLevel;
        }

        private OutputLevel OutputLevel { get; }

        public void Trace(string line = "") => Log(OutputLevel.Trace, line);
        public void Debug(string line = "") => Log(OutputLevel.Debug, line);
        public void Default(string line = "") => Log(OutputLevel.Default, line);
        public void Warning(string line = "") => Log(OutputLevel.Warning, line);
        public void Error(string line = "") => Log(OutputLevel.Error, line);
        public void Critical(string line = "") => Log(OutputLevel.Critical, line);

        private void Log(OutputLevel level, string line = "")
        {
            if (level >= OutputLevel)
            {
                Console.WriteLine($"{level}: {line}");
            }
        }

    }
}