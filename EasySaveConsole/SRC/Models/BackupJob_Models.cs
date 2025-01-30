//Représente un travail de sauvegarde (nom, source, destination, type de sauvegarde, état, etc.).

using System;

namespace EasySave.Models
{
    public enum BackupType
    {
        Full,
        Differential
    }

    public class BackupJobModel
    {
        public string Name { get; private set; }
        public string SourceDirectory { get; private set; }
        public string TargetDirectory { get; private set; }
        public BackupType Type { get; private set; }

        public BackupJobModel(string name, string source, string target, BackupType type)
        {
            Name = name;
            SourceDirectory = source;
            TargetDirectory = target;
            Type = type;
        }
    }
}
