using CommandLine;
using System;
using System.Threading.Tasks;

namespace DartSassBuilder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cliParser = new Parser(config =>
            {
                config.CaseInsensitiveEnumValues = true;
                config.AutoHelp = true;
                config.HelpWriter = Console.Out;
            });

            var dsb = new DartSassBuilder(cliParser);

            await dsb.ParseArgumentsAsync(args);
        }
    }
}
