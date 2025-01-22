namespace Flick;

public class PsxCore
{
    // Todo: Perhaps replace array with pointer if it boosts performance
    private byte[] bios;

    public PsxCore(string biosPath)
    {
        // Todo: Verify if BIOS size is correct
        bios = File.ReadAllBytes(biosPath);
    }

    public uint Read32(uint address)
    {
        if (address >= 0xBFC00000 && address < 0xBFC80000)
        {
            return BitConverter.ToUInt32(bios, (int)(address - 0xBFC00000));
        }

        return 0x00;
    }

    public void Write32(uint address, uint value)
    {
        Utility.Log($"PSXCORE: Unhandled Write32 to 0x{address:X8}: 0x{value:X8}");
    }
}