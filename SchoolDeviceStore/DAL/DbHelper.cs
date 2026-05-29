using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace DAL
{
    /// <summary>
    /// ADO.NET helper for the SQLite-only deployment path.
    /// </summary>
    public static class DbHelper
    {
        private static string ConnectionString
        {
            get
            {
                try
                {
                    var cs = ConfigurationManager.ConnectionStrings["SchoolDeviceStoreDB"];
                    if (cs != null)
                    {
                        var configured = cs.ConnectionString;
                        if (!string.IsNullOrWhiteSpace(configured))
                            return ResolveConnectionString(configured);
                    }
                }
                catch
                {
                    // Fall through to default.
                }

                return ResolveConnectionString("Data Source=|DataDirectory|\\SchoolDeviceStore.db;Version=3;");
            }
        }

        private static string ResolveConnectionString(string connectionString)
        {
            var connStr = connectionString;
            if (connStr.IndexOf("|DataDirectory|", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                connStr = connStr.Replace("|DataDirectory|", ResolveDataDirectory());
            }

            return connStr;
        }

        private static string ResolveDataDirectory()
        {
            return RuntimePaths.GetDataDirectory();
        }

        public static string GetConnectionString()
        {
            return ConnectionString;
        }

        public static bool IsSQLite()
        {
            return true;
        }

        public static DataTable ExecuteQuery(string sql, params DbParameter[] parameters)
        {
            var dt = new DataTable();

            using (var conn = new SQLiteConnection(ConnectionString))
            using (var cmd = new SQLiteCommand(sql, conn))
            using (var da = new SQLiteDataAdapter(cmd))
            {
                AddParameters(cmd, parameters);
                conn.Open();
                da.Fill(dt);
            }

            return dt;
        }

        public static int ExecuteNonQuery(string sql, params DbParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                AddParameters(cmd, parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params DbParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                AddParameters(cmd, parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        private static void AddParameters(DbCommand command, DbParameter[] parameters)
        {
            if (parameters == null)
                return;

            foreach (var parameter in parameters)
            {
                if (parameter == null)
                    continue;

                command.Parameters.Add(parameter);
            }
        }
    }
}