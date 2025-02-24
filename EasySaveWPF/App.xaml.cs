using System;
using System.Windows;

namespace EasySaveWPF
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run(new MainWindow());
        }
    }
}
