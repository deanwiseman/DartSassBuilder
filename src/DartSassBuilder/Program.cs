namespace DartSassBuilder
{
    public static class Program
    {
        private static Parser Parser => new(config =>
            {
                config.CaseInsensitiveEnumValues = true;
                config.AutoHelp = true;
                config.HelpWriter = Console.Out;
            });

        public static Task Main(string[] args)
        {
            return
                Parser.ParseArguments<DirectoryOptions, FilesOptions>(args)
                      .WithNotParsed(_ => Environment.Exit(1))
                      .WithParsedAsync(async o =>
                      {
                          if (o is not GenericOptions opt)
                          {
                              throw new NotImplementedException("Invalid commandline option parsing");
                          }

                          var compiler = new Compiler();
                          await compiler.Compile(opt);
                      });
        }
    }
}