namespace DartSassBuilder
{
    public class ConsoleLogger
    {

        public ConsoleLogger(OutputLevel outputLevel = OutputLevel.Default)
        {
            OutputLevel = outputLevel;
        }

        private OutputLevel OutputLevel { get; }

        public void Log(string line = "", OutputLevel level = OutputLevel.Default)
        {
            if (level >= OutputLevel)
            {
                Console.WriteLine(line);
            }
        }
        public void Error(string line) => Log(line, OutputLevel.Error);

        public void Verbose(string line) => Log(line, OutputLevel.Verbose);

    }
}