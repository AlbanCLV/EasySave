using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System;
using EasySaveWPF.ModelsWPF;
using System.Windows;

namespace EasySaveWPF.ViewModelsWPF
{
    public class BusinessApps_ViewModel : INotifyPropertyChanged
    {
        private readonly ProcessWatcherWPF _processWatcher;
        private string _newAppName;

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

        public BusinessApps_ViewModel()
        {
            _processWatcher = ProcessWatcherWPF.Instance;
            BusinessApplications = new ObservableCollection<string>(_processWatcher.GetBusinessApplications());

            // Écouter l'événement BusinessAppStateChanged
            _processWatcher.BusinessAppStateChanged += OnBusinessAppStateChanged;

            AddApplicationCommand = new RelayCommand(AddApplication);
            RemoveApplicationCommand = new RelayCommand(RemoveApplication);
        }

        private void AddApplication(object param)
        {
            if (!string.IsNullOrWhiteSpace(NewAppName))
            {
                _processWatcher.AddBusinessApplication(NewAppName);
                BusinessApplications.Add(NewAppName);
                NewAppName = ""; // Réinitialiser le champ
            }
        }

        private void RemoveApplication(object param)
        {
            if (param is string appName && BusinessApplications.Contains(appName))
            {
                _processWatcher.RemoveBusinessApplication(appName);
                BusinessApplications.Remove(appName);
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
                }
                else
                {
                    MessageBox.Show($"L'application métier '{appName}' a été fermée.",
                                    "Application Métier Fermée", MessageBoxButton.OK, MessageBoxImage.Information);
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
