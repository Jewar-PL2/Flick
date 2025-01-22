namespace Flick.Processor;

public partial class R3000
{
    private PsxCore core;
    
    private uint[] registers;
    
    private uint programCounter;
    private uint nextProgramCounter;
    
    private bool inBranchDelaySlot;
    private bool branchTaken;

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

        inBranchDelaySlot = branchTaken;
        branchTaken = false;
        
        Execute();
    }

    private void Execute()
    {
        switch (instruction.Opcode)
        {
            case 0x00:
                switch (instruction.Function)
                {
                    case 0x00: SLL(); break;
                    default: IllegalInstruction(); break;
                }
                break;
            
            case 0x02: J(); break;
            case 0x09: ADDIU(); break;
            case 0x0D: ORI(); break;
            case 0x0F: LUI(); break;
            case 0x2B: SW(); break;
            
            default: IllegalInstruction(); break;
        }
    }
}