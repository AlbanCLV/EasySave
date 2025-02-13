using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EasySaveConsole.Utilities
{
    public class EncryptionUtility
    {
        private static readonly string ConfigFile = "config.txt";
        private static string UserPassword;
        private static bool EncryptAll;
        private static string[] SelectedExtensions;
        private static bool EncryptEnabled; // Indique si le chiffrement est activé

        /// <summary>
        /// Définit les paramètres de chiffrement selon la saisie utilisateur.
        /// </summary>
        public static void SetEncryptionSettings(string password, bool encryptAllSetting, string[] selectedExtensions, bool encryptEnabledSetting)
        {
            UserPassword = password;
            EncryptAll = encryptAllSetting;
            SelectedExtensions = selectedExtensions;
            EncryptEnabled = encryptEnabledSetting;
        }

        private static byte[] GenerateKeyFromPassword()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(UserPassword));
            }
        }

        /// <summary>
        /// Traite le fichier en fonction du mode (chiffrer ou déchiffrer) si le chiffrement est activé.
        /// Pour le déchiffrement, on n’utilise pas le résultat.
        /// </summary>
        public static void ProcessFile(string filePath, bool encrypt)
        {
            if (!EncryptEnabled)
                return;

            if (!EncryptAll && !SelectedExtensions.Any(ext => filePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            string outputFile = encrypt ? filePath + ".aes" : filePath.Replace(".aes", "");
            if (encrypt)
                EncryptFile(filePath, outputFile);
            else
                DecryptFile(filePath, outputFile);
        }

        private static void EncryptFile(string inputFile, string outputFile)
        {
            byte[] key = GenerateKeyFromPassword();
            byte[] iv = key.Take(16).ToArray();

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (CryptoStream cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    fsInput.CopyTo(cs);
                }
            }
            File.Delete(inputFile);
            Console.WriteLine($"Chiffré: {inputFile}");
        }

        /// <summary>
        /// Déchiffre un fichier en renvoyant true si l'opération réussit, false sinon.
        /// </summary>
        private static bool DecryptFile(string inputFile, string outputFile)
        {
            byte[] key = GenerateKeyFromPassword();
            byte[] iv = key.Take(16).ToArray();

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                    using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                    using (CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cs.CopyTo(fsOutput);
                    }
                }
                File.Delete(inputFile);
                Console.WriteLine($"Déchiffré: {inputFile}");
                return true;
            }
            catch (CryptographicException)
            {
                Console.WriteLine("Erreur : Mauvais mot de passe ou déchiffrement échoué pour le fichier : " + inputFile);
                if (File.Exists(outputFile))
                    File.Delete(outputFile);
                return false;
            }
        }

        /// <summary>
        /// Méthode publique pour déchiffrer un fichier et obtenir un booléen résultat.
        /// </summary>
        public static bool DecryptFileWithResult(string filePath)
        {
            string outputFile = filePath.Replace(".aes", "");
            return DecryptFile(filePath, outputFile);
        }
    }
}
