namespace Flick.Processor;

public partial class R3000
{
    // Todo: Handle delayed loads before writing to registers

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

    private void J()
    {
        branchTaken = true;
        nextProgramCounter = (programCounter & 0xF0000000) | (instruction.Target * 4);
        Utility.Log($"R3000: Jumping to 0x{nextProgramCounter:X8}");
    }

    private void ADDIU()
    {
        uint a = registers[instruction.Rs];
        uint b = instruction.ImmediateSigned;
        uint result = a + b;
        
        registers[instruction.Rt] = result;
    }
    
    private void ORI()
    {
        uint value = registers[instruction.Rs] | instruction.Immediate;
        
        registers[instruction.Rt] = value;
    }
    
    private void LUI()
    {
        registers[instruction.Rt] = instruction.Immediate << 16;
    }

    private void SW()
    {
        uint address = registers[instruction.Rs] + instruction.ImmediateSigned;
        uint value = registers[instruction.Rt];
        
        if (address % 4 != 0)
        {
            Utility.Panic($"CPU: Unaligned address: 0x{address:8X}");
            return;
        }

        core.Write32(address, value);
    }
}