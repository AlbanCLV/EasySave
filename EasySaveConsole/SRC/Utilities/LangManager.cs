using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EasySave.Utilities
{
    /// <summary>
    /// Gère les traductions de l'application via des fichiers JSON.
    /// </summary>
    public class LangManager
    {
        private Dictionary<string, string> translations;
        private readonly string langDirectory;
        private static LangManager _instance;

        // Constructeur privé pour le singleton.
        private LangManager(string language)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            // Remonter jusqu'au répertoire racine du projet
            string projectRoot = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));
            langDirectory = Path.Combine(projectRoot, "SRC", "Lang");

            SetLanguage(language);
            Console.WriteLine($"LangManager initialized with language: {language}");
        }

        /// <summary>
        /// Accède à l'instance unique du LangManager.
        /// Par défaut, la langue est "en".
        /// </summary>
        public static LangManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LangManager("en");
                }
                return _instance;
            }
        }

        /// <summary>
        /// Charge les traductions depuis le fichier JSON correspondant à la langue.
        /// </summary>
        public void SetLanguage(string language)
        {
            string filePath = Path.Combine(langDirectory, $"{language}.json");

            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                    translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                                   ?? new Dictionary<string, string>();
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

        /// <summary>
        /// Retourne la traduction associée à une clé.
        /// </summary>
        public string Translate(string key)
        {
            return translations != null && translations.ContainsKey(key)
                ? translations[key]
                : $"[Missing translation: {key}]";
        }
    }
}
