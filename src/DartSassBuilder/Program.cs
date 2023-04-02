using CommandLine;
using System;
using System.Threading.Tasks;

namespace DartSassBuilder;

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

        using var dartSassBuilder = new DartSassBuilder(cliParser);

        await dartSassBuilder.ParseArgumentsAsync(args);
    }
}
