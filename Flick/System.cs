namespace Flick;

public class System
{
    // Todo: Perhaps replace array with pointer if it boosts performance
    private byte[] bios;

    public System(string biosPath)
    {
        // Todo: Verify is BIOS size is correct
        bios = File.ReadAllBytes(biosPath);
    }
}