namespace VRISC;

public static class Extensions
{
    public static int GetInt(this byte[] data, int index)
    {
        if (index >= data.Length || index < 0)
        {
            Log.Error("Index out of range.");
            throw new IndexOutOfRangeException();
        }
        return BitConverter.ToInt32(data, index);
    }
    
    public static void SetInt(this byte[] data, int index, int value)
    {
        if (index + 4 >= data.Length || index < 0)
        {
            Log.Error("Index out of range.");
            throw new IndexOutOfRangeException();
        }
        byte[] d = BitConverter.GetBytes(value);
        data[index + 0] = d[0];
        data[index + 1] = d[1];
        data[index + 2] = d[2];
        data[index + 3] = d[3];
    }
    
    public static int PeekInt(this Stack<byte> stack)
    {
        List<byte> bytes = stack.ToList();
       
        return BitConverter.ToInt32(bytes.ToArray(), bytes.Count - 4);
    }
    
    public static int PopInt(this Stack<byte> stack)
    {
        
        if (stack.Count < 4)
        {
            Log.Error("Stack is empty.");
            return 0;
        }
        List<byte> bytes = new List<byte>();
        bytes.Add(stack.Pop());
        bytes.Add(stack.Pop());
        bytes.Add(stack.Pop());
        bytes.Add(stack.Pop());
        bytes.Reverse();
        return BitConverter.ToInt32(bytes.ToArray(), 0);
    }

    public static void PushInt(this Stack<byte> stack, int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        foreach (var b in bytes)
        {
            stack.Push(b);
        }
    }
    
}