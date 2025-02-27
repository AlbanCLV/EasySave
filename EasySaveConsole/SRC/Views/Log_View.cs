using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasySaveConsole.Models;

namespace EasySaveConsole.Views
{
    /// <summary>
    /// Classe représentant l'affichage des logs dans la console.
    /// Permet la sélection du format de log et l'affichage du type de log actuel.
    /// </summary>
    internal class Log_View
    {
        /// <summary>
        /// Instance du gestionnaire de langues pour la traduction des messages.
        /// </summary>
        private readonly LangManager lang;

        /// <summary>
        /// Instance unique de la classe Log_View (Singleton).
        /// </summary>
        private static Log_View _instance;

        /// <summary>
        /// Objet utilisé pour verrouiller l'instance (thread-safe singleton).
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Constructeur privé de la classe Log_View.
        /// Initialise le gestionnaire de langues.
        /// </summary>
        private Log_View()
        {
            lang = LangManager.Instance;
        }

        /// <summary>
        /// Obtient l'instance unique de Log_View (Singleton).
        /// </summary>
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

        /// <summary>
        /// Affiche le type de log actuellement utilisé.
        /// </summary>
        /// <param name="a">Le type de log à afficher.</param>
        public void Get_Type_Log(string a)
        {
            Console.Clear();
            Console.WriteLine(lang.Translate("Type_Now") + " " + a);
        }

        /// <summary>
        /// Demande à l'utilisateur de sélectionner un format de fichier pour les logs.
        /// L'utilisateur peut choisir entre JSON et XML ou quitter sans changer le format.
        /// </summary>
        /// <returns>Le format sélectionné ("json", "xml" ou "exit").</returns>
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
