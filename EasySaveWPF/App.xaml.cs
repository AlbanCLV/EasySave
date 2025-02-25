using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace EasySaveWPF
{
    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AllocConsole(); // ✅ Ouvre une console au démarrage
            Console.WriteLine("[SERVER] Console initialisée.");
        }
    }
}
