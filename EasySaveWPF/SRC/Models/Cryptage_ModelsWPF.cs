using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Web;

namespace EasySaveWPF.ModelsWPF
{
    internal class Cryptage_ModelsWPF
    {
        private static Cryptage_ModelsWPF _instance;
        private static readonly object _lock = new object();
        public static Cryptage_ModelsWPF Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new Cryptage_ModelsWPF();
                    }
                }
                return _instance;
            }
        }
        
            public static string UserPassword { get; set; }
            public static bool EncryptAll { get; set; }
            public static string[] SelectedExtensions { get; set; }
            public static bool EncryptEnabled { get; set; } 

        public void SetEncryptionSettings(string password, bool encryptAllSetting, string[] selectedExtensionsSetting, bool encrypt)
        {
            UserPassword = password;
            EncryptAll = encryptAllSetting;
            SelectedExtensions = selectedExtensionsSetting;
            EncryptEnabled = encrypt;
        }

        /// <summary>
        /// Generates an AES key from the user's password using SHA256.
        /// </summary>
        private static byte[] GenerateKeyFromPassword()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(UserPassword));
                return key;
            }
        }
        public (string, string, bool) ProcessFile(string filePath, bool encrypt)
        {


            string outputFile = encrypt ? filePath + ".aes" : filePath.Replace(".aes", "");
            Console.WriteLine((encrypt ? "Starting encryption" : "Starting decryption") + " for file: " + filePath);
            if (encrypt)
                return EncryptFile(filePath, outputFile);
            else
            {
                return DecryptFile(filePath, outputFile);
            }
        }
        private (string,string,bool) EncryptFile(string inputFile, string outputFile)
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
                return (inputFile, outputFile, true);
            }
            catch (Exception ex)
            {
                return ($"KO {ex.Message}","KO",false);
            }
        }
        public (string, string, bool) DecryptFileWithResult(string filePath)
        {
            string outputFile = filePath.Replace(".aes", "");
            return  DecryptFile(filePath, outputFile);
            
        }
        private (string,string, bool) DecryptFile(string inputFile, string outputFile)
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
                return (inputFile, outputFile, true);
            }
            catch (CryptographicException)
            {
                if (File.Exists(outputFile))
                    File.Delete(outputFile);
                return ("KO Decryption", "KO Decryption", false);
            }
            catch (Exception ex)
            {
                return ("KO","KO", false) ;
            }
        }

    }

}
