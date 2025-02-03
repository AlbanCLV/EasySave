// Fournit des méthodes pour lire et écrire des fichiers JSON.

using System;
using System.IO;
using Newtonsoft.Json;

namespace EasySave.Utilities
{
    public static class JsonHelper
    {
        // Sauvegarde un objet en JSON dans un fichier.
        public static void SaveToJson<T>(string filePath, T data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                // Crée le répertoire si nécessaire
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving JSON: {ex.Message}");
            }
        }

        // Charge un objet depuis un fichier JSON.
        public static T LoadFromJson<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default;

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON: {ex.Message}");
                return default;
            }
        }
    }
}
