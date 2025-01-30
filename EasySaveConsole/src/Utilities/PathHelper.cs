// Valide les chemins des fichiers et répertoires (locaux, externes, réseaux).

using System;
using System.IO;

namespace EasySave.Utilities
{
    public class PathHelper
    {
        public static string GetRelativePath(string basePath, string fullPath)
        {
            Uri baseUri = new Uri(basePath);
            Uri fullUri = new Uri(fullPath);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
        public static string GetUniqueDirectoryName(string destinationPath, string sourceDirectoryPath)
        {
            string sourceDirectoryName = Path.GetFileName(sourceDirectoryPath.TrimEnd(Path.DirectorySeparatorChar));
            string uniqueName = sourceDirectoryName;
            int counter = 1;

            while (Directory.Exists(Path.Combine(destinationPath, uniqueName)))
            {
                uniqueName = $"{sourceDirectoryName} ({counter})";
                counter++;
            }

            return Path.Combine(destinationPath, uniqueName);
        }
    }
}
