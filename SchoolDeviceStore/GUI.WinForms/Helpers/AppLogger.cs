using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GUI.WinForms
{
    public static class AppLogger
    {
        private static readonly object SyncRoot = new object();
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly string LogFilePath = Path.Combine(LogDirectory, "application.log");

        public static void Info(string message)
        {
            Write("INFO", message, null);
        }

        public static void Warn(string message)
        {
            Write("WARN", message, null);
        }

        public static void Error(string message, Exception exception = null)
        {
            Write("ERROR", message, exception);
        }

        public static void UnhandledException(Exception exception, string context = null)
        {
            var message = string.IsNullOrWhiteSpace(context) ? exception.Message : context;
            Write("FATAL", message, exception);
        }

        private static void Write(string level, string message, Exception exception)
        {
            try
            {
                lock (SyncRoot)
                {
                    Directory.CreateDirectory(LogDirectory);
                    var builder = new StringBuilder();
                    builder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    builder.Append(" [").Append(level).Append("] ");
                    builder.Append(message ?? string.Empty);

                    if (exception != null)
                    {
                        builder.AppendLine();
                        builder.AppendLine(exception.ToString());
                    }

                    builder.AppendLine();
                    File.AppendAllText(LogFilePath, builder.ToString(), Encoding.UTF8);
                }
            }
            catch
            {
                Debug.WriteLine($"[{level}] {message}");
                if (exception != null)
                {
                    Debug.WriteLine(exception);
                }
            }
        }
    }
}
