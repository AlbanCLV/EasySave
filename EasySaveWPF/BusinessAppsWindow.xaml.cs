using System.Windows;
using EasySaveWPF.ViewModelsWPF;

namespace EasySaveWPF.Views
{
    public partial class BusinessAppsWindow : Window
    {
        public BusinessAppsWindow()
        {
            InitializeComponent();
            DataContext = new BusinessApps_ViewModel(); // Lier la fenêtre au ViewModel
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close(); // Ferme la fenêtre
        }
    }
}