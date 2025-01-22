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
}