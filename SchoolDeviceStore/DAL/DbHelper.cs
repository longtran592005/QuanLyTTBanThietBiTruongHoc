using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SqlClient;

namespace DAL
{
    /// <summary>
    /// ADO.NET helper that supports both SQLite and SQL Server.
    /// The active provider is taken from App.config, with SQLite fallback for older setups.
    /// </summary>
    public static class DbHelper
    {
        private const string ProviderKey = "SchoolDeviceStoreDB_Provider";

        private static string ConnectionString
        {
            get
            {
                string connStr = null;
                string providerName = null;

                try
                {
                    var cs = ConfigurationManager.ConnectionStrings["SchoolDeviceStoreDB"];
                    if (cs != null)
                    {
                        connStr = cs.ConnectionString;
                        providerName = cs.ProviderName;
                    }
                }
                catch
                {
                    // Fall through to default.
                }

                if (string.IsNullOrWhiteSpace(connStr))
                    connStr = "Data Source=|DataDirectory|\\SchoolDeviceStore.db;Version=3;";

                if (connStr.IndexOf("|DataDirectory|", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    connStr = connStr.Replace("|DataDirectory|", ResolveDataDirectory());
                }

                if (string.IsNullOrWhiteSpace(providerName))
                {
                    providerName = InferProviderFromConnectionString(connStr);
                }

                AppDomain.CurrentDomain.SetData(ProviderKey, providerName);
                return connStr;
            }
        }

        private static string ResolveDataDirectory()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (baseDir.IndexOf("DbQuery", StringComparison.OrdinalIgnoreCase) >= 0)
                baseDir = baseDir.Replace("DbQuery", "GUI.WinForms");
            else if (baseDir.IndexOf("Tests", StringComparison.OrdinalIgnoreCase) >= 0)
                baseDir = baseDir.Replace("Tests", "GUI.WinForms");

            if (!System.IO.Directory.Exists(baseDir))
                return AppDomain.CurrentDomain.BaseDirectory;

            return baseDir;
        }

        private static string InferProviderFromConnectionString(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
                return "System.Data.SQLite";

            if (connStr.IndexOf("Initial Catalog=", StringComparison.OrdinalIgnoreCase) >= 0 ||
                connStr.IndexOf("Integrated Security=", StringComparison.OrdinalIgnoreCase) >= 0 ||
                connStr.IndexOf("Trusted_Connection=", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "System.Data.SqlClient";
            }

            return "System.Data.SQLite";
        }

        private static string GetProviderName()
        {
            var provider = AppDomain.CurrentDomain.GetData(ProviderKey) as string;
            return string.IsNullOrWhiteSpace(provider) ? InferProviderFromConnectionString(ConnectionString) : provider;
        }

        public static string GetConnectionString()
        {
            return ConnectionString;
        }

        public static DataTable ExecuteQuery(string sql, params DbParameter[] parameters)
        {
            var provider = GetProviderName();
            var dt = new DataTable();

            if (provider.IndexOf("SQLite", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var da = new SQLiteDataAdapter(cmd))
                {
                    AddParameters(cmd, parameters);
                    conn.Open();
                    da.Fill(dt);
                }
            }
            else
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(sql, conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    AddParameters(cmd, parameters);
                    conn.Open();
                    da.Fill(dt);
                }
            }

            return dt;
        }

        public static int ExecuteNonQuery(string sql, params DbParameter[] parameters)
        {
            var provider = GetProviderName();

            if (provider.IndexOf("SQLite", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AddParameters(cmd, parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                AddParameters(cmd, parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params DbParameter[] parameters)
        {
            var provider = GetProviderName();

            if (provider.IndexOf("SQLite", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AddParameters(cmd, parameters);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
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

                if (command is SqlCommand sqlCommand && !(parameter is SqlParameter))
                {
                    sqlCommand.Parameters.Add(ConvertToSqlParameter(parameter));
                }
                else
                {
                    command.Parameters.Add(parameter);
                }
            }
        }

        private static SqlParameter ConvertToSqlParameter(DbParameter parameter)
        {
            var sqlParameter = new SqlParameter(parameter.ParameterName, parameter.Value ?? DBNull.Value)
            {
                Direction = parameter.Direction
            };

            if (parameter.DbType != DbType.Object)
            {
                sqlParameter.DbType = parameter.DbType;
            }

            return sqlParameter;
        }
    }
}