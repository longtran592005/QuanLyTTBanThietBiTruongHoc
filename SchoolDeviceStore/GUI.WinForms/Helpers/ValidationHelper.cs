using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public static class ValidationHelper
    {
        public static string RequireText(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException($"{fieldName} is required.");
            }

            return value.Trim();
        }

        public static int RequirePositiveInt(string value, string fieldName)
        {
            if (!int.TryParse(value, out var parsed) || parsed <= 0)
            {
                throw new ValidationException($"{fieldName} must be a positive number.");
            }

            return parsed;
        }

        public static int RequireNonNegativeInt(string value, string fieldName)
        {
            if (!int.TryParse(value, out var parsed) || parsed < 0)
            {
                throw new ValidationException($"{fieldName} must be zero or greater.");
            }

            return parsed;
        }

        public static decimal RequireCurrency(string value, string fieldName)
        {
            if (!decimal.TryParse(value, out var parsed) || parsed < 0m)
            {
                throw new ValidationException($"{fieldName} must be a valid currency amount.");
            }

            return parsed;
        }

        public static void SetError(Control control, ErrorProvider provider, string message)
        {
            if (provider != null)
            {
                provider.SetError(control, message);
            }
        }

        public static void ClearError(Control control, ErrorProvider provider)
        {
            if (provider != null)
            {
                provider.SetError(control, string.Empty);
            }
        }

        public static bool ConfirmOperation(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        public static string BuildDuplicateMessage(string fieldName, string value)
        {
            return $"A record with {fieldName} '{value}' already exists.";
        }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}
