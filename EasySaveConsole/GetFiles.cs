using System;
using System.IO;

namespace EasySave
{
    public class GetFile
    {
        // Attributes for source and destination
        public string Source { get; set; }
        public string Destination { get; set; }

        // Constructor to initialize source and destination
        public GetFile(string source, string destination)
        {
            Source = source;
            Destination = destination;
        }

        // Method to copy files and directories from source to destination
        public void Copy()
        {
            try
            {
                // Validate source directory
                if (!Directory.Exists(Source))
                {
                    Console.WriteLine($"Source directory '{Source}' does not exist.");
                    return;
                }

                // Copy files and directories
                CopyDirectory(Source, Destination);
                Console.WriteLine("Copy operation completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            // Create the target directory if it doesn't exist
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // Copy all files in the current directory
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDir, fileName);
                File.Copy(file, destFile, overwrite: true);
            }

            // Recursively copy subdirectories
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string subDirName = Path.GetFileName(subDir);
                string destSubDir = Path.Combine(targetDir, subDirName);
                CopyDirectory(subDir, destSubDir);
            }
        }
    }
}
