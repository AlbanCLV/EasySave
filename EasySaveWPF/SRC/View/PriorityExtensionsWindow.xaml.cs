using System.Windows;
using EasySaveWPF.ViewModelsWPF;

namespace EasySaveWPF.Views
{
    public partial class PriorityExtensionsWindow : Window
    {
        public PriorityExtensionsWindow()
        {
            InitializeComponent();
            DataContext = new PriorityExtensionsViewModel();
        }
    }
}
