// Fournit des m�thodes pour lire et �crire des fichiers JSON.

using System;
using System.IO;
using Newtonsoft.Json;

namespace EasySaveConsole.Models
{
    public static class JsonHelper
    {
        /// <summary>
        /// Provides methods for reading and writing JSON files.
        /// </summary>
        public static void SaveToJson<T>(string filePath, T data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving JSON: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads an object from a JSON file.
        /// </summary>
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