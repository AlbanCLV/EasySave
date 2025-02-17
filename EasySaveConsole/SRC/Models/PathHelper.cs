using System;
using System.IO;

namespace EasySaveConsole.Models
{
    /// <summary>
    /// Path helper to ensure path is correct
    /// </summary>
    public class PathHelper
    {
        public static string GetRelativePath(string basePath, string fullPath)
        {
            // Add "/" if necessary
            if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                basePath += Path.DirectorySeparatorChar;
            }

            Uri baseUri = new Uri(basePath, UriKind.Absolute);
            Uri fullUri = new Uri(fullPath, UriKind.Absolute);

            // Create relative path
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
