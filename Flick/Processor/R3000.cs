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

    private void Branch()
    {
        nextProgramCounter = programCounter + instruction.ImmediateSigned * 4;
        branchTaken = true;
    }

    private uint Read32(uint address)
    {
        return core.Read32(address);
    }
    
    private ushort Read16(uint address)
    {
        return core.Read16(address);
    }
    
    private byte Read8(uint address)
    {
        return core.Read8(address);
    }
    
    private void Write32(uint address, uint value)
    {
        if (!cop0.CacheIsolated)
        {
            core.Write32(address, value);
        }
    }
    
    private void Write16(uint address, ushort value)
    {
        if (!cop0.CacheIsolated)
        {
            core.Write16(address, value);
        }
    }
    
    private void Write8(uint address, byte value)
    {
        if (!cop0.CacheIsolated)
        {
            core.Write8(address, value);
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
                    case 0x03: SRA(); break;
                    case 0x08: JR(); break;
                    case 0x23: SUBU(); break;
                    case 0x24: AND(); break;
                    case 0x25: OR(); break;
                    default: IllegalInstruction(); break;
                }
                break;
            
            case 0x02: J(); break;
            case 0x03: JAL(); break;
            case 0x04: BEQ(); break;
            case 0x05: BNE(); break;
            case 0x09: ADDIU(); break;
            case 0x0D: ORI(); break;
            case 0x0F: LUI(); break;
            case 0x10: COP0(); break;
            case 0x20: LB(); break;
            case 0x23: LW(); break;
            case 0x28: SB(); break;
            case 0x29: SH(); break;
            case 0x2B: SW(); break;
            
            default: IllegalInstruction(); break;
        }
    }
}