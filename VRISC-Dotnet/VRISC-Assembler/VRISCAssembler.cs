namespace VRISC.Assembler;


using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

[System.Serializable]
public class DebugSymbols
{
    public Dictionary<int, string> Labels = new Dictionary<int, string>();
    public Dictionary<string, int> Vars = new Dictionary<string, int>();
}

public class Assembler
{
    private string input = "";
    private List<Tuple<string, object[]>> program = new List<Tuple<string, object[]>>();

    private List<Tuple<object, object>> data = new();
    public Dictionary<string, object> vars = new();

    private List<byte> output = new List<byte>();
    private bool compiled = false;

    public const int PROGRAM_LENGTH = 1024*4;


    public Assembler(string input){
        this.input = input;
    }

    public byte[] Assemble(out DebugSymbols symbols)
    {
       
        
        DateTime startTime = DateTime.Now;
        
        string[] lines = input.Split("\n");
        
        int linenumber = 0;

        foreach (var l in lines)
        {
            string line = l.ToUpper().Split(";")[0];
            line = line.Replace("\n", "");
            line = line.Replace("\r", "");
            line = line.Trim();
            if (line.Length > 0)
            {
                parseLine(line, linenumber);
            }

            linenumber++;
        }
        
        parseUnmarkedLabels();
        parseProgram();
        
        while (output.Count < PROGRAM_LENGTH)
        {
            output.Add(0); // Add 0 to pad the list
        }
        
        symbols = new();
        foreach (var l in getLabels())
        {
            symbols.Labels.Add(l.Item2, l.Item1);
        }

        foreach (var var in vars)
        {
            symbols.Vars.Add(var.Key, (int)parseNumber(var.Value.ToString(), -999));
        }
        
        File.WriteAllBytes("program.bin", output.ToArray());
        File.WriteAllText("program.bin_dbg", JsonSerializer.Serialize(symbols, new JsonSerializerOptions() {WriteIndented = true, IncludeFields = true}));
        
        
        return output.ToArray();
    }

    private bool labelOpen;
    private string labelToUse;

    public List<Tuple<string, int>> getLabels()
    {
        int location = 0;
        List<Tuple<string, int>> labels = new();
        foreach (var operation in program)
        {
            if (operation.Item2 != null)
            {
                if (operation.Item1.Length > 0)
                    labels.Add(new Tuple<string, int>(operation.Item1, location));

                if (operation.Item2[0] is int i && (i == 0 || i == 1))
                {
                    location = location + 5;
                }
                else
                {
                    location = location + operation.Item2.Length;
                }
            }
        }
        return labels;
    }

    private void parseLine(string line, int linenumber)
    {
        var labelLine = line.Split(":");
        var label = labelLine[0];
        if (line.Contains(":") && labelLine[1].Length <= 1)
        {
            if (labelOpen)
            {
                labelError(linenumber);
            }

            labelToUse = label;
            labelOpen = true;
            return;
        }
        else if (!line.Contains(":"))
        {
            if (labelOpen)
            {
                label = labelToUse;
                labelOpen = false;
            }
            else
            {
                label = "";
            }
        }
        else
        {
            if (labelOpen)
            {
                labelError(linenumber);
            }

            line = labelLine[1].Trim();
        }

        string[] lineparts = line.Split(" ");
        object[] operations = encode(lineparts, linenumber);
        
        program.Add(new Tuple<string, object[]>(label, operations));
        
    }

    object[] encode(string[] line, int linenumber)
    {
        string opcode = line[0];
        object operand0 = 0;
        object operand1 = 0;
        bool twoByte = false;

