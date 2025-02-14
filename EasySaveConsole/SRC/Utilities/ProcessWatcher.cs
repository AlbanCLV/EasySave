using System.Diagnostics;
using System.Threading;
using System;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace EasySave.Utilities
{
    class ProcessWatcher
    {
        private static bool _running = true;
        private static readonly string ConfigFilePath = "business_apps.txt";
        private static bool _wasBusinessAppRunning = false;

        public static void StartWatching()
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

        public static bool IsBusinessApplicationRunning()
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

        public static void StopWatching()
        {
            _running = false;
        }
    }
}
