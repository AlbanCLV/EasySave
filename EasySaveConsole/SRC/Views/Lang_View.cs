using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySaveConsole.Models;
using EasySaveConsole.Views;

namespace EasySaveConsole.Views
{
    /// <summary>
    /// Represents the language selection and message display view.
    /// </summary>
    internal class Lang_View
    {
        private readonly LangManager lang;
        private static Lang_View _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="Lang_View"/> class.
        /// </summary>
        public Lang_View()
        {
            lang = LangManager.Instance;
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="Lang_View"/>.
        /// </summary>
        public static Lang_View Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new Lang_View();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Displays the language selection menu and returns the chosen language code.
        /// </summary>
        /// <returns>The language code ("en" for English, "fr" for French).</returns>
        public string DisplayLangue()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("ChooseLanguage"); // e.g., "Choose a language:"
            Console.WriteLine("1. English");
            Console.WriteLine("2. Français");
            Console.Write("EnterChoice : "); // e.g., "Enter your choice: "
            return Console.ReadLine()?.Trim() == "2" ? "fr" : "en";
        }

        /// <summary>
        /// Displays a translated message based on the given key.
        /// </summary>
        /// <param name="key">The translation key for the message to display.</param>
        public void DisplayMessage(string key)
        {
            Console.WriteLine(lang.Translate(key));
        }
    }
}
