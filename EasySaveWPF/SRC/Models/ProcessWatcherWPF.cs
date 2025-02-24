using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Threading;

namespace EasySaveWPF.ModelsWPF
{
    public class ProcessWatcherWPF
    {
        private static ProcessWatcherWPF _instance;
        private static readonly object _lock = new object();
        private static readonly string ConfigFilePath = "business_apps.txt";
        private DispatcherTimer _timer;
        public event Action<string, bool> BusinessAppStateChanged;
        private bool _isBusinessAppRunning = false;
        private string _lastDetectedApp = null; // Stocke la dernière application métier détectée

        public static ProcessWatcherWPF Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new ProcessWatcherWPF();
                    return _instance;
                }
            }
        }

        private ProcessWatcherWPF()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _timer.Tick += (s, e) => CheckBusinessApplications();
            CheckBusinessApplications(); // Vérification immédiate au démarrage
            _timer.Start();
        }

        public void CheckBusinessApplications()
        {
            string runningApp = GetRunningBusinessApps();
            bool isRunning = !string.IsNullOrEmpty(runningApp);

            if (isRunning)
            {
                if (!_isBusinessAppRunning)
                {
                    _isBusinessAppRunning = true;
                    _lastDetectedApp = runningApp; // Stocker le nom de l'application détectée
                    BusinessAppStateChanged?.Invoke(runningApp, true);
                }
            }
            else if (_isBusinessAppRunning)
            {
                _isBusinessAppRunning = false;
                BusinessAppStateChanged?.Invoke(_lastDetectedApp, false); // Utiliser le dernier nom stocké
                _lastDetectedApp = null; // Réinitialiser après affichage
            }
        }

        public string GetRunningBusinessApps()
        {
            if (!File.Exists(ConfigFilePath))
                return null;

            var businessApps = File.ReadAllLines(ConfigFilePath)
                                   .Select(line => line.Trim().ToLower())
                                   .Where(line => !string.IsNullOrWhiteSpace(line))
                                   .ToList();

            var runningProcess = Process.GetProcesses()
                                        .FirstOrDefault(p => businessApps.Contains(p.ProcessName.ToLower()));

            return runningProcess?.ProcessName;
        }

        public List<string> GetBusinessApplications()
        {
            if (!File.Exists(ConfigFilePath)) return new List<string>();
            return File.ReadAllLines(ConfigFilePath)
                       .Select(line => line.Trim())
                       .Where(line => !string.IsNullOrWhiteSpace(line))
                       .ToList();
        }

        public void AddBusinessApplication(string appName)
        {
            if (string.IsNullOrWhiteSpace(appName)) return;

            var existingApps = GetBusinessApplications();
            if (!existingApps.Contains(appName.ToLower()))
            {
                File.AppendAllText(ConfigFilePath, appName + Environment.NewLine);
            }
        }

        public void RemoveBusinessApplication(string appName)
        {
            var apps = GetBusinessApplications();
            if (apps.Contains(appName))
            {
                apps.Remove(appName);
                File.WriteAllLines(ConfigFilePath, apps);
            }
        }
    }
}
