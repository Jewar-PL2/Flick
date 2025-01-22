namespace Flick.Processor;

public partial class R3000
{
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
            Utility.Panic($"Unaligned address: 0x{address:8X}");
            return;
        }

        core.Write32(address, value);
    }
}