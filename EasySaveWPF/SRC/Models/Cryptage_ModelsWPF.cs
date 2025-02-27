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
    /// <summary>
    /// Provides encryption and decryption functionalities using AES.
    /// Implements a singleton pattern to manage encryption settings and operations.
    /// </summary>
    internal class Cryptage_ModelsWPF
    {
        private static Cryptage_ModelsWPF _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the singleton instance of <see cref="Cryptage_ModelsWPF"/>.
        /// Ensures only one instance exists (thread-safe).
        /// </summary>
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

        /// <summary>
        /// Stores the user's password for encryption and decryption.
        /// </summary>
        public static string UserPassword { get; set; }

        /// <summary>
        /// Indicates whether all files should be encrypted.
        /// </summary>
        public static bool EncryptAll { get; set; }

        /// <summary>
        /// Stores the selected file extensions for encryption.
        /// </summary>
        public static string[] SelectedExtensions { get; set; }

        /// <summary>
        /// Indicates whether encryption is enabled.
        /// </summary>
        public static bool EncryptEnabled { get; set; }

        /// <summary>
        /// Sets encryption settings such as password, selected extensions, and encryption mode.
        /// </summary>
        /// <param name="password">The encryption password.</param>
        /// <param name="encryptAllSetting">Indicates whether to encrypt all files.</param>
        /// <param name="selectedExtensionsSetting">Array of selected file extensions to encrypt.</param>
        /// <param name="encrypt">Enables or disables encryption.</param>
        public void SetEncryptionSettings(string password, bool encryptAllSetting, string[] selectedExtensionsSetting, bool encrypt)
        {
            UserPassword = password;
            EncryptAll = encryptAllSetting;
            SelectedExtensions = selectedExtensionsSetting;
            EncryptEnabled = encrypt;
        }

        /// <summary>
        /// Generates an AES key from the user's password using SHA-256.
        /// </summary>
        /// <returns>A 256-bit key derived from the password.</returns>
        private static byte[] GenerateKeyFromPassword()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(UserPassword));
            }
        }

        /// <summary>
        /// Processes a file by encrypting or decrypting it.
        /// </summary>
        /// <param name="filePath">The path of the file to process.</param>
        /// <param name="encrypt">True for encryption, false for decryption.</param>
        /// <returns>A tuple containing the input file path, output file path, and operation success status.</returns>
        public (string, string, bool) ProcessFile(string filePath, bool encrypt)
        {
            string outputFile = encrypt ? filePath + ".aes" : filePath.Replace(".aes", "");
            Console.WriteLine((encrypt ? "Starting encryption" : "Starting decryption") + " for file: " + filePath);

            return encrypt ? EncryptFile(filePath, outputFile) : DecryptFile(filePath, outputFile);
        }

        /// <summary>
        /// Reads a password input from the console without displaying characters.
        /// </summary>
        /// <returns>The entered password as a string.</returns>
        public string ReadPassword()
        {
            StringBuilder sb = new StringBuilder();
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);
                else if (!char.IsControl(key.KeyChar))
                    sb.Append(key.KeyChar);
            }
            Console.WriteLine();
            return sb.ToString();
        }

        /// <summary>
        /// Encrypts a file using AES encryption.
        /// </summary>
        /// <param name="inputFile">The path of the file to encrypt.</param>
        /// <param name="outputFile">The path of the encrypted output file.</param>
        /// <returns>A tuple containing the input file path, output file path, and encryption success status.</returns>
        private (string, string, bool) EncryptFile(string inputFile, string outputFile)
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
                File.Delete(inputFile);
                return (inputFile, outputFile, true);
            }
            catch (Exception ex)
            {
                return ($"KO {ex.Message}", "KO", false);
            }
        }

        /// <summary>
        /// Decrypts a file using AES decryption.
        /// </summary>
        /// <param name="filePath">The path of the file to decrypt.</param>
        /// <returns>A tuple containing the input file path, output file path, and decryption success status.</returns>
        public (string, string, bool) DecryptFileWithResult(string filePath)
        {
            string outputFile = filePath.Replace(".aes", "");
            return DecryptFile(filePath, outputFile);
        }

        /// <summary>
        /// Decrypts a file using AES encryption.
        /// </summary>
        /// <param name="inputFile">The path of the encrypted file.</param>
        /// <param name="outputFile">The path where the decrypted file will be saved.</param>
        /// <returns>A tuple containing the input file path, output file path, and decryption success status.</returns>
        private (string, string, bool) DecryptFile(string inputFile, string outputFile)
        {
            try
            {
                byte[] key = GenerateKeyFromPassword();
                using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                {
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
                return ("KO", "KO", false);
            }
        }
    }
}
