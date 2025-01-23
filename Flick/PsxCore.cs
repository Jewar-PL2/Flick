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

    private record Range(uint Start, uint Size)
    {
        public bool Contains(uint address, out uint offset)
        {
            if (address >= Start && address < Start + Size)
            {
                offset = address - Start;
                return true;
            }

            offset = 0;
            return false;
        }
    }

    private readonly struct MemoryRanges
    {
        public static readonly Range Ram          = new(0x00000000, 2 * 1024 * 1024);
        public static readonly Range Expansion    = new(0x1F000000, 1024 * 1024);
        public static readonly Range MemControl   = new(0x1F801000, 36);
        public static readonly Range RamSize      = new(0x1F801060, 4);
        public static readonly Range IrqControl   = new(0x1F801070, 8);
        public static readonly Range Spu          = new(0x1F801C00, 640);
        public static readonly Range Expansion2   = new(0x1F802000, 66);
        public static readonly Range Bios         = new(0x1FC00000, 512 * 1024);
        public static readonly Range CacheControl = new(0xFFFE0130, 4);
    }
    
    public PsxCore(string biosPath)
    {
        // Todo: Verify if BIOS size is correct
        bios = File.ReadAllBytes(biosPath);
        ram = new byte[2 * 1024 * 1024];
    }

    public uint Read32(uint address)
    {
        address = GetMaskedAddress(address);

        if (MemoryRanges.Ram.Contains(address, out uint offset))
        {
            // 2 MB RAM is mirrored
            return Utility.ReadUInt32(ram, offset & 0x1FFFFF);
        }
        
        // TODO: FIX THIS OR CPU ASAP
        // For some reason the CPU tries to read from EXPANSION 1 (it shouldn't do that yet)
        // Then the PC gets misaligned which shouldn't happen either
        // I need to figure out what's going on
        
        if (MemoryRanges.Bios.Contains(address, out offset))
        {
            return Utility.ReadUInt32(bios, offset);
        }
        
        Utility.Panic($"PSXCORE: Unhandled Read32 from 0x{address:X8}"); 
        return 0x00;
    }
    
    public ushort Read16(uint address)
    {
        address = GetMaskedAddress(address);
        
        Utility.Panic($"PSXCORE: Unhandled Read16 from 0x{address:X8}"); 
        return 0x00;
    }

    public byte Read8(uint address)
    {
        address = GetMaskedAddress(address);
        
        if (MemoryRanges.Ram.Contains(address, out uint offset))
        {
            // 2 MB RAM is mirrored
            return ram[offset & 0x1FFFFF];
        }
        
        if (MemoryRanges.Expansion.Contains(address, out offset))
        {
            Utility.Log($"PSXCORE: Unhandled Read8 from EXPANSION at 0x{address:X8}");
            return 0xFF;
        }
        
        if (MemoryRanges.Bios.Contains(address, out offset))
        {
            return bios[offset];
        }
        
        Utility.Panic($"PSXCORE: Unhandled Read8 from 0x{address:X8}"); 
        return 0x00;
    }

    public void Write32(uint address, uint value)
    {
        address = GetMaskedAddress(address);

        if (MemoryRanges.Ram.Contains(address, out uint offset))
        {
            // 2 MB RAM is mirrored
            Utility.WriteUInt32(ram, offset & 0x1FFFFF, value);
            return;
        }
        
        if (MemoryRanges.MemControl.Contains(address, out offset))
        {
            Utility.Log($"PSXCORE: Unhandled Write32 to MEMORY CONTROL 1 at 0x{address:X8}: 0x{value:X8}");
            return;
        }
        
        if (MemoryRanges.RamSize.Contains(address, out offset))
        {
            Utility.Log($"PSXCORE: Unhandled Write32 to RAM_SIZE at 0x{address:X8}: 0x{value:X8}");
            return;
        }
        
        if (MemoryRanges.IrqControl.Contains(address, out offset))
        {
            Utility.Log($"PSXCORE: Unhandled Write32 to IRQ CONTROL at 0x{address:X8}: 0x{value:X8}");
            return;
        }
        
        if (MemoryRanges.CacheControl.Contains(address, out offset))
        {
            Utility.Log($"PSXCORE: Unhandled Write32 to CACHE CONTROL at 0x{address:X8}: 0x{value:X8}");
            return;
        }

        Utility.Panic($"PSXCORE: Unhandled Write32 to 0x{address:X8}: 0x{value:X8}");
    }

    public void Write16(uint address, ushort value)
    {
        address = GetMaskedAddress(address);

        if (MemoryRanges.Spu.Contains(address, out uint offset))
        {
            Utility.Log($"PSXCORE: Unhandled Write16 to SPU at 0x{address:X8}: 0x{value:X8}");
            return;
        }
        
        Utility.Panic($"PSXCORE: Unhandled Write16 to 0x{address:X8}: 0x{value:X8}");
    }
    
    public void Write8(uint address, byte value)
    {
        address = GetMaskedAddress(address);

        if (MemoryRanges.Ram.Contains(address, out uint offset))
        {
            // 2 MB RAM is mirrored
            ram[offset & 0x1FFFFF] = value;
            return;
        }
        
        if (MemoryRanges.Expansion2.Contains(address, out offset))
        {
            Utility.Log($"PSXCORE: Unhandled Write8 to EXPANSION 2 at 0x{address:X8}: 0x{value:X8}");
            return;
        }
        
        Utility.Panic($"PSXCORE: Unhandled Write8 to 0x{address:X8}: 0x{value:X8}");
    }
}