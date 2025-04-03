namespace VRISC.Emulator.CLI;

using System.CommandLine.NamingConventionBinder;
using System.CommandLine;
using System.CommandLine.Invocation;

class Program
{
    static async Task Main(string[] args)
    {
        
        var filePathArg = new Argument<string>("target", "Path to target file")
        {
            Arity = ArgumentArity.ExactlyOne
        };
        
        
        var rootCommand = new RootCommand("Execute an SRISC binary.")
        {
            filePathArg
        };

        rootCommand.Handler = CommandHandler.Create<string>((target) =>
        {
            new Emulator(File.ReadAllBytes(target)).Run();
        });

        await rootCommand.InvokeAsync(args);
    }
}