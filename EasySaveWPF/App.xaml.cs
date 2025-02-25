using System.Configuration;
using System.Data;
using System.Windows;
using EasySaveWPF.ModelsWPF;

namespace EasySaveWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _ = ProcessWatcherWPF.Instance; // Initialisation au démarrage
        }
    }

}
