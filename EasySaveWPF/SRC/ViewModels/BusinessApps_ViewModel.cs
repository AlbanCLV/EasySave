using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System;
using EasySaveWPF.ModelsWPF;
using System.Windows;
using EasySaveConsole.Models;
using EasySaveLog;
using System.Diagnostics;
using System.Reactive.Concurrency;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace EasySaveWPF.ViewModelsWPF
{
    public class BusinessApps_ViewModel : INotifyPropertyChanged
    {
        private readonly ProcessWatcherWPF _processWatcher;
        private string _newAppName;
        private LangManager lang;
        private Log_ViewModels Log_VM;
        Stopwatch stopwatch = new Stopwatch();
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> BusinessApplications { get; private set; }

        public string NewAppName
        {
            get => _newAppName;
            set
            {
                _newAppName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewAppName)));
            }
        }
        public ICommand AddApplicationCommand { get; }
        public ICommand RemoveApplicationCommand { get; }

        public BusinessApps_ViewModel(string select, bool e)
        {
            _processWatcher = ProcessWatcherWPF.Instance;
            BusinessApplications = new ObservableCollection<string>(_processWatcher.GetBusinessApplications());
            lang = LangManager.Instance;
            lang.SetLanguage(select);
            if (e)
            {
                _processWatcher.BusinessAppStateChanged += OnBusinessAppStateChanged; // Puis abonne
            }
            AddApplicationCommand = new RelayCommand(AddApplication);
            RemoveApplicationCommand = new RelayCommand(RemoveApplication);
            Log_VM = Log_ViewModels.Instance;
        }
        private void AddApplication(object param)
        {
            if (!string.IsNullOrWhiteSpace(NewAppName))
            {
                stopwatch.Start();
                _processWatcher.AddBusinessApplication(NewAppName);
                BusinessApplications.Add(NewAppName);
                stopwatch.Stop();
                string time = stopwatch.ElapsedMilliseconds.ToString();
                Log_VM.LogBackupAction(NewAppName, "", "", time, "", "");
                NewAppName = ""; // Réinitialiser le champ


            }
        }
        private void RemoveApplication(object param)
        {
            if (param is string appName && BusinessApplications.Contains(appName))
            {
                stopwatch.Start();
                _processWatcher.RemoveBusinessApplication(appName);
                BusinessApplications.Remove(appName);
                stopwatch.Stop();
                string time = stopwatch.ElapsedMilliseconds.ToString();
                Log_VM.LogBackupAction(appName, "", "", time, "RemoveApplication", "");
            }
        }
        private void OnBusinessAppStateChanged(string appName, bool isRunning)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (isRunning)
                {
                    MessageBox.Show($"Attention ! L'application métier '{appName}' est en cours d'exécution.",
                                    "Application Métier Détectée", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Log_VM.LogBackupAction(appName, "", "", "-1", "isRunning", "");

                }
                else
                {
                    MessageBox.Show($"L'application métier '{appName}' a été fermée.",
                                    "Application Métier Fermée", MessageBoxButton.OK, MessageBoxImage.Information);
                    Log_VM.LogBackupAction(appName, "", "", "-1", "Is CLose", "");

                }
            });
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}