        switch (opcode)
        {
            case "BGE":
                if (line.Length != 2)
                {
                    syntaxError(linenumber);
                }

                operand0 = 0;

                twoByte = true;
                operand1 = parseNumber(line[1], linenumber);
                break;
            case "LDI":
                if (line.Length != 2)
                {
                    syntaxError(linenumber);
                }

                twoByte = true;
                operand0 = 1;
                operand1 = parseNumber(line[1], linenumber);

                break;
            case "CMP":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                twoByte = false;
                operand0 = 2;
                break;
            case "ADD":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 3;
                break;
            case "PUSH":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 4;
                break;
            case "POP":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 5;
                break;
            case "LDR":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 6;
                break;
            case "PG":
                if (line.Length != 2)
                {
                    syntaxError(linenumber);
                }

                return encode(new[] { "STR", "$00", line[1] }, linenumber);
                break;
            case "STR":
                if (line.Length != 1 && line.Length != 3)
                {
                    syntaxError(linenumber);
                }

                operand0 = 7;
                if (line.Length == 3)
                {
                    List<object> res = new();
                    parseLine("LDI " + line[2], -1);
                    /*
                    object[] items = encode(new[] { "LDI", line[2] }, -1);
                    res.Add(items[0]);
                    foreach (var b in BitConverter.GetBytes((int)items[1]))
                    {
                        res.Add(b);
                    }
                    
                    */
                    
                    parseLine("PUSH", -1);
                    parseLine("LDI " + line[1], -1);
                    
                    
                    
                }

                break;
            case "CLC":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 8;
                break;
            case "SEC":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 9;
                break;
            case "RET":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 10;
                break;
            case "HLT":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 11;
                break;
            case "GRT":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 12;
                break;
            case "SUB":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 13;
                break;
            case "MUL":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 14;
                break;
            case "DIV":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 15;
                break;
            case "SYS":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 16;
                break;
            case "FLP":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 17;
                break;
            case "LES":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 18;
                break;
            case "CPOP":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 19;
                break;
            case "LSHFT":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 20;
                break;
            case "RSHFT":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 21;
                break;
            case "IRET":
                if (line.Length != 1)
                {
                    syntaxError(linenumber);
                }

                operand0 = 22;
                break;
            default:
                if (line.Length == 3 && line[1] == "=")
                {
                    if (program.Count > 0)
                    {
                        //varSequenceError(linenumber);
                    }

                    data.Add(new Tuple<object, object>(line[0], line[2]));
                    return null;
                }
                else
                {
                    unrecognizedError(linenumber);
                }

                break;


        }
        
        if (twoByte)
        {
            return new object[]{operand0, operand1};
        }
        else
        {
            return new object[]{operand0};
        }
        
        
    }




    string getVarValue(string varName)
    {
        foreach (var tuple in data)
        {
            if (tuple.Item1.ToString() == varName)
            {
                if (!vars.ContainsKey(varName))
                {
                    vars.Add(varName, tuple.Item2);
                }
                return ((string)tuple.Item2);
            }
        }

        return "-1";
    }
    
    void labelError(int lineNumber)
    {
        Log.Throw("Unreferenced label detected before line " + lineNumber);
    }
    void syntaxError(int lineNumber)
    {
        Log.Throw("Syntax error, line: " + lineNumber);
    }

    void integerError(int lineNumber)
    {
        Log.Throw("Integer outside expected range, line: " + lineNumber);
    }

    void unrecognizedError(int lineNumber)
    {
        Log.Throw("Unrecognized Instruction, line: " + lineNumber);
    }

    void varSequenceError(int lineNumber)
    {
        Log.Throw("Variables Must be defined before program code, line: " + lineNumber);
    }

    void referenceNotfoundError(string labelName)
    {
        Log.Throw("Reference not found, label: " + labelName);
    }

    private void parseUnmarkedLabels()
    {
        int pLine = 0;
        int instCount = 0;
        foreach (var operations in program)
        {
            int valLine = 0;
            if (operations.Item2 != null)
            {
                foreach (var val in operations.Item2)
                {
                    if (val is string)
                    {
                        if (((string)val).Contains("LABEL"))
                        {
                            program[pLine].Item2[valLine] = parseNumber(((string)val).Substring(6), -instCount);
                        }

                    }
                    valLine++;
                }
                
                instCount = instCount + operations.Item2.Length;
            }
                

            
            pLine++;
        }
    }

