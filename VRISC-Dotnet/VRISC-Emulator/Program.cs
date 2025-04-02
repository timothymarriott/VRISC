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
            new Option<bool>("--debug", "Should it display the debugger."),
            filePathArg
        };

        rootCommand.Handler = CommandHandler.Create<bool, string>((debug, target) =>
        {
            
        });

        await rootCommand.InvokeAsync(args);
    }
}