using Flick;
using Flick.Processor;

namespace Flick.ConsoleApp;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please specify BIOS path");
            return;
        }
        
        string biosPath = args[0];
        
        PsxCore psxCore = new PsxCore(biosPath);
        R3000 cpu = new R3000(psxCore);

        while (true)
        {
            // Crashes at unhandled instruction or memory access
            cpu.Step();
        }
    }
}

// I need my baby boy...