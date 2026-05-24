using System;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;

namespace BLL
{
    public class BackupService
    {
        public void BackupDatabase(string backupFilePath)
        {
            if (string.IsNullOrWhiteSpace(backupFilePath)) throw new ArgumentException("Backup file path is required.");

            if (IsSqlServerConfigured())
            {
                BackupSqlServerDatabase(backupFilePath);
                return;
            }

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

            if (IsSqlServerConfigured())
            {
                RestoreSqlServerDatabase(backupFilePath);
                return;
            }

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
            var connectionString = ConfigurationManager.ConnectionStrings["SchoolDeviceStoreDB"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Missing SchoolDeviceStoreDB connection string.");

            foreach (var part in connectionString.Split(';'))
            {
                var item = part.Trim();
                if (item.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
                {
                    var value = item.Substring("Data Source=".Length).Trim().Trim('"');
                    var dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string ?? AppDomain.CurrentDomain.BaseDirectory;
                    value = value.Replace("|DataDirectory|", dataDirectory);
                    if (Path.IsPathRooted(value))
                        return value;

                    return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value));
                }
            }

            throw new InvalidOperationException("Unable to determine SQLite database file path from connection string.");
        }

        private static bool IsSqlServerConfigured()
        {
            var provider = ConfigurationManager.ConnectionStrings["SchoolDeviceStoreDB"]?.ProviderName ?? string.Empty;
            return provider.IndexOf("SqlClient", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void BackupSqlServerDatabase(string backupFilePath)
        {
            var cs = ConfigurationManager.ConnectionStrings["SchoolDeviceStoreDB"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("Missing SQL Server connection string.");

            var builder = new SqlConnectionStringBuilder(cs);
            var databaseName = builder.InitialCatalog;
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("SQL Server connection string must include Initial Catalog.");

            var backupDirectory = Path.GetDirectoryName(backupFilePath);
            if (!string.IsNullOrWhiteSpace(backupDirectory) && !Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            using (var conn = new SqlConnection(cs))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"BACKUP DATABASE [{databaseName}] TO DISK = @path WITH INIT, COPY_ONLY";
                cmd.Parameters.AddWithValue("@path", backupFilePath);
                cmd.ExecuteNonQuery();
            }
        }

        private static void RestoreSqlServerDatabase(string backupFilePath)
        {
            var cs = ConfigurationManager.ConnectionStrings["SchoolDeviceStoreDB"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("Missing SQL Server connection string.");

            var builder = new SqlConnectionStringBuilder(cs);
            var databaseName = builder.InitialCatalog;
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("SQL Server connection string must include Initial Catalog.");

            using (var conn = new SqlConnection(builder.ConnectionString.Replace($"Initial Catalog={databaseName}", "Initial Catalog=master")))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $@"
ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [{databaseName}] FROM DISK = @path WITH REPLACE;
ALTER DATABASE [{databaseName}] SET MULTI_USER;";
                cmd.Parameters.AddWithValue("@path", backupFilePath);
                cmd.ExecuteNonQuery();
            }
        }
    }
}