using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using EasySaveWPF.ModelsWPF;

namespace EasySaveWPF.ViewModelsWPF
{
    public class PriorityExtensionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
