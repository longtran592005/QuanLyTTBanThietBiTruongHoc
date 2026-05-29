using System;
using System.IO;
using DAL;

namespace BLL
{
    public class BackupService
    {
        public void BackupDatabase(string backupFilePath)
        {
            if (string.IsNullOrWhiteSpace(backupFilePath)) throw new ArgumentException("Backup file path is required.");

            var databaseFilePath = GetDatabaseFilePath();
            if (!File.Exists(databaseFilePath))
                throw new FileNotFoundException("SQLite database file not found.", databaseFilePath);

            var backupDirectory = Path.GetDirectoryName(backupFilePath);
            if (!string.IsNullOrWhiteSpace(backupDirectory) && !Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            File.Copy(databaseFilePath, backupFilePath, true);
        }

        public void RestoreDatabase(string backupFilePath)
        {
            if (string.IsNullOrWhiteSpace(backupFilePath)) throw new ArgumentException("Backup file path is required.");
            if (!File.Exists(backupFilePath)) throw new FileNotFoundException("Backup file not found.", backupFilePath);

            var databaseFilePath = GetDatabaseFilePath();
            var databaseDirectory = Path.GetDirectoryName(databaseFilePath);
            if (!string.IsNullOrWhiteSpace(databaseDirectory) && !Directory.Exists(databaseDirectory))
                Directory.CreateDirectory(databaseDirectory);

            if (File.Exists(databaseFilePath))
            {
                File.Delete(databaseFilePath);
            }

            File.Copy(backupFilePath, databaseFilePath, true);
        }

        private static string GetDatabaseFilePath()
        {
            var connectionString = DbHelper.GetConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Missing SchoolDeviceStoreDB connection string.");

            foreach (var part in connectionString.Split(';'))
            {
                var item = part.Trim();
                if (item.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
                {
                    var value = item.Substring("Data Source=".Length).Trim().Trim('"');
                    var dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string ?? RuntimePaths.GetDataDirectory();
                    value = value.Replace("|DataDirectory|", dataDirectory);
                    if (Path.IsPathRooted(value))
                        return value;

                    return Path.GetFullPath(Path.Combine(dataDirectory, value));
                }
            }

            throw new InvalidOperationException("Unable to determine SQLite database file path from connection string.");
        }
    }
}