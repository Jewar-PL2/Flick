namespace Flick.Processor;

public class Cop0
{
    // I am not sure if COP0 registers should be uints or structures...
    private uint status;

    public bool CacheIsolated => (status & 0x10000) != 0;
    
    public uint Read(uint index)
    {
        uint data = 0;
        
        switch (index)
        {
            case 12: data = status; break;
            
            default: 
                Utility.Panic($"COP0: Unhandled read from register {index}");
                break;
        }
        
        Utility.Log($"COP0: Read from register {index}");
        
        return data;
    }
    
    public void Write(uint index, uint value)
    {
        switch (index)
        {
            case 3: break;
            case 5: break;
            case 6: break;
            case 7: break;
            case 9: break;
            case 11: break;

            case 12: status = value; break;
            
            case 13:
                // CAUSE is not yet implemented, for now ignoring
                break;
            
            default: 
                Utility.Panic($"COP0: Unhandled write to register {index}: 0x{value:X8}");
                break;
        }
        
        Utility.Log($"COP0: Write to register {index}: 0x{value:X8}");
    }
}