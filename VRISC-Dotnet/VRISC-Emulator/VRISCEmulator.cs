namespace VRISC.Emulator;

public enum Instruction {
    Bge = 0,
    Ldi = 1,
    Cmp = 2,
    Add = 3,
    Push = 4,
    Pop = 5,
    Ldr = 6,
    Str = 7,
    Clc = 8,
    Sec = 9,
    Ret = 10,
    Hlt = 11,
    Grt = 12,
    Sub = 13,
    Mul = 14,
    Div = 15,
    Sys = 16,
    Flp = 17,
    Les = 18,
    Cpop = 19,
    Lshft = 20,
    Rshft = 21,
    IRet = 22,
}

public static class InstructionSet
{
    public static int GetInstructionLength(Instruction instr)
    {
        if (instr == Instruction.Bge || instr == Instruction.Ldi)
        {
            return 5;
        }
        else
        {
            return 1;
        }
    }
}

public class Emulator
{
    public EmulatorState state;

    
    
    public delegate void OnStepCallback();
    public OnStepCallback OnStep;
    
    public delegate void OnStartCallback();
    public OnStartCallback OnStart;
    
    public delegate void OnHaltCallback();
    public OnHaltCallback OnHalt;
    
    
    public Emulator(byte[] executable)
    {
        state = new EmulatorState(this, executable);
        
    }
    
    public void Run()
    {
        state.Running = true;
        Log.Info("Starting Emulator...");
        while (state.Running)
        {
            Step();
        }
        Log.Info("Emulation finished.");
    }

    public void Step()
    {
        Instruction i = (Instruction)state.memory[state.ProgramCounter];
        Execute(i, state.memory.GetInt(state.ProgramCounter + 1), out bool hasJumped);
        if (!hasJumped)
        {
            state.ProgramCounter += InstructionSet.GetInstructionLength(i);
        }
    }

