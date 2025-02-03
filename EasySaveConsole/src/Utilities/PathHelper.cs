using System;
using System.IO;

namespace EasySave
{
    public class PathHelper
    {
        public static string GetRelativePath(string basePath, string fullPath)
        {
            // Ajoute une barre de séparation finale si nécessaire
            if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                basePath += Path.DirectorySeparatorChar;
            }

            Uri baseUri = new Uri(basePath, UriKind.Absolute);
            Uri fullUri = new Uri(fullPath, UriKind.Absolute);

            // Calcule le chemin relatif
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
