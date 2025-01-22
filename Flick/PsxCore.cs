namespace Flick;

public class PsxCore
{
    // Todo: Perhaps replace array with pointer if it boosts performance
    private byte[] bios;

    public PsxCore(string biosPath)
    {
        // Todo: Verify is BIOS size is correct
        bios = File.ReadAllBytes(biosPath);
    }
}