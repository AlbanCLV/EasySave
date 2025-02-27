using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using EasySaveLog;
using EasySaveWPF.ModelsWPF;
using Microsoft.VisualBasic.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace EasySaveWPF.ViewModelsWPF
{
    public class PriorityExtensionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Log_ViewModels Log_VM;

        public ObservableCollection<string> PriorityExtensions { get; set; }

        private string _newExtension;
        public string NewExtension
        {
            get => _newExtension;
            set
            {
                _newExtension = value;
                OnPropertyChanged("NewExtension");
            }
        }

        private string _selectedExtension;
        public string SelectedExtension
        {
            get => _selectedExtension;
            set
            {
                _selectedExtension = value;
                OnPropertyChanged("SelectedExtension");
            }
        }

        public ICommand AddExtensionCommand { get; set; }
        public ICommand RemoveExtensionCommand { get; set; }

        public PriorityExtensionsViewModel()
        {
            // Initialise la collection à partir de PriorityManager
            PriorityExtensions = new ObservableCollection<string>(PriorityManager.PriorityExtensions);
            AddExtensionCommand = new RelayCommand(AddExtension, CanAddExtension);
            RemoveExtensionCommand = new RelayCommand(RemoveExtension, CanRemoveExtension);
            Log_VM = Log_ViewModels.Instance;
        }

        private void AddExtension(object obj)
        {
            if (!string.IsNullOrWhiteSpace(NewExtension))
            {
                string ext = NewExtension.Trim();
                if (!ext.StartsWith("."))
                {
                    ext = "." + ext;
                }
                ext = ext.ToLower();

                if (!PriorityExtensions.Contains(ext))
                {
                    PriorityExtensions.Add(ext);
                    Log_VM.LogBackupAction(ext, "", "", "", "Add Extension", "");
                    PriorityManager.PriorityExtensions.Add(ext);
                    PriorityManager.SaveExtensions(); // Sauvegarde après ajout
                }
                NewExtension = string.Empty;
            }
        }

        private bool CanAddExtension(object obj) => !string.IsNullOrWhiteSpace(NewExtension);

        private void RemoveExtension(object obj)
        {
            if (!string.IsNullOrWhiteSpace(SelectedExtension))
            {
                PriorityExtensions.Remove(SelectedExtension);
                Log_VM.LogBackupAction(SelectedExtension, "", "", "", "Remove Extension", "");

                PriorityManager.PriorityExtensions.Remove(SelectedExtension);
                PriorityManager.SaveExtensions(); // Sauvegarde après suppression
            }
        }

        private bool CanRemoveExtension(object obj) => !string.IsNullOrWhiteSpace(SelectedExtension);

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
