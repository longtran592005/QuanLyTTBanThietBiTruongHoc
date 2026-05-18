using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public static class UIHelper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public static void SetPlaceholder(TextBox textBox, string placeholder)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, placeholder);
        }

        public static void ApplyFormTheme(Form form)
        {
            form.BackColor = UITheme.BackgroundColor;
            form.Font = UITheme.BaseFont;
        }

        public static Button CreateSidebarButton(string text, string iconChar)
        {
            var button = CreateFlatButton(text, UITheme.SidebarColor, UITheme.TextPrimaryColor);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.Padding = new Padding(12, 0, 16, 0); // Clean border gap
            button.Margin = new Padding(0, 0, 0, 4);
            button.Height = 44;
            button.Font = UITheme.BaseFont;
            button.AutoEllipsis = true;
            button.AccessibleDescription = iconChar;
            
            button.Image = CreateIconImage(iconChar, UITheme.TextPrimaryColor, 20);

            button.MouseEnter += (s, e) =>
            {
                if (!(button.Tag is bool active && active))
                {
                    button.BackColor = UITheme.SidebarHoverColor;
                }
            };
            button.MouseLeave += (s, e) =>
            {
                if (button.Tag is bool active && active)
                {
                    button.BackColor = UITheme.SelectionColor;
                }
                else
                {
                    button.BackColor = UITheme.SidebarColor;
                }
            };
            
            button.Paint += (s, e) =>
            {
                if (button.Tag is bool active && active)
                {
                    using (var brush = new SolidBrush(UITheme.PrimaryColor))
                    {
                        e.Graphics.FillRectangle(brush, 0, 4, 4, button.Height - 8);
                    }
                }
            };
            
            return button;
        }

        public static Button CreateSidebarDangerButton(string text, string iconChar)
        {
            var button = CreateFlatButton(text, UITheme.SidebarColor, UITheme.ErrorColor);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.Padding = new Padding(12, 0, 16, 0);
            button.Height = 44;
            button.Font = UITheme.BodyBoldFont;
            button.AutoEllipsis = true;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(253, 235, 236);
            
            button.Image = CreateIconImage(iconChar, UITheme.ErrorColor, 20);
            return button;
        }

        public static Button CreatePrimaryButton(string text)
        {
            return CreateActionButton(text, UITheme.PrimaryColor, Color.White);
        }

        public static Button CreateSecondaryButton(string text)
        {
            Color lightGrayBorder = Color.FromArgb(209, 213, 219); // #D1D5DB
            Color darkGrayText = Color.FromArgb(55, 65, 81); // #374151
            return CreateActionButton(text, UITheme.SurfaceColor, darkGrayText, lightGrayBorder);
        }

        public static Button CreateDangerButton(string text)
        {
            return CreateActionButton(text, UITheme.ErrorColor, Color.White);
        }

        public static Button CreateOutlineDangerButton(string text)
        {
            return CreateActionButton(text, UITheme.SurfaceColor, UITheme.ErrorColor, UITheme.ErrorColor);
        }

        public static Button CreateWarningButton(string text)
        {
            return CreateActionButton(text, UITheme.WarningColor, Color.White);
        }

        public static void StyleTextBox(Control control)
        {
            control.Font = UITheme.BaseFont;
            control.BackColor = Color.White;

            if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Margin = new Padding(0);
                textBox.Height = 36;
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.FlatStyle = FlatStyle.Flat;
                comboBox.Margin = new Padding(0);
            }
            else if (control is NumericUpDown numericUpDown)
            {
                numericUpDown.BorderStyle = BorderStyle.FixedSingle;
                numericUpDown.Margin = new Padding(0);
            }
        }

        public static void StyleLabel(Label label, bool secondary = false)
        {
            label.Font = secondary ? UITheme.CaptionFont : UITheme.BaseFont;
            label.ForeColor = secondary ? UITheme.TextSecondaryColor : UITheme.TextPrimaryColor;
        }

        public static void StyleCard(Control control)
        {
            control.BackColor = UITheme.SurfaceColor;
            
            control.Resize += (s, e) =>
            {
                if (control.Width > 1 && control.Height > 1)
                {
                    var rect = new Rectangle(0, 0, control.Width, control.Height);
                    using (var path = GetRoundedPath(rect, UITheme.BorderRadius))
                    {
                        var oldRegion = control.Region;
                        control.Region = new Region(path);
                        oldRegion?.Dispose();
                    }
                }
            };

            control.Paint += (s, e) =>
            {
                if (control.Width <= 1 || control.Height <= 1)
                    return;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, control.Width - 1, control.Height - 1);

                using (var path = GetRoundedPath(rect, UITheme.BorderRadius))
                {
                    using (var pen = new Pen(UITheme.BorderColor))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };
        }

        public static void StyleDataGridView(DataGridView grid)
        {
            grid.BackgroundColor = UITheme.SurfaceColor;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = UITheme.BorderColor;
            grid.EnableHeadersVisualStyles = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.RowTemplate.Height = 44;
            grid.ColumnHeadersHeight = 48;
            grid.DefaultCellStyle.BackColor = UITheme.SurfaceColor;
            grid.DefaultCellStyle.ForeColor = UITheme.TextPrimaryColor;
            grid.DefaultCellStyle.SelectionBackColor = UITheme.SelectionColor;
            grid.DefaultCellStyle.SelectionForeColor = UITheme.TextPrimaryColor;
            grid.DefaultCellStyle.Font = UITheme.BaseFont;
            grid.DefaultCellStyle.Padding = new Padding(12, 6, 12, 6);
            grid.AlternatingRowsDefaultCellStyle.BackColor = UITheme.TableAlternateColor;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = UITheme.SelectionColor;
            grid.ColumnHeadersDefaultCellStyle.BackColor = UITheme.HoverFillColor; // #F3F4F6
            grid.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.TextPrimaryColor;
            grid.ColumnHeadersDefaultCellStyle.Font = UITheme.BodyBoldFont;
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 12, 12, 12);
            grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            grid.AdvancedColumnHeadersBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedColumnHeadersBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedColumnHeadersBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedColumnHeadersBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;
            grid.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            grid.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;
        }

        public static void StyleActionButton(Button button, string text, Color backColor)
        {
            button.Text = text;
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = UITheme.BodyBoldFont;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor);
            button.Cursor = Cursors.Hand;
        }

        public static void SetSidebarButtonState(Button button, bool active)
        {
            if (button == null)
                return;

            button.Tag = active;
            button.BackColor = active ? UITheme.SelectionColor : UITheme.SidebarColor;
            button.ForeColor = active ? UITheme.PrimaryColor : UITheme.TextPrimaryColor;
            
            if (active)
            {
                button.Font = UITheme.BodyBoldFont;
            }
            else
            {
                button.Font = UITheme.BaseFont;
            }
            
            if (!string.IsNullOrEmpty(button.AccessibleDescription))
            {
                var oldImg = button.Image;
                button.Image = CreateIconImage(button.AccessibleDescription, button.ForeColor, 20);
                oldImg?.Dispose();
            }
            
            button.Invalidate();
        }

        private static Button CreateActionButton(string text, Color backColor, Color foreColor, Color borderColor = default(Color))
        {
            var button = new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Height = 36,
                MinimumSize = new Size(100, 36),
                AutoSize = true,
                Font = UITheme.BodyBoldFont,
                Margin = new Padding(0, 0, 8, 0), // 8px gap between buttons
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            
            // Hover states logic (Micro-Interaction Rule)
            if (backColor == UITheme.PrimaryColor)
            {
                button.FlatAppearance.MouseOverBackColor = UITheme.PrimaryHoverColor;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 64, 175); // Darker blue
            }
            else if (backColor == UITheme.ErrorColor)
            {
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 38, 38); // Red 600
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(185, 28, 28); // Red 700
            }
            else if (foreColor == UITheme.ErrorColor) // Outline Danger
            {
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(254, 242, 242); // Soft red
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(254, 226, 226); // Slightly darker
            }
            else // Secondary
            {
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(243, 244, 246); // Soft gray
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(229, 231, 235); // Gray 200
            }

            // Subtle border radius using Resize & Paint
            button.Resize += (s, e) =>
            {
                if (button.Width > 1 && button.Height > 1)
                {
                    var rect = new Rectangle(0, 0, button.Width, button.Height);
                    using (var path = GetRoundedPath(rect, 6)) // 6px subtle border radius
                    {
                        var oldRegion = button.Region;
                        button.Region = new Region(path);
                        oldRegion?.Dispose();
                    }
                }
            };

            button.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, button.Width - 1, button.Height - 1);
                using (var path = GetRoundedPath(rect, 6)) // Smooth the edges via drawpath
                {
                    Color borderC = (borderColor == default(Color)) ? backColor : borderColor;
                    if (borderC != Color.Transparent && borderC != UITheme.BackgroundColor)
                    {
                        using (var pen = new Pen(borderC, 1.5f)) // Draw border
                        {
                            e.Graphics.DrawPath(pen, path);
                        }
                    }
                }
            };
            
            return button;
        }

        private static Button CreateFlatButton(string text, Color backColor, Color foreColor)
        {
            var button = new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Height = 44,
                AutoSize = false,
                Dock = DockStyle.Top,
                Margin = new Padding(0),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = UITheme.SidebarHoverColor;
            return button;
        }

        private static Image CreateIconImage(string iconChar, Color color, int size = 20)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                using (var font = new Font("Segoe UI Symbol", 12F, FontStyle.Regular))
                using (var brush = new SolidBrush(color))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    // Add space after icon in image to create a gap if TextImageRelation gap isn't enough, 
                    // but we will use Padding instead.
                    g.DrawString(iconChar, font, brush, new RectangleF(0, 0, size, size), sf);
                }
            }
            return bmp;
        }

        private static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
