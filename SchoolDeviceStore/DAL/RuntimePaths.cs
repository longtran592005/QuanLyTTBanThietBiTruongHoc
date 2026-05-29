using System;
using System.IO;

namespace DAL
{
    public static class RuntimePaths
    {
        private const string AppFolderName = "SchoolDeviceStore";

        public static string GetDataDirectory()
        {
            var dataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppFolderName,
                "Data");

            Directory.CreateDirectory(dataDir);
            return dataDir;
        }

        public static string GetLogsDirectory()
        {
            var logsDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppFolderName,
                "Logs");

            Directory.CreateDirectory(logsDir);
            return logsDir;
        }
    }
}