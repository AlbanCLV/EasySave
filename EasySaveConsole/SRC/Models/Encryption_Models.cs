using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EasySaveConsole.Controllers;

namespace EasySaveConsole.Models
{
    /// <summary>
    /// Utility for encrypting/decrypting files using AES in CBC mode with PKCS7 padding.
    /// This version uses a random IV for each file, which is written as the first 16 bytes
    /// of the encrypted file.
    /// </summary>
    public class Encryption_Models
    {
        private static Encryption_Models _instance;
        private static readonly object _lock = new object();
        public static Encryption_Models Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new Encryption_Models();
                    }
                }
                return _instance;
            }
        }
        private static string UserPassword;
        private static bool EncryptAll;
        private static string[] SelectedExtensions;
        private static bool EncryptEnabled;

        /// <summary>
        /// Configures encryption parameters.
        /// </summary>
        public void SetEncryptionSettings(string password, bool encryptAllSetting, string[] selectedExtensionsSetting, bool encryptEnabledSetting)
        {
            UserPassword = password;
            EncryptAll = encryptAllSetting;
            SelectedExtensions = selectedExtensionsSetting;
            EncryptEnabled = encryptEnabledSetting;
            Console.WriteLine("Encryption settings updated. Encryption enabled: " + EncryptEnabled);
        }

        /// <summary>
        /// Indicates whether encryption is enabled.
        /// </summary>
        public static bool IsEncryptionEnabled => EncryptEnabled;

        /// <summary>
        /// Generates an AES key from the user's password using SHA256.
        /// </summary>
        private static byte[] GenerateKeyFromPassword()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(UserPassword));
                Console.WriteLine("Key generated. Length: " + key.Length);
                return key;
            }
        }

        /// <summary>
        /// Processes a file by encrypting or decrypting it based on the 'encrypt' parameter.
        /// </summary>
        public static void ProcessFile(string filePath, bool encrypt)
        {
            if (!EncryptEnabled)
            {
                Console.WriteLine("Encryption is disabled.");
                return;
            }

            // For testing, extension filtering is disabled.
            string outputFile = encrypt ? filePath + ".aes" : filePath.Replace(".aes", "");
            Console.WriteLine((encrypt ? "Starting encryption" : "Starting decryption") + " for file: " + filePath);
            if (encrypt)
                EncryptFile(filePath, outputFile);
            else
                DecryptFile(filePath, outputFile);
        }

        /// <summary>
        /// Encrypts the input file and writes the encrypted data (prefixed by a random IV) to the output file.
        /// </summary>
        private static void EncryptFile(string inputFile, string outputFile)
        {
            try
            {
                byte[] key = GenerateKeyFromPassword();
                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.GenerateIV();  // Generate a random IV
                    byte[] iv = aes.IV;
                    Console.WriteLine("IV generated. Length: " + iv.Length);

                    // Write the IV at the beginning of the output file
                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    {
                        fsOutput.Write(iv, 0, iv.Length);
                        using (CryptoStream cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                        {
                            fsInput.CopyTo(cs);
                        }
                    }
                }
                // Delete the original file after successful encryption
                File.Delete(inputFile);
                Console.WriteLine($"Encryption completed for file: {inputFile}. Encrypted file: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during encryption: " + ex.Message);
            }
        }

        /// <summary>
        /// Decrypts the input file and writes the decrypted data to the output file.
        /// The method reads the IV from the beginning of the encrypted file.
        /// </summary>
        private static bool DecryptFile(string inputFile, string outputFile)
        {
            try
            {
                byte[] key = GenerateKeyFromPassword();
                using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                {
                    // Read the IV from the beginning of the file
                    byte[] iv = new byte[16];
                    int bytesRead = fsInput.Read(iv, 0, iv.Length);
                    if (bytesRead < iv.Length)
                        throw new Exception("Failed to read IV from file.");

                    using (Aes aes = Aes.Create())
                    {
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.Key = key;
                        aes.IV = iv;

                        using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        using (CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            cs.CopyTo(fsOutput);
                        }
                    }
                }
                File.Delete(inputFile);
                Console.WriteLine($"Decryption completed for file: {inputFile}. Decrypted file: {outputFile}");
                return true;
            }
            catch (CryptographicException)
            {
                Console.WriteLine("Decryption error: Incorrect password or corrupt file: " + inputFile);
                if (File.Exists(outputFile))
                    File.Delete(outputFile);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during decryption: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Public method to decrypt a file and return the result.
        /// </summary>
        public  bool DecryptFileWithResult(string filePath)
        {
            string outputFile = filePath.Replace(".aes", "");
            return DecryptFile(filePath, outputFile);
        }
    }
}
