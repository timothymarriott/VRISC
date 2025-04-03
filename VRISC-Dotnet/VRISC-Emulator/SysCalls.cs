namespace VRISC.Emulator;

public static class SysCalls
{
    public delegate void SysCall(EmulatorState state);

    public static Dictionary<int, SysCall> SystemCalls = new Dictionary<int, SysCall>();

    public static void Create(int index, SysCall call)
    {
        SystemCalls.Add(index, call);
    }

    public static void Create()
    {
        Create(0, (state) =>
        {
            int code = state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte();
                        
            Log.Info("Application exited with code: " + code.ToString());
            state.Running = false;
        });
        Create(1, (state) =>
        {
            int str_pointer = state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte();
            string _s = "";

            for (int l = 0; l < state.memory[str_pointer]; l++)
            {
                _s += (char)state.memory[str_pointer + 1 + l];
            }
                        
            Log.Info("APPLICATION: " + _s);
        });
        Create(2, (state) =>
        {
            Log.Info("ASSERT: 0x" + (state.isLargeMode ? state.stack.PopInt() : state.stack.PopByte()).ToString("x2"));

        });
        
        Create(3, (state) =>
        {
            state.BlitRequested = true;
        });
        
        Create(4, (state) =>
        {
            state.HasInitialized = true;
        });
        
        Create(5, (state) =>
        {
            state.isLargeMode = !state.isLargeMode;
        });
        
        Create(6, (state) =>
        {
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
        });
        
        Create(7, (state) =>
        {
            
        });
    }
}