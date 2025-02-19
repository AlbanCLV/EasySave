using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySaveConsole.Models;

namespace EasySaveConsole.Views
{
    internal class Log_View
    {
        private readonly LangManager lang;
        private static Log_View _instance;
        private static readonly object _lock = new object();
        public Log_View()
        {
            lang = LangManager.Instance;

        }
        public static Log_View Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new Log_View();
                    }
                }
                return _instance;
            }
        }
        public void Get_Type_Log(string a)
        {
            Console.Clear();
            Console.WriteLine(lang.Translate("Type_Now") + " " + a);
        }

        public string GET_Type_File_Log()
        {
            Console.Write(lang.Translate("Choix_log"));
            string format = Console.ReadLine();

            while (format != "xml" && format != "json" && format != "exit")
            {
                Console.WriteLine(lang.Translate("No_Change_Type_Log"));
                Console.Write(lang.Translate("Choix_log"));
                format = Console.ReadLine();
            }
            if (format == "json" || format == "xml")
            {
                Console.WriteLine(lang.Translate("GUI_Log"));
                Console.WriteLine(format);
                return format;
            }
            else if (format == "exit")
            {
                Console.WriteLine(lang.Translate("No_Change_Log"));
                return format;
            }
            return format;
        }


    }
}
