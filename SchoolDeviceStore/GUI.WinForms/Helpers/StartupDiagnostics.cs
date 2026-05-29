using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Windows.Forms;
using BLL;
using DAL;

namespace GUI.WinForms
{
    public static class StartupDiagnostics
    {
        public static bool CheckDatabaseConnectivity()
        {
            try
            {
                var connectionString = DbHelper.GetConnectionString();
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    AppLogger.Error("Database connection string is missing or empty.");
                    UiDialogs.ShowError(
                        "Database configuration is missing.\n\n" +
                        "The application cannot connect to the database because the connection string is not configured.\n\n" +
                        "Please check the configuration file and try again.",
                        "Database Configuration Error");
                    return false;
                }

                using (var connection = CreateConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                }

                AppLogger.Info("Database connectivity check passed.");
                return true;
            }
            catch (SQLiteException sqliteEx)
            {
                AppLogger.Error("SQLite database connection failed", sqliteEx);
                UiDialogs.ShowError(
                    "Unable to connect to the database.\n\n" +
                    "The application could not establish a connection to the SQLite database.\n\n" +
                    "Error: " + sqliteEx.Message + "\n\n" +
                    "Please ensure the database file exists and is accessible.",
                    "Database Connection Error");
                return false;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Unexpected error during database connectivity check", ex);
                UiDialogs.ShowError(
                    "An unexpected error occurred while checking the database.\n\n" +
                    "Error: " + ex.Message,
                    "Startup Error");
                return false;
            }
        }

        public static bool EnsureApplicationState()
        {
            try
            {
                AppLogger.Info("Initializing application state...");

                var dbSetup = new BLL.DatabaseSetupService();
                dbSetup.EnsureDemoDatabaseReady();
                AppLogger.Info("Database initialization completed.");

                var authService = new BLL.AuthService();
                authService.EnsureAdminExists();
                AppLogger.Info("Admin user bootstrap completed.");

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Application state initialization failed", ex);
                UiDialogs.ShowError(
                    "Unable to initialize the application.\n\n" +
                    "The application failed to set up the database and admin user.\n\n" +
                    "Error: " + ex.Message,
                    "Initialization Error");
                return false;
            }
        }

        private static DbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }
    }
}
