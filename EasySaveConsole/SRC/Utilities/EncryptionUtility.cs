using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EasySave.Utilities
{
    /// <summary>
    /// Utilitaire de chiffrement/déchiffrement des fichiers.
    /// </summary>
    public class EncryptionUtility
    {
        private static string UserPassword;
        private static bool EncryptAll;
        private static string[] SelectedExtensions;
        private static bool EncryptEnabled;

        /// <summary>
        /// Configure les paramètres de chiffrement.
        /// </summary>
        public static void SetEncryptionSettings(string password, bool encryptAllSetting, string[] selectedExtensionsSetting, bool encryptEnabledSetting)
        {
            UserPassword = password;
            EncryptAll = encryptAllSetting;
            SelectedExtensions = selectedExtensionsSetting;
            EncryptEnabled = encryptEnabledSetting;
            Console.WriteLine("Encryption settings updated. Encryption enabled: " + EncryptEnabled);
        }

        /// <summary>
        /// Indique si le chiffrement est activé.
        /// </summary>
        public static bool IsEncryptionEnabled => EncryptEnabled;

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
        /// Traite le fichier en chiffrant ou déchiffrant selon le paramètre "encrypt".
        /// </summary>
        public static void ProcessFile(string filePath, bool encrypt)
        {
            if (!EncryptEnabled)
            {
                Console.WriteLine("Encryption is disabled.");
                return;
            }

            // Pour tester, le filtrage par extension est désactivé.
            string outputFile = encrypt ? filePath + ".aes" : filePath.Replace(".aes", "");
            Console.WriteLine((encrypt ? "Starting encryption" : "Starting decryption") + " for file: " + filePath);
            if (encrypt)
                EncryptFile(filePath, outputFile);
            else
                DecryptFile(filePath, outputFile);
        }

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
                    // Utilise les 16 premiers octets de la clé comme IV
                    aes.IV = key.Take(16).ToArray();
                    Console.WriteLine("IV generated. Length: " + aes.IV.Length);

                    using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    using (CryptoStream cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        fsInput.CopyTo(cs);
                    }
                }
                File.Delete(inputFile);
                Console.WriteLine($"Encryption completed for file: {inputFile}. Encrypted file: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during encryption: " + ex.Message);
            }
        }

        private static bool DecryptFile(string inputFile, string outputFile)
        {
            try
            {
                byte[] key = GenerateKeyFromPassword();
                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.IV = key.Take(16).ToArray();

                    using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    using (CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cs.CopyTo(fsOutput);
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

        public static bool DecryptFileWithResult(string filePath)
        {
            string outputFile = filePath.Replace(".aes", "");
            return DecryptFile(filePath, outputFile);
        }
    }
}
