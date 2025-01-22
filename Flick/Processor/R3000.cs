namespace Flick.Processor;

public class R3000
{
    private System system;
    
    private uint[] registers;
    
    private uint programCounter;
    private uint nextProgramCounter;

    public R3000(System sys)
    {
        system = sys;
        registers = new uint[32];
        programCounter = 0xBFC00000;
        nextProgramCounter = programCounter + 4;
    }
}