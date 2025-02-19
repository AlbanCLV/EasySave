using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySaveConsole.Models;
using EasySaveConsole.Views;

namespace EasySaveConsole.Views
{
    internal class Lang_View
    {
        private readonly LangManager lang;
        private static Lang_View _instance;
        private static readonly object _lock = new object();
        public Lang_View()
        {
            lang = LangManager.Instance;

        }
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
        public string DisplayLangue()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("ChooseLanguage"); // e.g., "Choose a language:"
            Console.WriteLine("1. English");
            Console.WriteLine("2. Français");
            Console.Write("EnterChoice : "); // e.g., "Enter your choice: "
            return Console.ReadLine()?.Trim() == "2" ? "fr" : "en";
        }
        public void DisplayMessage(string key)
        {
            Console.WriteLine(lang.Translate(key));
        }
    }
}