    public void Execute(Instruction instruction, int operand, out bool hasJumped)
    {
        int s;
        switch (instruction)
        {
            case Instruction.Bge:

                if (state.CarryFlag == 1){
                    state.callStack.Push(state.ProgramCounter + 5);
                    
                    hasJumped = true;
                    
                    state.ProgramCounter = operand;
                }

                break;
            case Instruction.Cpop:
                
                state.callStack.Pop();
                break;
            case Instruction.Ldi:
                if (state.isLargeMode)
                {
                    state.LRegister = operand;
                }
                else
                {
                    state.Register = (byte)operand; 
                }
                
                break;
            case Instruction.Cmp:
                state.CarryFlag = (state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte()) == (state.isLargeMode ? state.LRegister : state.Register) ? (byte)0x01 : (byte)0x00;
                break;
            case Instruction.Grt:
                state.CarryFlag = (state.isLargeMode ? state.LRegister : state.Register) > (state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte()) ? (byte)0x01 : (byte)0x00;
                break;
            case Instruction.Les:
                state.CarryFlag = (state.isLargeMode ? state.LRegister : state.Register) < (state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte()) ? (byte)0x01 : (byte)0x00;
                break;
            case Instruction.Add:
                if (state.isLargeMode)
                {
                    s = state.stack.PopInt();

                    
                    state.LRegister = state.LRegister + s + state.CarryFlag;
                    
                }
                else
                {
                    s = state.stack.PopByte();

                    if (state.Register + s + state.CarryFlag > 255){
                        state.Register = 0;
                    } else {
                        state.Register = (byte)(state.Register + s + state.CarryFlag);
                    }
                }
                
                break;
            case Instruction.Sub:
                if (state.isLargeMode)
                {
                    s = state.stack.PopInt();

                    
                    state.LRegister = s - state.LRegister;
                    
                }
                else
                {
                    s = state.stack.PopByte();

                    if (s - state.Register < 0)
                    {
                        state.Register = 0;
                    }
                    else
                    {
                        state.Register = (byte)(s - state.Register);
                    }
                }
                
                break;
            case Instruction.Div:
                if (state.isLargeMode)
                {
                    s = state.stack.PopInt();

                    state.LRegister = s / state.LRegister;
                    
                }
                else
                {
                    s = state.stack.PopByte();

                    if ((int)(s / state.Register) < 0)
                    {
                        state.Register = 0;
                    }
                    else
                    {
                        state.Register = (byte)((int)(s / state.Register));
                    }
                }
                
                break;
            case Instruction.Mul:
                if (state.isLargeMode)
                {
                    s = state.stack.PopInt();

                    state.LRegister = s * state.LRegister * state.CarryFlag;
                    
                }
                else
                {
                    s = state.stack.PopByte();

                    if (s * state.Register * state.CarryFlag < 0)
                    {
                        state.Register = 0;
                    } else if (s * state.Register * state.CarryFlag > 255)
                    {
                        state.Register = 255;
                    }
                    else
                    {
                        state.Register = (byte)(s * state.Register * state.CarryFlag);
                    }
                }
                
                break;
            case Instruction.Push:
                if (state.isLargeMode)
                {
                    state.stack.PushInt(state.LRegister);
                }
                else
                {
                    state.stack.PushByte(state.Register);
                }
                
                break;
            case Instruction.Pop:
                if (state.isLargeMode)
                {
                    state.LRegister = state.stack.PopInt();
                }
                else
                {
                    state.Register = state.stack.PopByte();
                }
                
                break;
            case Instruction.Ldr:

                if (state.stack.Count == 0)
                {
                    Log.Error("Stack is empty for loading");
                }
                else
                {
                    
                    //Console.WriteLine("Reading a " + ram[Stack.Peek()] + " from " + Stack.Peek().ToString());
                    if (state.isLargeMode)
                    {
                        state.LRegister = state.memory.GetInt(state.stack.PopInt());
                    }
                    else
                    {
                        
                        state.Register = state.memory[state.stack.PopByte()];

                    }
                    
                }
                
                
                break;
            case Instruction.Str:
                if (state.isLargeMode)
                {
                    state.memory.SetInt(state.LRegister, state.stack.PopInt());
                }
                else
                {
                    state.memory[state.Register] = state.stack.PopByte();
                }
                
                break;
            case Instruction.Clc:
                state.CarryFlag = 0;
                break;
            case Instruction.Sec:
                state.CarryFlag = 1;
                break;
            case Instruction.Flp:
                if (state.CarryFlag == 0){
                    state.CarryFlag = 1;
                } else {
                    state.CarryFlag = 0;
                }
                break;
            case Instruction.Ret:
                if (state.CarryFlag > 0){
                    if (state.callStack.Count == 0){
                        hasJumped = true;
                        state.ProgramCounter = 0;
                    } else {
                        hasJumped = true;
                        state.ProgramCounter = state.callStack.Pop();
                        
                        //Console.WriteLine("Jumping to " + ProgramCounter.ToString());
                    }

                }
                break;
            case Instruction.Hlt:
                state.Running = false;
                break;
            case Instruction.Sys:
                //Console.WriteLine("Calling system call " + Register.ToString());
                int opcode = state.isLargeMode ? state.LRegister : state.Register;
                switch (opcode) {
                    case 0:
                        int code = state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte();
                        
                        Log.Info("Application exited with code: " + code.ToString());
                        state.Running = false;
                        break;
                    case 1:
                        int str_pointer = state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte();
                        string _s = "";

                        for (int l = 0; l < state.memory[str_pointer]; l++)
                        {
                            _s += (char)state.memory[str_pointer + 1 + l];
                        }
                        
                        Log.Info("APPLICATION: " + _s);
                        break;
                    case 2:
                        
                        //Assertations.Add(state.isLargeMode ? Stack.PeekInt() : Stack.Peek());
                        Log.Info("ASSERT: 0x" + (state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte()).ToString("x2"));
                        break;
                    case 3:
                        state.BlitRequested = true;
                        break;
                    case 4:
                        state.HasInitialized = true;
                        break;
                    case 5:
                        state.isLargeMode = !state.isLargeMode;
                        break;
                    case 6:
                        int path_pointer = state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte();
                        string path = "";

                        for (int l = 0; l < state.memory[path_pointer]; l++)
                        {
                            path += (char)state.memory[path_pointer + 1 + l];
                        }

                        int offset = state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte();
                        int chunkoffset = state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte();
                        int chunksize = state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte();

                        byte[] file_contents = File.ReadAllBytes("data/" + path);
                        Log.Info($"Loading chunk with size {chunksize} from offset {chunkoffset} into file \"{path}\" at offset {offset}");
                        int index = 0;
                        for (int j = chunkoffset; j < chunkoffset + chunksize; j++)
                        {
                            if (j >= file_contents.Length)
                            {
                                Log.Warning("File chunk size overflowed file contents");
                                break;
                            }
                            byte b = file_contents[j];
                            
                            state.memory[offset + index] = b;
                                

                            index++;

                        }

                        if (state.isLargeMode)
                        {
                            state.stack.PushInt(file_contents.Length);
                        }
                        else
                        {
                            state.stack.PushByte((byte)file_contents.Length);
                        }
                        
                        break;
                    
                }

                break;
            case Instruction.Lshft:
                
                if (state.isLargeMode)
                {
                    state.LRegister = state.stack.PopInt() << state.LRegister;
                }
                else
                {
                    state.Register = (byte)((int)state.stack.PopByte() << (int)state.Register);
                }
                break;
            case Instruction.Rshft:
                if (state.isLargeMode)
                {
                    state.LRegister = state.stack.PopInt() >> state.LRegister;
                }
                else
                {
                    state.Register = (byte)((int)state.stack.PopByte() >> (int)state.Register);
                }
                break;
            case Instruction.IRet:
                throw new NotImplementedException();
                break;
            

        }
        hasJumped = false;
    }
    
}


public class EmulatorState
{
    // Hardware
    public Memory memory;
    public Ram ram;
    public VRam vram;
    public SystemPage systemPage;
    public Stack stack;
    public ProgramRom programRom;
    
    public int LRegister;
    public byte Register;

    public byte CarryFlag;

    public int ProgramCounter;
    
    public Stack<int> callStack;

    public bool isLargeMode;
    public bool BlitRequested;
    
    // System
    public bool HasInitialized;

    public bool Running;

    public EmulatorState(Emulator owner, byte[] rom)
    {
        memory = new Memory();
        ram = new Ram();
        vram = new VRam();
        systemPage = new SystemPage(owner);
        stack = new Stack();
        programRom = new ProgramRom(rom);
        memory.regions.Add(ram);
        memory.regions.Add(vram);
        memory.regions.Add(systemPage);
        memory.regions.Add(stack);
        memory.regions.Add(programRom);

        LRegister = 0;
        Register = 0;
        CarryFlag = 0;
        ProgramCounter = 0x04000000;
        
        callStack = new Stack<int>();
        
        isLargeMode = false;
        HasInitialized = false;
        BlitRequested = false;

        Running = false;
    }
    
}