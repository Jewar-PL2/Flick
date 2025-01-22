namespace Flick.Processor;

public record Instruction(uint Raw)
{
    public uint Immediate => (ushort)Raw;
    public uint ImmediateSigned => (uint)(short)Raw;

    public uint Target => Raw & 0x3FFFFFF;

    public uint Opcode => (Raw >> 26) & 0x3F;
    public uint Rs => (Raw >> 21) & 0x1F;
    public uint Rt => (Raw >> 16) & 0x1F;
    public uint Rd => (Raw >> 11) & 0x1F;
    public uint Shift => (Raw >> 6) & 0x1F;
    public uint Function => Raw & 0x3F;

    public uint CoprocessorId => (Raw >> 26) & 0x3;
}