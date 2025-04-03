namespace VRISC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Log
{

    public static string debugInfo = "";
    
    public static void Info(string value)
    {
        ConsoleColor ogForeground = Console.ForegroundColor;
        ConsoleColor ogBackground = Console.BackgroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"[INFO] [{DateTime.Now.ToShortTimeString()}] [{debugInfo}] ");
        Console.ForegroundColor = ogForeground;
        Console.WriteLine($"{value}");
        Console.ForegroundColor = ogForeground;
        Console.BackgroundColor = ogBackground;
    }
    
    public static void Execution(string value)
    {
        ConsoleColor ogForeground = Console.ForegroundColor;
        ConsoleColor ogBackground = Console.BackgroundColor;
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write($"[EXEC] [{DateTime.Now.ToShortTimeString()}] ");
        Console.ForegroundColor = ogForeground;
        Console.WriteLine($"{value}");
        Console.ForegroundColor = ogForeground;
        Console.BackgroundColor = ogBackground;
    }

    public static void Warning(string value)
    {
        ConsoleColor ogForeground = Console.ForegroundColor;
        ConsoleColor ogBackground = Console.BackgroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARNING] [{DateTime.Now.ToShortTimeString()}] [{debugInfo}] {value}");
        Console.ForegroundColor = ogForeground;
        Console.BackgroundColor = ogBackground;
    }

    public static void Error(string value)
    {
        ConsoleColor ogForeground = Console.ForegroundColor;
        ConsoleColor ogBackground = Console.BackgroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] [{DateTime.Now.ToShortTimeString()}] [{debugInfo}] {value}");
        Console.ForegroundColor = ogForeground;
        Console.BackgroundColor = ogBackground;
    }

    public static void Fatal(string value)
    {
        
        
        ConsoleColor ogForeground = Console.ForegroundColor;
        ConsoleColor ogBackground = Console.BackgroundColor;
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine($"[FATAL] [{DateTime.Now.ToShortTimeString()}] [{debugInfo}] {value}");
        Console.ForegroundColor = ogForeground;
        Console.BackgroundColor = ogBackground;
        Console.ReadKey();
        System.Environment.Exit(1);
        

        

    }
    
    public static void Throw(string value)
    {
        throw new Exception($"[ERROR] [{DateTime.Now.ToShortTimeString()}] [{debugInfo}] {value}");
        
        

        

    }

}

public class Error
{


    public static int currLine;
    public static string currFileName;

    public static Error MissingEndLine() => new("Line not terminated with semicolon.");

    public static Error Unknown() => new("An unknown error occurred.");

    public static Error Expected(string expected, string after, string got)
    {
        return new($"Expected {expected} after {after}, got \"{got}\".");
    }

    public static Error InvalidContextFor(string keyword)
    {
        return new($"Invalid context for {keyword}.");
    }

    public string msg;

    public Error(string msg)
    {
        this.msg = msg;
        Log.Fatal(msg + $" On line {currLine} in {currFileName}.");
    }

}
