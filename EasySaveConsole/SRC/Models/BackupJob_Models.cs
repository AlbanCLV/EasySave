//Représente un travail de sauvegarde (nom, source, destination, type de sauvegarde, état, etc.).

using System;

namespace EasySave.Models
{
    public enum BackupType
    {
        Full,
        Differential
    }

    public class BackupJob_Models
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public BackupType Type { get; set; }

        public BackupJob_Models(string name, string source, string target, BackupType type)
        {
            Name = name;
            SourceDirectory = source;
            TargetDirectory = target;
            Type = type;
        }
    }
}
