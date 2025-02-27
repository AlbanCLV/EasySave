using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasySaveWPF.ModelsWPF
{
    /// <summary>
    /// Manages priority file extensions for backup operations.
    /// </summary>
    public static class PriorityManager
    {
        /// <summary>
        /// Path to the file storing priority extensions.
        /// </summary>
        private static readonly string FilePath = "PriorityExtensions.txt";

        /// <summary>
        /// List of priority file extensions defined by the user.
        /// </summary>
        public static List<string> PriorityExtensions { get; set; } = new List<string>();

        /// <summary>
        /// Global counter for pending priority files.
        /// </summary>
        public static int PendingPriorityFiles = 0;

        /// <summary>
        /// Static constructor to load extensions at startup.
        /// </summary>
        static PriorityManager()
        {
            LoadExtensions();
        }

        /// <summary>
        /// Checks if a given file has a priority extension.
        /// </summary>
        /// <param name="filePath">Path of the file to check.</param>
        /// <returns>True if the file has a priority extension, otherwise false.</returns>
        public static bool IsPriority(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return PriorityExtensions.Contains(ext);
        }

        /// <summary>
        /// Loads priority extensions from the configuration file.
        /// If the file does not exist, initializes with an empty list.
        /// </summary>
        public static void LoadExtensions()
        {
            if (File.Exists(FilePath))
            {
                PriorityExtensions = File.ReadAllLines(FilePath)
                    .Select(line => line.Trim().ToLower())
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Distinct()
                    .ToList();
            }
            else
            {
                PriorityExtensions = new List<string>();
                SaveExtensions();
            }
        }

        /// <summary>
        /// Saves the current list of priority extensions to the configuration file.
        /// </summary>
        public static void SaveExtensions()
        {
            File.WriteAllLines(FilePath, PriorityExtensions);
        }
    }
}
