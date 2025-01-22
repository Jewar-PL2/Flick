namespace Flick.Processor;

public class Cop0
{
    // I am not sure if COP0 registers should be uints or structures...
    
    public uint Read(uint index)
    {
        uint data = 0;
        
        switch (index)
        {
            default: 
                Utility.Panic($"COP0: Unhandled read from register {index}");
                break;
        }
        
        return data;
    }
    
    public void Write(uint index, uint value)
    {
        switch (index)
        {
            default: 
                Utility.Panic($"COP0: Unhandled write to register {index}: 0x{value:X8}");
                break;
        }
    }
}