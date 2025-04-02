using VRISC;

namespace VRISC.Assembler.CLI;

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
        
        var outputPathArg = new Argument<string>("output", "Path to output binary")
        {
            Arity = ArgumentArity.ExactlyOne,
        };
        
        var rootCommand = new RootCommand("Assemble a .s file into an executable binary.")
        {
            filePathArg,
            outputPathArg
        };

        rootCommand.Handler = CommandHandler.Create<string, string>((target, output) =>
        {
            Log.Info($"Assembling {target} to {output}...");
            
            DateTime startTime = DateTime.Now;
            
            byte[] binary = new Assembler(target).Assemble(out var symbols);

            File.WriteAllBytes(output, binary);
            
            TimeSpan duration = startTime - DateTime.Now;
            
            Log.Info($"Assembled in {duration.TotalMilliseconds}ms.");
        });

        await rootCommand.InvokeAsync(args);
    }
}