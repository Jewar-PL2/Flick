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
}