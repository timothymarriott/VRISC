namespace VRISC.Emulator;

public class Emulator
{
    public EmulatorState state = new EmulatorState();

    
    
    public delegate void OnStepCallback();
    public OnStepCallback OnStep;
    
    public delegate void OnStartCallback();
    public OnStartCallback OnStart;
    
    public delegate void OnHaltCallback();
    public OnHaltCallback OnHalt;
    
    
    public Emulator(byte[] executable)
    {
        
    }

    public void Step()
    {
        
    }
    
}


public class EmulatorState
{
    
}

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
    public abstract int GetSize();
}