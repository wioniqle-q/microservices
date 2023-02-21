using System.Text;

namespace Liup.Authorization.Infrastructure.GlobalEncryption.Encryption;

public class EncryptionOptions
{
    public string PassPhrase { get; set; } = "1?";
    public byte[] SaltBytes { get; set; } = Encoding.ASCII.GetBytes("2?");
    public int Iterations { get; set; } = 1000;
    public int KeySize { get; set; } = 128;
    public int InitVectorSize { get; set; } = 16;
    public int SaltSize { get; set; } = 16;

    public EncryptionOptions(string passPhrase, byte[] saltBytes, int iterations, int keySize, int initVectorSize, int saltSize)
    {
        PassPhrase = passPhrase;
        SaltBytes = saltBytes;
        Iterations = iterations;
        KeySize = keySize;
        InitVectorSize = initVectorSize;
        SaltSize = saltSize;
    }

    public EncryptionOptions()
    {
    }
}
