using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EasySave.Utilities
{
    /// <summary>
    /// Manages translations for the application.
    /// </summary>
    public class LangManager
    {
        private Dictionary<string, string> translations;

        /// <summary>
        /// Directory containing language files.
        /// </summary>
        private readonly string langDirectory;

        /// <summary>
        /// Initializes the LangManager with a specified language.
        /// </summary>
        /// <param name="language">Language code (e.g., "en" or "fr").</param>
        public LangManager(string language)
        {
            // Set the language directory to a dynamic relative path inside the "src" folder
            langDirectory = Path.Combine(
    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,
    "src",
    "Lang"
);
            SetLanguage(language);
        }

        /// <summary>
        /// Loads the translations for the specified language.
        /// </summary>
        /// <param name="language">Language code (e.g., "en" or "fr").</param>
        public void SetLanguage(string language)
        {
            string filePath = Path.Combine(langDirectory, $"{language}.json");

            if (File.Exists(filePath))
            {
                try
                {
                    // Read JSON file with UTF-8 encoding
                    string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

                    // Deserialize the JSON content into a dictionary
                    translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                    // If the file is empty or invalid, initialize an empty dictionary
                    if (translations == null)
                    {
                        translations = new Dictionary<string, string>();
                    }
                }
                catch
                {
                    // If there's an error reading or parsing the file, fallback to an empty dictionary
                    translations = new Dictionary<string, string>();
                }
            }
            else
            {
                // If the file does not exist, initialize an empty dictionary
                translations = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Translates a key into the current language.
        /// </summary>
        /// <param name="key">The key to translate.</param>
        /// <returns>The translated string if available; otherwise, the key itself.</returns>
        public string Translate(string key)
        {
            // Return the translated value if it exists; otherwise, return the key itself
            return translations.ContainsKey(key) ? translations[key] : $"[Missing translation: {key}]";
        }
    }
}
