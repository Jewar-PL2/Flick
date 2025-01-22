namespace Flick.Processor;

public class R3000
{
    private PsxCore core;
    
    private uint[] registers;
    
    private uint programCounter;
    private uint nextProgramCounter;

    private Instruction instruction;

    public R3000(PsxCore core)
    {
        this.core = core;
        
        registers = new uint[32];
        programCounter = 0xBFC00000;
        nextProgramCounter = programCounter + 4;
        
        instruction = new Instruction(0);
    }

    private Instruction FetchInstruction()
    {
        // Todo: Handle iCache and stuff
        return new Instruction(core.Read32(programCounter));
    }

    public void Step()
    {
        instruction = FetchInstruction();

        programCounter = nextProgramCounter;
        nextProgramCounter = programCounter + 4;
        
        // what now?
    }

    private void Execute()
    {
        switch (instruction.Opcode)
        {
            default:
                Utility.Panic($"Unhandled instruction: {instruction.Raw:X8}");
                break;
        }
    }
}