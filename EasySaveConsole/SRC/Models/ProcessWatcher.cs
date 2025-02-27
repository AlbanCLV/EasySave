using System.Diagnostics;
using System.Threading;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using EasySaveLog;

namespace EasySaveConsole.Models
{
    /// <summary>
    /// Monitors business applications and pauses/resumes backups accordingly.
    /// Implements Singleton pattern.
    /// </summary>
    public class ProcessWatcher
    {
        private static ProcessWatcher _instance;
        private static readonly object _lock = new object();
        private static bool _running = true;
        private static readonly string ConfigFilePath = "business_apps.txt";
        private static bool _wasBusinessAppRunning = false;

        /// <summary>
        /// Gets the singleton instance of ProcessWatcher.
        /// </summary>
        public static ProcessWatcher Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new ProcessWatcher();
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Starts monitoring business applications in a background thread.
        /// </summary>
        public void StartWatching()
        {
            Thread watcherThread = new Thread(() =>
            {
                while (_running)
                {
                    bool isRunning = IsBusinessApplicationRunning();

                    if (isRunning && !_wasBusinessAppRunning)
                    {
                        Console.WriteLine("\n⚠️ Logiciel métier détecté ! Les sauvegardes sont suspendues.");
                        _wasBusinessAppRunning = true;
                    }
                    else if (!isRunning && _wasBusinessAppRunning)
                    {
                        Console.WriteLine("\n✅ Logiciel métier fermé. Les sauvegardes peuvent reprendre.");
                        _wasBusinessAppRunning = false;
                    }

                    Thread.Sleep(2000); // Check every 2 seconds
                }
            })
            {
                IsBackground = true
            };
            watcherThread.Start();
        }

        /// <summary>
        /// Checks if any monitored business application is running.
        /// </summary>
        /// <returns>True if a business application is running, otherwise false.</returns>
        public bool IsBusinessApplicationRunning()
        {
            if (!File.Exists(ConfigFilePath))
                return false;

            var metierApplications = File.ReadAllLines(ConfigFilePath)
                                         .Select(line => line.Trim())
                                         .Where(line => !string.IsNullOrWhiteSpace(line))
                                         .ToArray();

            var runningProcesses = Process.GetProcesses();

            foreach (var process in runningProcesses)
            {
                try
                {
                    if (metierApplications.Any(app => process.ProcessName.Equals(app, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    continue; // Avoid access errors
                }
            }
            return false;
        }

        /// <summary>
        /// Stops monitoring business applications.
        /// </summary>
        public void StopWatching()
        {
            _running = false;
        }

        /// <summary>
        /// Displays the list of registered business applications.
        /// </summary>
        public void DisplayExistingApplications()
        {
            if (File.Exists(ConfigFilePath))
            {
                List<string> existingApps = File.ReadAllLines(ConfigFilePath)
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .ToList();

                Console.WriteLine("\n📋 Applications métier déjà enregistrées :");
                if (existingApps.Count > 0)
                {
                    foreach (var app in existingApps)
                    {
                        Console.WriteLine("- " + app);
                    }
                }
                else
                {
                    Console.WriteLine("(Aucune application enregistrée pour l'instant.)");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Displays menu options and gets the user's choice.
        /// </summary>
        /// <returns>The user's choice as a string.</returns>
        public string GetChoiceMetier()
        {
            Console.WriteLine("=== Gestion des Applications Métier ===");
            Console.WriteLine("1. Ajouter une application métier");
            Console.WriteLine("2. Supprimer une application métier");
            Console.WriteLine("3. Retour au menu principal");
            Console.Write("\nChoisissez une option : ");
            return Console.ReadLine();
        }

        /// <summary>
        /// Adds a business application to the monitored list.
        /// </summary>
        /// <param name="appName">The application name to add.</param>
        public void AddBusinessApplication(string appName)
        {
            if (string.IsNullOrWhiteSpace(appName)) return;

            List<string> existingApps = File.ReadAllLines(ConfigFilePath)
                .Select(line => line.Trim().ToLower())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            if (existingApps.Contains(appName.ToLower()))
            {
                Console.WriteLine($"{appName} est déjà dans la liste des logiciels métier.");
            }
            else
            {
                File.AppendAllText(ConfigFilePath, appName + Environment.NewLine);
                Console.WriteLine($"{appName} ajouté à la liste des logiciels métier.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }

        /// <summary>
        /// Removes a business application from the monitored list.
        /// </summary>
        /// <returns>The name of the removed application, or an error message.</returns>
        public string RemoveBusinessApplication()
        {
            if (!File.Exists(ConfigFilePath) || File.ReadAllLines(ConfigFilePath).Length == 0)
            {
                Console.WriteLine("\n❌ Aucune application à supprimer. Retour au menu précédent.");
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
                return "No_Applications";
            }

            List<string> existingApps = File.ReadAllLines(ConfigFilePath)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            Console.WriteLine("\n📋 Applications métier enregistrées :");
            for (int i = 0; i < existingApps.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {existingApps[i]}");
            }

            Console.Write("\nEntrez le numéro de l'application à supprimer : ");
            if (int.TryParse(Console.ReadLine(), out int choix) && choix > 0 && choix <= existingApps.Count)
            {
                string applicationSupprimee = existingApps[choix - 1];

                Console.Write($"\n⚠️ Êtes-vous sûr de vouloir supprimer \"{applicationSupprimee}\" ? (O/N) : ");
                string confirmation = Console.ReadLine().Trim().ToLower();

                if (confirmation == "o" || confirmation == "oui")
                {
                    existingApps.RemoveAt(choix - 1);
                    File.WriteAllLines(ConfigFilePath, existingApps);
                    Console.WriteLine($"\n✅ L'application \"{applicationSupprimee}\" a été supprimée !");
                    return applicationSupprimee;
                }
                else
                {
                    Console.WriteLine("\n❌ Suppression annulée.");
                    return "canceled";
                }
            }
            return "Invalid_Number";
        }
    }
}
