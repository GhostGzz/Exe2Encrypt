using System;
using System.IO;
using System.Security.Cryptography;

class Program
{
    static void Main()
    {
        Console.Write("Enter the path to the .exe file: ");
        string exePath = Console.ReadLine();

        if (!File.Exists(exePath))
        {
            Console.WriteLine("File not found.");
            return;
        }

        byte[] key = GenerateRandomBytes(32); // AES-256 Key
        byte[] iv = GenerateRandomBytes(16);  // AES IV

        byte[] exeBytes = File.ReadAllBytes(exePath);
        byte[] encryptedBytes = EncryptAES(exeBytes, key, iv);

        string outputBin = exePath + ".enc.bin";
        File.WriteAllBytes(outputBin, encryptedBytes);
        Console.WriteLine($"Encrypted file saved as: {outputBin}");

        // Save the key and IV in a single file for decryption
        string keyFile = exePath + ".key";
        using (FileStream fs = new FileStream(keyFile, FileMode.Create, FileAccess.Write))
        {
            fs.Write(key, 0, key.Length);
            fs.Write(iv, 0, iv.Length);
        }

        Console.WriteLine($"Key and IV saved as: {keyFile}");
    }

    static byte[] EncryptAES(byte[] data, byte[] key, byte[] iv)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }
    }

    static byte[] GenerateRandomBytes(int size)
    {
        byte[] bytes = new byte[size];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }
}
