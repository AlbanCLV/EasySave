using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using EasySaveLog;
using EasySaveWPF.ModelsWPF;
using Microsoft.VisualBasic.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace EasySaveWPF.ViewModelsWPF
{
    /// <summary>
    /// ViewModel for managing priority file extensions in the backup system.
    /// </summary>
    public class PriorityExtensionsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event triggered when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private Log_ViewModels Log_VM;

        /// <summary>
        /// Collection of priority file extensions.
        /// </summary>
        public ObservableCollection<string> PriorityExtensions { get; set; }

        private string _newExtension;
        /// <summary>
        /// Gets or sets the new extension to be added.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the selected extension for removal.
        /// </summary>
        public string SelectedExtension
        {
            get => _selectedExtension;
            set
            {
                _selectedExtension = value;
                OnPropertyChanged("SelectedExtension");
            }
        }

        /// <summary>
        /// Command for adding a new file extension.
        /// </summary>
        public ICommand AddExtensionCommand { get; set; }

        /// <summary>
        /// Command for removing an existing file extension.
        /// </summary>
        public ICommand RemoveExtensionCommand { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityExtensionsViewModel"/> class.
        /// Loads existing priority extensions and sets up commands.
        /// </summary>
        public PriorityExtensionsViewModel()
        {
            PriorityExtensions = new ObservableCollection<string>(PriorityManager.PriorityExtensions);
            AddExtensionCommand = new RelayCommand(AddExtension, CanAddExtension);
            RemoveExtensionCommand = new RelayCommand(RemoveExtension, CanRemoveExtension);
            Log_VM = Log_ViewModels.Instance;
        }

        /// <summary>
        /// Adds a new extension to the priority list.
        /// </summary>
        /// <param name="obj">The command parameter (not used).</param>
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
                    PriorityManager.SaveExtensions();
                }
                NewExtension = string.Empty;
            }
        }

        /// <summary>
        /// Determines whether the add extension command can execute.
        /// </summary>
        /// <param name="obj">The command parameter (not used).</param>
        /// <returns>True if a new extension is provided, otherwise false.</returns>
        private bool CanAddExtension(object obj) => !string.IsNullOrWhiteSpace(NewExtension);

        /// <summary>
        /// Removes the selected extension from the priority list.
        /// </summary>
        /// <param name="obj">The command parameter (not used).</param>
        private void RemoveExtension(object obj)
        {
            if (!string.IsNullOrWhiteSpace(SelectedExtension))
            {
                PriorityExtensions.Remove(SelectedExtension);
                Log_VM.LogBackupAction(SelectedExtension, "", "", "", "Remove Extension", "");

                PriorityManager.PriorityExtensions.Remove(SelectedExtension);
                PriorityManager.SaveExtensions();
            }
        }

        /// <summary>
        /// Determines whether the remove extension command can execute.
        /// </summary>
        /// <param name="obj">The command parameter (not used).</param>
        /// <returns>True if a selected extension exists, otherwise false.</returns>
        private bool CanRemoveExtension(object obj) => !string.IsNullOrWhiteSpace(SelectedExtension);

        /// <summary>
        /// Notifies that a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
