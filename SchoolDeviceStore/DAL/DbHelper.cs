using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace DAL
{
    /// <summary>
    /// Simple ADO.NET helper for SQLite demo database.
    /// Uses parameterized queries and returns DataTable / scalar / rows affected.
    /// Connection string is read from App.config with name "SchoolDeviceStoreDB".
    /// </summary>
    public static class DbHelper
    {
        private static string ConnectionString
        {
            get
            {
                string connStr = null;
                try
                {
                    var cs = ConfigurationManager.ConnectionStrings["SchoolDeviceStoreDB"];
                    if (cs != null && !string.IsNullOrEmpty(cs.ConnectionString))
                        connStr = cs.ConnectionString;
                }
                catch
                {
                    // Fall through to default
                }
                
                if (string.IsNullOrEmpty(connStr))
                    connStr = "Data Source=|DataDirectory|\\SchoolDeviceStore.db;Version=3;";

                Console.WriteLine($"[DEBUG] ConnectionString property - Before resolution: {connStr}");
                
                // Resolve |DataDirectory| token for SQLite
                if (connStr.Contains("|DataDirectory|"))
                {
                    string dataDir = ResolveDataDirectory();
                    connStr = connStr.Replace("|DataDirectory|", dataDir);
                    Console.WriteLine($"[DEBUG] ConnectionString property - After resolution: {connStr}");
                }

                return connStr;
            }
        }

        /// <summary>
        /// Resolve |DataDirectory| to a consistent location across all projects.
        /// Always uses the GUI.WinForms bin\Debug\net48 directory for the shared database.
        /// </summary>
        private static string ResolveDataDirectory()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"[DEBUG] ResolveDataDirectory() - baseDir: {baseDir}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] ResolveDataDirectory() - baseDir: {baseDir}");
            
            // If running from DbQuery or other projects, replace with GUI.WinForms path
            if (baseDir.Contains("DbQuery"))
            {
                string newPath = baseDir.Replace("DbQuery", "GUI.WinForms");
                Console.WriteLine($"[DEBUG] Detected DbQuery, replacing: {baseDir} -> {newPath}");
                baseDir = newPath;
            }
            else if (baseDir.Contains("Tests"))
            {
                string newPath = baseDir.Replace("Tests", "GUI.WinForms");
                Console.WriteLine($"[DEBUG] Detected Tests, replacing: {baseDir} -> {newPath}");
                baseDir = newPath;
            }
            
            // Ensure directory exists, if not use current directory
            if (!System.IO.Directory.Exists(baseDir))
            {
                Console.WriteLine($"[DEBUG] Resolved path doesn't exist: {baseDir}, using current: {AppDomain.CurrentDomain.BaseDirectory}");
                return AppDomain.CurrentDomain.BaseDirectory;
            }
            
            Console.WriteLine($"[DEBUG] Final resolved directory: {baseDir}");
            return baseDir;
        }

        public static string GetConnectionString()
        {
            return ConnectionString;
        }

        public static DataTable ExecuteQuery(string sql, params SQLiteParameter[] parameters)
        {
            var dt = new DataTable();
            using (var conn = new SQLiteConnection(ConnectionString))
            using (var cmd = new SQLiteCommand(sql, conn))
            using (var da = new SQLiteDataAdapter(cmd))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                da.Fill(dt);
            }
            return dt;
        }

        public static int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
