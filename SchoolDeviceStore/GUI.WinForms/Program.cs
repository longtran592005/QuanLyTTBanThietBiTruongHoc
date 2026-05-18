using System;
using System.Windows.Forms;

namespace GUI.WinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                AppLogger.UnhandledException(e.Exception, "UI thread exception");
                UiDialogs.ShowError("An unexpected error occurred. The problem has been logged.", "Application Error");
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = e.ExceptionObject as Exception;
                AppLogger.UnhandledException(exception ?? new Exception("Unknown unhandled exception"), "App domain exception");
            };

            AppLogger.Info("=== Application Startup ===");

            if (!StartupDiagnostics.CheckDatabaseConnectivity())
            {
                AppLogger.Error("Startup aborted: Database connectivity check failed.");
                return;
            }

            if (!StartupDiagnostics.EnsureApplicationState())
            {
                AppLogger.Error("Startup aborted: Application state initialization failed.");
                return;
            }

            try
            {
                AppLogger.Info("Launching LoginForm...");
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                AppLogger.UnhandledException(ex, "Fatal application error");
                UiDialogs.ShowError("The application encountered a fatal error and must close.\n\nPlease check the log file for details.", "Fatal Error");
            }
            finally
            {
                AppLogger.Info("=== Application Shutdown ===");
            }
        }
    }
}
