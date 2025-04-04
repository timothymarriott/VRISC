using Raylib_cs;
using VRISC.Emulator.GPU;

namespace VRISC.Emulator.CLI;

using System.CommandLine.NamingConventionBinder;
using System.CommandLine;
using System.CommandLine.Invocation;

class Program
{
    static int Main(string[] args)
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
            Emulator emu = new Emulator(File.ReadAllBytes(target));
            
            
            
            Raylib.SetTraceLogLevel(TraceLogLevel.None);
            
            Raylib.InitWindow(800, 800, "VRISC Emulator");
            
            new Thread(emu.Run).Start();

            while (emu.state.Running)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Magenta);
                
                Raylib.SetWindowSize(emu.state.gpu.width, emu.state.gpu.height);

                if (emu.state.gpu.CurrentDisplayMode != DisplayMode.Console)
                {
                    if (emu.state.gpu.blitRequested)
                    {
                        emu.state.gpu.blitRequested = false;
                        emu.state.gpu.Render();
                    }
                    Raylib.DrawTexture(emu.state.gpu.target.Texture, 0, 0, Color.White);
                }
                
                
                
                Raylib.EndDrawing();
            }
            
            Raylib.CloseWindow();
            
            

        });

        return rootCommand.Invoke(args);
    }
}