    private object parseNumber(string numberString, int lineNumber, bool isOrgIsnt = false)
    {
        char prefix = numberString[0];
        numberString = numberString.Substring(1);
        int result = 0;
        string varVal = getVarValue(prefix + numberString);
        if (prefix == '$' || prefix == '@')
        {
            result = int.Parse(numberString, NumberStyles.HexNumber);
            if ((result >= PROGRAM_LENGTH && isOrgIsnt))
            {
                integerError(lineNumber);
            }
        } else if (prefix == '#')
        {
            
            result = int.Parse(numberString);
            if ((result >= PROGRAM_LENGTH && isOrgIsnt))
            {
                integerError(lineNumber);
            }
        } else if (varVal != "-1")
        {
            return parseNumber(varVal, lineNumber);
        }
        else
        {
            result = getDistanceToLabel(prefix + numberString, -lineNumber);
            if (result == int.MinValue && lineNumber <= -1)
            {
                referenceNotfoundError(prefix + numberString);
            } else if (result == int.MinValue)
            {
                return "LABEL-" + (prefix + numberString);
            }
        }

        if (result < 0)
        {
            integerError(lineNumber);
        }
        
        return result;
    }

    private void parseProgram()
    {
        foreach (var operations in program)
        {
            if (operations.Item2 != null)
            {
                if (operations.Item2[0] is int i ? (i == 0 || i == 1) : false)
                {
                    
                    output.Add((byte)i);
                    int num = (int)operations.Item2[1];
                    output.Add(BitConverter.GetBytes(num)[0]);
                    output.Add(BitConverter.GetBytes(num)[1]);
                    output.Add(BitConverter.GetBytes(num)[2]);
                    output.Add(BitConverter.GetBytes(num)[3]);
                    //Log.Info(i.ToString());
                }
                else
                {
                    foreach (var value in operations.Item2)
                    {
                        if (value is int)
                        {
                            if ((int)value > 255) output.AddRange(BitConverter.GetBytes((int)value));
                            else output.Add((byte)(int)value);
                        }
                    
                    }
                }
                
                
                
                
                
            }
                
        }

        if (output.Count > PROGRAM_LENGTH)
        {
            Log.Throw("Program Is too large");
        }


        compiled = true;
    }

    int getDistanceToLabel(string labelName, int originOperation)
    {
        if (originOperation < 0)
        {
            return int.MinValue;
        }

        int location = 0;
        foreach (var operation in program)
        {
            if (labelName == operation.Item1)
            {
                return location;
            }

            if (operation.Item2 != null)
            {
                if (operation.Item2[0] is int i && (i == 0 || i == 1))
                {
                    location = location + 5;
                }
                else
                {
                    location = location + operation.Item2.Length;
                }
            }
            
        }

        return int.MinValue;

    }
    
    public byte[] PyAssemble(string inputFilePath, string outputFilePath){
        if (File.Exists(outputFilePath))
            File.Delete(outputFilePath);
        File.WriteAllBytes(outputFilePath, new byte[PROGRAM_LENGTH]);
        ProcessStartInfo start = new ProcessStartInfo();
        if (System.OperatingSystem.IsWindows())
        {
            start.FileName = "py";
        }
        else
        {
            start.FileName = "python3";
        }
        
        start.Arguments = "./assemble.py " + inputFilePath + " " + outputFilePath; // specify script path
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.WorkingDirectory = ".";

        using (Process process = Process.Start(start))
        {
            process.WaitForExit();
            using (System.IO.StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                //Console.WriteLine(result);
            }

            if (process.ExitCode != 0)
            {
                throw new Exception($"Python process failed with exit code {process.ExitCode}, message: {process.StandardError.ReadToEnd()}");
            }
        }

        return File.ReadAllBytes(outputFilePath);
    }

}
