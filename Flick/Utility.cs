namespace Flick;

public static class Utility
{
    public static void Panic(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("[PANIC] " + message);
        Console.ResetColor();

        // Todo: Replace this with a better Exception
        throw new Exception("Panic");
    }

    public static void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("[LOG] " + message);
        Console.ResetColor();
    }

    public static uint ReadUInt32(byte[] buffer, uint offset)
    {
        return BitConverter.ToUInt32(buffer, (int)offset);
    }
    
    public static ushort ReadUInt16(byte[] buffer, uint offset)
    {
        return BitConverter.ToUInt16(buffer, (int)offset);
    }
    
    public static void WriteUInt32(byte[] buffer, uint offset, uint value)
    {
        buffer[offset + 0] = (byte)(value >> 0);
        buffer[offset + 1] = (byte)(value >> 8);
        buffer[offset + 2] = (byte)(value >> 16);
        buffer[offset + 3] = (byte)(value >> 24);
    }
    
    public static void WriteUInt16(byte[] buffer, uint offset, ushort value)
    {
        buffer[offset + 0] = (byte)(value >> 0);
        buffer[offset + 1] = (byte)(value >> 8);
    }
}