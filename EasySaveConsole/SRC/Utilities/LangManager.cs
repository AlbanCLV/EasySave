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

        private readonly string langDirectory;

        private static LangManager _instance;

        // Private constructor to prevent instantiation
        public LangManager(string language)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));

            // Ensure we go to the correct SRC/Lang directory
            langDirectory = Path.Combine(projectRoot, "SRC", "Lang");

            SetLanguage(language);
            Console.WriteLine($"LangManager initialized with language: {language}");
        }

        // Singleton pattern for LangManager
        public static LangManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LangManager("en"); // Default language, can be overridden
                }
                return _instance;
            }
        }

        public void SetLanguage(string language)
        {
            string filePath = Path.Combine(langDirectory, $"{language}.json");

            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                    translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
                catch
                {
                    translations = new Dictionary<string, string>();
                }
            }
            else
            {
                translations = new Dictionary<string, string>();
            }
        }

        public string Translate(string key)
        {
            return translations.ContainsKey(key) ? translations[key] : $"[Missing translation: {key}]";
        }
    }
}
