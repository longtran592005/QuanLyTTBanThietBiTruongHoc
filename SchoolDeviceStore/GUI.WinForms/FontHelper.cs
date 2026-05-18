using System.Drawing;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public static class FontHelper
    {
        private static Font _vietnameseFont;
        private static Font _vietnameseFontBold;

        static FontHelper()
        {
            try
            {
                // Segoe UI is the best font for Vietnamese support on Windows
                _vietnameseFont = new Font("Segoe UI", 10f, FontStyle.Regular);
                _vietnameseFontBold = new Font("Segoe UI", 10f, FontStyle.Bold);
            }
            catch
            {
                // Fallback to Tahoma if Segoe UI is not available
                _vietnameseFont = new Font("Tahoma", 10f, FontStyle.Regular);
                _vietnameseFontBold = new Font("Tahoma", 10f, FontStyle.Bold);
            }
        }

        public static Font GetVietnameseFont()
        {
            return _vietnameseFont;
        }

        public static Font GetVietnameseFontBold()
        {
            return _vietnameseFontBold;
        }

        public static void ApplyVietnameseFontToForm(Form form)
        {
            form.Font = _vietnameseFont;
            ApplyFontToControls(form.Controls);
        }

        private static void ApplyFontToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                control.Font = _vietnameseFont;

                if (control is Button button)
                {
                    button.Font = _vietnameseFont;
                }
                else if (control is Label label)
                {
                    label.Font = _vietnameseFont;
                }
                else if (control is TextBox textBox)
                {
                    textBox.Font = _vietnameseFont;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.Font = _vietnameseFont;
                }
                else if (control is DataGridView dataGridView)
                {
                    dataGridView.Font = _vietnameseFont;
                    dataGridView.DefaultCellStyle.Font = _vietnameseFont;
                    dataGridView.ColumnHeadersDefaultCellStyle.Font = _vietnameseFontBold;
                }
                else if (control is MenuStrip menuStrip)
                {
                    menuStrip.Font = _vietnameseFont;
                    foreach (ToolStripItem item in menuStrip.Items)
                    {
                        item.Font = _vietnameseFont;
                    }
                }

                if (control.HasChildren)
                {
                    ApplyFontToControls(control.Controls);
                }
            }
        }
    }
}
