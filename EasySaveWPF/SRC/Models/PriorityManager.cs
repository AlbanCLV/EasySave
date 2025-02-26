using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasySaveWPF.ModelsWPF
{
    public static class PriorityManager
    {
        // Chemin du fichier de sauvegarde des extensions prioritaires
        private static readonly string FilePath = "PriorityExtensions.txt";

        // Liste des extensions prioritaires définie par l'utilisateur
        public static List<string> PriorityExtensions { get; set; } = new List<string>();

        // Compteur global des fichiers prioritaires en attente
        public static int PendingPriorityFiles = 0;

        // Constructeur statique pour charger les extensions au démarrage
        static PriorityManager()
        {
            LoadExtensions();
        }

        public static bool IsPriority(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return PriorityExtensions.Contains(ext);
        }

        public static void LoadExtensions()
        {
            if (File.Exists(FilePath))
            {
                // Charge les extensions en supprimant les espaces inutiles et en convertissant en minuscule
                PriorityExtensions = File.ReadAllLines(FilePath)
                    .Select(line => line.Trim().ToLower())
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Distinct()
                    .ToList();
            }
            else
            {
                // Si le fichier n'existe pas, on initialise avec une liste par défaut
                PriorityExtensions = new List<string> {};
                SaveExtensions();
            }
        }

        public static void SaveExtensions()
        {
            // Écrit chaque extension sur une ligne
            File.WriteAllLines(FilePath, PriorityExtensions);
        }
    }
}
