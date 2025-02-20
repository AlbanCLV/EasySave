using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using EasySave;
using System.Diagnostics;

namespace EasySaveConsole.Models
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
            DirectoryInfo dir = new DirectoryInfo(basePath);

            // Remonter jusqu'à trouver "Cesi-AlbanCalvo"
            while (dir != null && dir.Name != "Cesi-AlbanCalvo")
            {
                dir = dir.Parent;
            }

            if (dir == null)
            {
                throw new DirectoryNotFoundException("Impossible de trouver le dossier 'Cesi-AlbanCalvo'.");
            }

            // Construire le chemin final
            langDirectory = Path.Combine(dir.FullName, "EasySave", "EasySaveConsole", "SRC", "Lang");

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
                    _instance = new LangManager(Program.SelectedLanguage); // Default language, can be overridden
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
