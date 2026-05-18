using System;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public static class UiDialogs
    {
        public static void ShowInfo(string message, string title = "Thông báo")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowWarning(string message, string title = "Cảnh báo")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ShowError(string message, string title = "Lỗi")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowSuccess(string message, string title = "Thành công")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool Confirm(string message, string title = "Xác nhận")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public static void RunSafe(Action action, string friendlyMessage, string logContext = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                AppLogger.Error(logContext ?? friendlyMessage, ex);
                ShowError(friendlyMessage);
            }
        }
    }
}
