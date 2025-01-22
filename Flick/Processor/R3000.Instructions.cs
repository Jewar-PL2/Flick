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
        PerformDelayedLoad();
        
        registers[instruction.Rd] = value;
    }
    
    private void SRA()
    {
        uint value = (uint)((int)registers[instruction.Rt] >> (byte)instruction.Shift);
        PerformDelayedLoad();
        
        registers[instruction.Rd] = value;
    }

    private void JR()
    {
        nextProgramCounter = registers[instruction.Rs];
        branchTaken = true;
        
        PerformDelayedLoad();
    }
    
    private void JALR()
    {
        registers[instruction.Rd] = nextProgramCounter;
        JR();
    }
    
    private void ADDU()
    {
        uint a = registers[instruction.Rs];
        uint b = registers[instruction.Rt];
        uint result = a + b;
        PerformDelayedLoad();
        
        registers[instruction.Rd] = result;
    }

    private void SUBU()
    {
        uint a = registers[instruction.Rs];
        uint b = registers[instruction.Rt];
        uint result = a - b;
        PerformDelayedLoad();
        
        registers[instruction.Rd] = result;
    }
    
    private void AND()
    {
        uint value = registers[instruction.Rs] & registers[instruction.Rt];
        PerformDelayedLoad();
        
        registers[instruction.Rd] = value;
    }
    
    private void OR()
    {
        uint value = registers[instruction.Rs] | registers[instruction.Rt];
        PerformDelayedLoad();
        
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
        registers[31] = nextProgramCounter;
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

    private void LB()
    {
        uint address = registers[instruction.Rs] + instruction.ImmediateSigned;

        sbyte value = (sbyte) Read8(address);
        SetupDelayedLoad(instruction.Rt, (uint)value);
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
    
    private void LBU()
    {
        uint address = registers[instruction.Rs] + instruction.ImmediateSigned;

        byte value = Read8(address);
        SetupDelayedLoad(instruction.Rt, value);
    }
    
    private void SB()
    {
        uint address = registers[instruction.Rs] + instruction.ImmediateSigned;
        byte value = (byte)registers[instruction.Rt];
        PerformDelayedLoad();
        
        Write8(address, value);
    }
    
    private void SH()
    {
        uint address = registers[instruction.Rs] + instruction.ImmediateSigned;
        ushort value = (ushort)registers[instruction.Rt];
        PerformDelayedLoad();
        
        if (address % 2 != 0)
        {
            Utility.Panic($"CPU: Unaligned address: 0x{address:8X}");
            return;
        }

        Write16(address, value);
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