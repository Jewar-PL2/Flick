namespace Flick;

public class PsxCore
{
    // Todo: Perhaps replace array with pointer if it boosts performance
    private byte[] bios;
    private byte[] ram;

    private static uint[] RegionMasks =
    [
        0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, // KUSEG: 2048MB
        0x7FFFFFFF,                                     // KSEG0:  512MB
        0x1FFFFFFF,                                     // KSEG1:  512MB
        0xFFFFFFFF, 0xFFFFFFFF,                         // KSEG2: 1024MB
    ];

    public static uint GetMaskedAddress(uint address) => address & RegionMasks[address >> 29];

    public PsxCore(string biosPath)
    {
        // Todo: Verify if BIOS size is correct
        bios = File.ReadAllBytes(biosPath);
        ram = new byte[2 * 1024 * 1024];
    }

    public uint Read32(uint address)
    {
        address = GetMaskedAddress(address);

        if (address < 0x1F000000)
        {
            // 2 MB RAM is mirrored
            return Utility.ReadUInt32(ram, address & 0x1FFFFF);
        }
        
        if (address >= 0x1FC00000 && address < 0x1FC80000)
        {
            return Utility.ReadUInt32(bios, address - 0x1FC00000);
        }
        
        Utility.Panic($"PSXCORE: Unhandled Read32 from 0x{address:X8}"); 
        return 0x00;
    }

    public void Write32(uint address, uint value)
    {
        address = GetMaskedAddress(address);

        if (address < 0x1F000000)
        {
            // 2 MB RAM is mirrored
            Utility.WriteUInt32(ram, address & 0x1FFFFF, value);
            return;
        }
        
        if (address >= 0x1F801000 && address < 0x1F801040)
        {
            Utility.Log($"PSXCORE: Unhandled Write32 to MEMORY CONTROL 1 at 0x{address:X8}: 0x{value:X8}");
            return;
        }
        
        if (address >= 0x1F801060 && address < 0x1F801064)
        {
            Utility.Log($"PSXCORE: Unhandled Write32 to RAM_SIZE at 0x{address:X8}: 0x{value:X8}");
            return;
        }
        
        if (address == 0xFFFE0130)
        {
            Utility.Log($"PSXCORE: Unhandled Write32 to CACHE CONTROL at 0x{address:X8}: 0x{value:X8}");
            return;
        }

        Utility.Panic($"PSXCORE: Unhandled Write32 to 0x{address:X8}: 0x{value:X8}");
    }
}