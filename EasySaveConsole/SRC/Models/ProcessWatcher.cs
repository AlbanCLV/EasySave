using System.Diagnostics;
using System.Threading;
using System;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using EasySaveLog;

namespace EasySaveConsole.Models
{
    public class ProcessWatcher
    {
        private static ProcessWatcher _instance;
        private static readonly object _lock = new object();
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
        private static bool _running = true;
        private static readonly string ConfigFilePath = "business_apps.txt";
        private static bool _wasBusinessAppRunning = false;

        public  void StartWatching()
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

                    Thread.Sleep(2000); // Vérification toutes les 2 secondes
                }
            })
            {
                IsBackground = true
            };
            watcherThread.Start();
        }

        public  bool IsBusinessApplicationRunning()
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
                    continue; // Éviter erreurs d'accès
                }
            }

            return false;
        }

        public  void StopWatching()
        {
            _running = false;
        }
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
        public void AddBusinessApplication(string appName)
        {
            if (string.IsNullOrWhiteSpace(appName)) return;

            List<string> existingApps = File.ReadAllLines(ConfigFilePath)
            .Select(line => line.Trim().ToLower()) // Normalisation
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

            // Vérifier si l'application existe déjà
            if (existingApps.Contains(appName.ToLower())) // Comparaison insensible à la casse
            {
                Console.WriteLine($"{appName} est déjà dans la liste des logiciels métier.");
            }
            else
            {
                File.AppendAllText(ConfigFilePath, appName + Environment.NewLine);
                Console.WriteLine($"{appName} ajouté à la liste des logiciels métier.");
            }
            Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
            Console.ReadKey(); // Pause pour laisser le temps à l'utilisateur de voir le message
        }
    }
}
