namespace VRISC.Emulator;

public class Memory
{
    public List<MemoryRegion> regions = new List<MemoryRegion>();
    public byte this[int i] {
        get
        {
            int totalSize = 0;
            foreach (var region in regions)
            {
                totalSize += region.GetSize();
                if (i < totalSize)
                {
                    if (region.permisions.HasFlag(MemoryPerms.READ))
                    {
                        return region.GetByte(i - totalSize - region.GetSize());
                    }
                    else
                    {
                        Log.Error("Unauthorized memory read.");
                        return 0;
                    }
                    
                    
                }
            }
            Log.Warning("Out of range memory read.");
            return 0;
        }
        set{
            int totalSize = 0;
            foreach (var region in regions)
            {
                totalSize += region.GetSize();
                if (i < totalSize)
                {
                    if (region.permisions.HasFlag(MemoryPerms.WRITE))
                    {
                        region.SetByte(i - totalSize - region.GetSize(), value);
                        return;
                    }
                    else
                    {
                        Log.Error("Unauthorized memory write.");
                        return;

                    }
                    
                    
                }
            }
            Log.Warning("Out of range memory write.");
            return;

        }
    }

    public int GetInt(int i)
    {
        int totalSize = 0;
        foreach (var region in regions)
        {
            totalSize += region.GetSize();
            if (i < totalSize)
            {
                if (region.permisions.HasFlag(MemoryPerms.READ))
                {
                    return region.GetInt(i - totalSize - region.GetSize());
                }
                else
                {
                    Log.Error("Unauthorized memory read.");
                    return 0;
                }
                    
                    
            }
        }
        Log.Warning("Out of range memory read.");
        return 0;
    }
    
    public void SetInt(int i, int value)
    {
        int totalSize = 0;
        foreach (var region in regions)
        {
            totalSize += region.GetSize();
            if (i < totalSize)
            {
                if (region.permisions.HasFlag(MemoryPerms.WRITE))
                {
                    region.SetInt(i - totalSize - region.GetSize(), value);
                    return;
                }
                else
                {
                    Log.Error("Unauthorized memory write.");
                    return;
                }
                    
                    
            }
        }
        Log.Warning("Out of range memory write.");
        
    }
}

[Flags]
public enum MemoryPerms
{
    READ,
    WRITE,
    EXECUTE
}

public abstract class MemoryRegion
{
    
    
    public MemoryPerms permisions = MemoryPerms.READ | MemoryPerms.WRITE;
    public abstract int GetInt(int address);
    public abstract byte GetByte(int address);
    
    public abstract void SetInt(int address, int value);
    public abstract void SetByte(int address, byte value);

    public virtual int GetSize()
    {
        return 0x00FFFFFF;
    }
}

public class ProgramRom : MemoryRegion
{
    private byte[] _data;
    public override int GetInt(int address)
    {
        if (address + 4 >= _data.Length || address < 0)
        {
            Log.Warning("Out of bounds program read.");
            return 0;
        }
        return _data.GetInt(address);
    }

    public override byte GetByte(int address)
    {
        if (address >= _data.Length || address < 0)
        {
            Log.Warning("Out of bounds program read.");
            return 0;
        }
        return _data[address];
    }

    public override void SetInt(int address, int value)
    {
        throw new UnauthorizedAccessException();
    }

    public override void SetByte(int address, byte value)
    {
        throw new UnauthorizedAccessException();
    }


    public ProgramRom(byte[] executable)
    {
        _data = executable;
        permisions = MemoryPerms.READ | MemoryPerms.EXECUTE;
    }
}

public class BasicMemoryRegion : MemoryRegion
{
    private byte[] _data;
    public override int GetInt(int address)
    {
        if (address + 4 >= _data.Length || address < 0)
        {
            Log.Warning("Out of bounds program read.");
            return 0;
        }
        return _data.GetInt(address);
    }

    public override byte GetByte(int address)
    {
        if (address >= _data.Length || address < 0)
        {
            Log.Warning("Out of bounds program read.");
            return 0;
        }
        return _data[address];
    }

    public override void SetInt(int address, int value)
    {
        if (address + 4 >= _data.Length || address < 0)
        {
            Log.Warning("Out of bounds program write.");
            return;
        }
        _data.SetInt(address, value);
    }

    public override void SetByte(int address, byte value)
    {
        if (address >= _data.Length || address < 0)
        {
            Log.Warning("Out of bounds program write.");
            return;
        }
        _data[address] = value;
    }


    public BasicMemoryRegion()
    {
        _data = new byte[GetSize()];
        permisions = MemoryPerms.READ | MemoryPerms.WRITE;
    }
}

public class Ram : BasicMemoryRegion
{
    
}

public class VRam : BasicMemoryRegion
{
    
}

public class SystemPage : MemoryRegion
{

    private Emulator owner;
    
    public override int GetInt(int address)
    {
        switch (address)
        {
            case 0:
                return owner.state.ProgramCounter;
            case 4:
                return owner.state.CarryFlag;
            case 8:
                return owner.state.stack.Count;
        }

        return 0;
    }

    public override byte GetByte(int address)
    {
        switch (address)
        {
            case 0:
                return (byte)owner.state.ProgramCounter;
            case 4:
                return (byte)owner.state.CarryFlag;
            case 8:
                return (byte)owner.state.stack.Count;
        }

        return 0;
    }

    public override void SetInt(int address, int value)
    {
        switch (address)
        {
            case 0:
                owner.state.ProgramCounter = value;
                break;
            case 4:
                owner.state.CarryFlag = (byte)value;
                break;
            case 8:
                Log.Warning("Cannot write to stack pointer.");
                break;
        }

    }

    public override void SetByte(int address, byte value)
    {
        switch (address)
        {
            case 0:
                owner.state.ProgramCounter = value;
                break;
            case 4:
                owner.state.CarryFlag = (byte)value;
                break;
            case 8:
                Log.Warning("Cannot write to stack pointer.");
                break;
        }
    }

    public SystemPage(Emulator owner)
    {
        this.owner = owner;
        permisions = MemoryPerms.READ | MemoryPerms.WRITE;
    }

}

public class Stack : MemoryRegion
{

    public int Count => stack.Count;
    public System.Collections.Generic.Stack<byte> stack = new Stack<byte>();
    
    public override int GetInt(int address)
    {
        if (address < 0 || address + 4 >= stack.Count)
        {
            Log.Error("Out of bounds stack read.");
            return 0;
        }
        return BitConverter.ToInt32(stack.ToArray(), address);
    }

    public override byte GetByte(int address)
    {
        if (address < 0 || address >= stack.Count)
        {
            Log.Error("Out of bounds stack read.");
            return 0;
        }
        return stack.ToArray()[address];
    }

    public override void SetInt(int address, int value)
    {
        throw new UnauthorizedAccessException();
    }

    public override void SetByte(int address, byte value)
    {
        throw new UnauthorizedAccessException();
    }

    public int PopInt()
    {
        return stack.PopInt();
    }

    public void PushInt(int value)
    {
        stack.PushInt(value);
    }
    
    public int PeekInt()
    {
        return stack.PeekInt();
    }
    
    public byte PopByte()
    {
        return stack.Pop();
    }

    public void PushByte(byte value)
    {
        stack.Push(value);
    }
    
    public byte PeekByte()
    {
        return stack.Peek();
    }

    public Stack()
    {
        permisions = MemoryPerms.READ;
    }
}