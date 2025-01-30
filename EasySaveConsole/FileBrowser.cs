using System;
using Terminal.Gui;

namespace EasySave
{
    public class FileBrowser
    {
        public string BrowsePath(bool canChooseFiles = false, bool canChooseDirectories = true)
        {
            Application.Init();

            var dialog = new OpenDialog("Select Path", "Choose a file or directory")
            {
                CanChooseFiles = canChooseFiles,
                CanChooseDirectories = canChooseDirectories
            };

            Application.Run(dialog);

            if (!string.IsNullOrEmpty(dialog.FilePath.ToString()))
            {
                Application.Shutdown();
                return dialog.FilePath.ToString();
            }

            Application.Shutdown();
            return null; // Aucun chemin sélectionné
        }
    }
}