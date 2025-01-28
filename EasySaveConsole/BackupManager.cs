using System;
using System.Collections.Generic;
using System.IO;

namespace EasySave
{
    public class BackupManager
    {
        private readonly List<BackupTask> tasks = new List<BackupTask>();
        private readonly Logger logger = new Logger();
        private readonly StateManager stateManager = new StateManager();

        public void CreateBackupTask()
        {
            Console.Write("Enter task name: ");
            string name = Console.ReadLine();

            Console.Write("Enter source directory: ");
            string source = Console.ReadLine();

            Console.Write("Enter target directory: ");
            string target = Console.ReadLine();

            Console.Write("Enter type (1 = Full, 2 = Differential): ");
            int typeInput = int.Parse(Console.ReadLine() ?? "1");
            BackupType type = typeInput == 1 ? BackupType.Full : BackupType.Differential;

            tasks.Add(new BackupTask(name, source, target, type));
            Console.WriteLine($"Backup task '{name}' created successfully.");
        }

        public void ExecuteSpecificTask()
        {
            Console.Write("Enter task number to execute (1, 2, ...): ");
            int taskNumber = int.Parse(Console.ReadLine() ?? "1");

            if (taskNumber < 1 || taskNumber > tasks.Count)
            {
                Console.WriteLine("Invalid task number.");
                return;
            }

            ExecuteBackup(tasks[taskNumber - 1]);
        }

        public void ExecuteAllTasks()
        {
            foreach (var task in tasks)
            {
                ExecuteBackup(task);
            }
        }

        private void ExecuteBackup(BackupTask task)
        {
            Console.WriteLine($"Executing backup: {task.Name}");

            if (!Directory.Exists(task.SourceDirectory))
            {
                Console.WriteLine($"Source directory '{task.SourceDirectory}' does not exist.");
                return;
            }

            var files = Directory.GetFiles(task.SourceDirectory, "*", SearchOption.AllDirectories);
            long totalSize = 0;
            foreach (var file in files)
            {
                totalSize += new FileInfo(file).Length;
            }

            long transferredSize = 0;
            int remainingFiles = files.Length;

            foreach (var file in files)
            {
                // Utilisation de la méthode personnalisée pour obtenir le chemin relatif
                string relativePath = PathHelper.GetRelativePath(task.SourceDirectory, file);
                string targetPath = Path.Combine(task.TargetDirectory, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                File.Copy(file, targetPath, true);

                transferredSize += new FileInfo(file).Length;
                remainingFiles--;

                logger.LogAction(task, file, targetPath);
                stateManager.UpdateState(task, remainingFiles, totalSize - transferredSize, file, targetPath);
            }

            Console.WriteLine($"Backup '{task.Name}' completed successfully.");
        }
    }
}
