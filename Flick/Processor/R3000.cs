namespace Flick.Processor;

using LoadSlot = (uint index, uint value);
public partial class R3000
{
    private PsxCore core;
    private Cop0 cop0;
    
    private uint[] registers;
    
    private uint programCounter;
    private uint nextProgramCounter;
    
    private bool inBranchDelaySlot;
    private bool branchTaken;

    private LoadSlot? loadSlot;

    private Instruction instruction;

    public R3000(PsxCore core)
    {
        this.core = core;
        cop0 = new Cop0();
        
        registers = new uint[32];
        programCounter = 0xBFC00000;
        nextProgramCounter = programCounter + 4;

        loadSlot = null;
        
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

    private void PerformDelayedLoad()
    {
        if (loadSlot is null) return;
        
        (uint index, uint value) = loadSlot.Value;
        registers[index] = value;
        loadSlot = null;
    }

    private void SetupDelayedLoad(uint index, uint value)
    {
        if (loadSlot is not null)
        {
            (uint pendingIndex, uint pendingValue) = loadSlot.Value;
            if (pendingIndex != index) registers[pendingIndex] = pendingValue;
        }
        
        loadSlot = new LoadSlot(index, value);
    }

    private void Write32(uint address, uint value)
    {
        if (!cop0.CacheIsolated)
        {
            core.Write32(address, value);
        }
    }

    private void Execute()
    {
        switch (instruction.Opcode)
        {
            case 0x00:
                switch (instruction.Function)
                {
                    case 0x00: SLL(); break;
                    case 0x25: OR(); break;
                    default: IllegalInstruction(); break;
                }
                break;
            
            case 0x02: J(); break;
            case 0x03: JAL(); break;
            case 0x09: ADDIU(); break;
            case 0x0D: ORI(); break;
            case 0x0F: LUI(); break;
            case 0x10: COP0(); break;
            case 0x2B: SW(); break;
            
            default: IllegalInstruction(); break;
        }
    }
}