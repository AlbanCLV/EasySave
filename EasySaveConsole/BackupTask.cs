using System;

namespace EasySave
{
    public enum BackupType
    {
        Full,
        Differential
    }

    public class BackupTask
    {
        public string Name { get; set; }
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
        public BackupType Type { get; set; }

        public BackupTask(string name, string source, string target, BackupType type)
        {
            Name = name;
            SourceDirectory = source;
            TargetDirectory = target;
            Type = type;
        }
    }
}
