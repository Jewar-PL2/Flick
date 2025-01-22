namespace Flick.Processor;

public partial class R3000
{
    private void IllegalInstruction()
    {
        Utility.Panic($"CPU: Unhandled instruction: 0x{instruction.Raw:X8}");
    }
    
    private void SLL()
    {
        // Don't do anything if instruction is NOP
        if (instruction.Raw == 0) return;
        
        uint value = registers[instruction.Rt] << (byte)instruction.Shift;

        registers[instruction.Rd] = value;
    }

    private void JR()
    {
        nextProgramCounter = registers[instruction.Rs];
        branchTaken = true;
        
        PerformDelayedLoad();
    }
    
    private void OR()
    {
        uint value = registers[instruction.Rs] | registers[instruction.Rt];
        
        registers[instruction.Rd] = value;
    }

    private void J()
    {
        nextProgramCounter = (programCounter & 0xF0000000) | (instruction.Target * 4);
        branchTaken = true;
        
        PerformDelayedLoad();
    }

    private void JAL()
    {
        uint returnAddress = nextProgramCounter;
        registers[31] = returnAddress;
        J();
    }
    
    private void BEQ()
    {
        if (registers[instruction.Rs] == registers[instruction.Rt]) Branch();
        
        PerformDelayedLoad();
    }

    private void BNE()
    {
        if (registers[instruction.Rs] != registers[instruction.Rt]) Branch();
        
        PerformDelayedLoad();
    }

    private void ADDIU()
    {
        uint a = registers[instruction.Rs];
        uint b = instruction.ImmediateSigned;
        uint result = a + b;
        PerformDelayedLoad();
        
        registers[instruction.Rt] = result;
    }
    
    private void ORI()
    {
        uint value = registers[instruction.Rs] | instruction.Immediate;
        PerformDelayedLoad();
        registers[instruction.Rt] = value;
    }
    
    private void LUI()
    {
        PerformDelayedLoad();
        registers[instruction.Rt] = instruction.Immediate << 16;
    }

    private void COP0()
    {
        switch (instruction.Rs)
        {
            case 0: MFC0(); break;
            case 4: MTC0(); break;
            default: IllegalInstruction(); break;
        }
    }

    private void MFC0()
    {
        uint value = cop0.Read(instruction.Rd);
        SetupDelayedLoad(instruction.Rt, value);
    }

    private void MTC0()
    {
        uint value = registers[instruction.Rt];
        PerformDelayedLoad();
        
        cop0.Write(instruction.Rd, value);
    }

    private void LW()
    {
        uint address = registers[instruction.Rs] + instruction.ImmediateSigned;
        if (address % 4 != 0)
        {
            Utility.Panic($"CPU: Unaligned address: 0x{address:8X}");
            return;
        }

        uint value = Read32(address);
        SetupDelayedLoad(instruction.Rt, value);
    }

    private void SW()
    {
        uint address = registers[instruction.Rs] + instruction.ImmediateSigned;
        uint value = registers[instruction.Rt];
        PerformDelayedLoad();
        
        if (address % 4 != 0)
        {
            Utility.Panic($"CPU: Unaligned address: 0x{address:8X}");
            return;
        }

        Write32(address, value);
    }
}