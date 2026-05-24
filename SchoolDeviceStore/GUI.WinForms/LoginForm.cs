using System;
using System.Drawing;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public class LoginForm : Form
    {
        private TextBox _usernameBox;
        private TextBox _passwordBox;
        private Label _statusLabel;
        private CheckBox _showPasswordCheckBox;

        public LoginForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "Đăng nhập - Quản lý thiết bị trường học";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(980, 620);
            Size = new Size(1100, 700);
            BackColor = UITheme.BackgroundColor;
            Font = UITheme.BaseFont;
            DoubleBuffered = true;
            ControlBox = false; // Disable title bar buttons as requested

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UITheme.BackgroundColor
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));

            root.Controls.Add(BuildBrandPanel(), 0, 0);
            root.Controls.Add(BuildLoginPanel(), 1, 0);
            Controls.Add(root);
        }

        private Control BuildBrandPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.SidebarColor, // Fallback
                Padding = new Padding(48)
            };

            // Add smooth gradient and floating geometries (10/10 UX)
            panel.Paint += (s, e) =>
            {
                // Smooth gradient
                var rect = panel.ClientRectangle;
                if (rect.Width > 0 && rect.Height > 0)
                {
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, Color.FromArgb(255, 255, 255), Color.FromArgb(242, 246, 252), System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }

                    // Floating geometries (Academic/Tech theme)
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (var pen = new Pen(Color.FromArgb(12, UITheme.PrimaryColor), 2f)) // Extremely subtle blue
                    {
                        e.Graphics.DrawEllipse(pen, -100, -100, 400, 400); // Top left circle
                        e.Graphics.DrawEllipse(pen, rect.Width - 150, 80, 200, 200); // Right middle circle
                        
                        using (var path = GetRoundedPath(new Rectangle(80, rect.Height - 200, 120, 120), 20))
                        {
                            e.Graphics.DrawPath(pen, path); // Bottom left rounded square
                        }
                    }

                    using (var fillBrush = new SolidBrush(Color.FromArgb(8, UITheme.PrimaryColor)))
                    {
                        e.Graphics.FillEllipse(fillBrush, rect.Width / 2, rect.Height - 150, 300, 300); // Bottom right solid blob
                    }
                }
            };
            panel.Resize += (s, e) => panel.Invalidate();

            // 1. Title
            var title = new Label
            {
                Dock = DockStyle.Top,
                Height = 74,
                ForeColor = UITheme.TextPrimaryColor,
                Font = new Font(UITheme.TitleFont.FontFamily, 32F, FontStyle.Bold),
                Text = "Quản lý thiết bị",
                Margin = new Padding(0, 0, 0, 16),
                BackColor = Color.Transparent
            };

            // 2. Subtitle
            var subtitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 64,
                ForeColor = UITheme.SidebarMutedTextColor,
                Font = new Font(UITheme.BaseFont.FontFamily, 13F, FontStyle.Regular),
                Text = "Phần mềm quản lý thiết bị trường học, bán hàng, báo cáo\nvà vận hành hiện đại.",
                BackColor = Color.Transparent
            };

            // 3. Feature Cards Layout
            var cardsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 120,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0, 24, 0, 16),
                BackColor = Color.Transparent
            };
            cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            cardsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

            cardsLayout.Controls.Add(BuildFeatureCard("\uE2B4", "Giao diện quản trị\nthống nhất"), 0, 0); // Dashboard icon
            cardsLayout.Controls.Add(BuildFeatureCard("\uE192", "Phân quyền linh\nhoạt chuẩn xác"), 1, 0); // Key icon
            cardsLayout.Controls.Add(BuildFeatureCard("\uE9D9", "Báo cáo thống kê\nthời gian thực"), 2, 0); // Chart icon

            // 4. Illustration
            var illustration = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 16, 0, 0)
            };

            try
            {
                string imgPath = System.IO.Path.Combine(Application.StartupPath, "Assets", "login_illustration.png");
                if (System.IO.File.Exists(imgPath))
                {
                    illustration.Image = Image.FromFile(imgPath);
                }
                else
                {
                    string sourcePath = System.IO.Path.Combine(Application.StartupPath, "..", "..", "..", "Assets", "login_illustration.png");
                    if (System.IO.File.Exists(sourcePath))
                        illustration.Image = Image.FromFile(sourcePath);
                }
            }
            catch { /* Keep empty if missing */ }

            panel.Controls.Add(illustration);
            panel.Controls.Add(cardsLayout);
            panel.Controls.Add(subtitle);
            panel.Controls.Add(title);
            return panel;
        }

        private Control BuildFeatureCard(string iconChar, string text)
        {
            var card = new Panel 
            { 
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 0, 4, 0),
                BackColor = Color.FromArgb(250, 252, 255) // Very light blue tint
            };

            card.Resize += (s, e) =>
            {
                if (card.Width > 1 && card.Height > 1)
                {
                    var rect = new Rectangle(0, 0, card.Width, card.Height);
                    using (var path = GetRoundedPath(rect, 12))
                    {
                        var oldRegion = card.Region;
                        card.Region = new Region(path);
                        oldRegion?.Dispose();
                    }
                }
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                using (var path = GetRoundedPath(rect, 12)) // Softer 12px border for cards
                {
                    using (var pen = new Pen(Color.FromArgb(180, 200, 230), 1.5f)) // Primary blue-tinted border
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            var iconLabel = new Label
            {
                Text = iconChar,
                Font = new Font("Segoe MDL2 Assets", 20F, FontStyle.Regular),
                ForeColor = UITheme.PrimaryColor,
                Dock = DockStyle.Top,
                Height = 44,
                TextAlign = ContentAlignment.BottomCenter,
                BackColor = Color.Transparent
            };

            var textLabel = new Label
            {
                Text = text,
                Font = new Font(UITheme.BaseFont.FontFamily, 8.5F, FontStyle.Regular),
                ForeColor = UITheme.TextPrimaryColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
                BackColor = Color.Transparent,
                Padding = new Padding(2)
            };

            card.Controls.Add(textLabel);
            card.Controls.Add(iconLabel);

            // Handle Resize triggers repaint
            card.Resize += (s, e) => card.Invalidate();

            return card;
        }

        private Control BuildLoginPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.BackgroundColor,
                Padding = new Padding(48)
            };

            var card = new Panel
            {
                Width = 440,
                Height = 560,
                BackColor = UITheme.SurfaceColor,
                Padding = new Padding(32)
            };
            UIHelper.StyleCard(card);

            // Add Soft Drop Shadow
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var shadowColor = Color.FromArgb(20, 0, 0, 0); // Very light shadow
                for (int i = 1; i <= 8; i++)
                {
                    using (var pen = new Pen(Color.FromArgb(shadowColor.A / i, shadowColor.R, shadowColor.G, shadowColor.B), 2))
                    {
                        var rect = new Rectangle(card.Left - i, card.Top - i + 2, card.Width + i * 2, card.Height + i * 2);
                        e.Graphics.DrawPath(pen, GetRoundedPath(rect, UITheme.BorderRadius + i / 2));
                    }
                }
            };

            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 10,
                Margin = new Padding(0)
            };
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));  // Heading
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));  // Body text
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // User Label
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));  // Username
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // Pass Label
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));  // Password
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));  // Show password checkbox
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));  // Login button (spaced)
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));  // Exit button
            container.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));  // Status label

            var heading = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Đăng nhập",
                Font = UITheme.TitleFont,
                ForeColor = UITheme.TextPrimaryColor
            };
            var body = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Dùng tài khoản được cấp để truy cập hệ thống.",
                Font = UITheme.CaptionFont,
                ForeColor = UITheme.TextSecondaryColor
            };

            var userLabel = new Label { Dock = DockStyle.Fill, Text = "Tên đăng nhập", Font = UITheme.CaptionFont, ForeColor = UITheme.TextPrimaryColor, TextAlign = ContentAlignment.BottomLeft };
            var passLabel = new Label { Dock = DockStyle.Fill, Text = "Mật khẩu", Font = UITheme.CaptionFont, ForeColor = UITheme.TextPrimaryColor, TextAlign = ContentAlignment.BottomLeft };

            _usernameBox = new TextBox { Visible = false }; // Keep for compatibility with existing _usernameBox ref
            var userRounded = new RoundedTextBox { Dock = DockStyle.Fill, PlaceholderText = "Nhập tên tài khoản...", Margin = new Padding(0) };
            userRounded.TextChangedEvent += (s, e) => _usernameBox.Text = userRounded.Text;
            var userField = BuildIconInputField("\uE716", userRounded, UITheme.PrimaryColor);

            _passwordBox = new TextBox { Visible = false }; // Keep for compatibility
            var passRounded = new RoundedTextBox { Dock = DockStyle.Fill, PlaceholderText = "Nhập mật khẩu của bạn...", UseSystemPasswordChar = true, Margin = new Padding(0) };
            passRounded.TextChangedEvent += (s, e) => _passwordBox.Text = passRounded.Text;
            var passField = BuildIconInputField("\uE72E", passRounded, UITheme.TextSecondaryColor);

            _showPasswordCheckBox = new CheckBox { Dock = DockStyle.Fill, Text = "Hiển thị mật khẩu", ForeColor = UITheme.TextSecondaryColor, Font = UITheme.CaptionFont };
            _showPasswordCheckBox.CheckedChanged += (s, e) => passRounded.UseSystemPasswordChar = !_showPasswordCheckBox.Checked;

            var button = UIHelper.CreatePrimaryButton("Đăng nhập");
            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(0, 16, 0, 0); // Spacing added here
            button.BackColor = UITheme.PrimaryColor; // Ceramic Blue
            button.FlatAppearance.MouseOverBackColor = UITheme.PrimaryHoverColor;
            
            button.Click += BtnLogin_Click;

            // Ghost Button for Exit
            var exitButton = new Button
            {
                Text = "Thoát ứng dụng",
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = UITheme.TextSecondaryColor,
                Font = UITheme.BodyBoldFont,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 8, 0, 0)
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.FlatAppearance.MouseOverBackColor = UITheme.HoverFillColor;
            exitButton.Click += (s, e) =>
            {
                if (UiDialogs.Confirm("Bạn có chắc chắn muốn đóng ứng dụng không?", "Thoát ứng dụng"))
                    Application.Exit();
            };

            _statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UITheme.ErrorColor,
                Font = UITheme.CaptionFont,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = string.Empty
            };

            container.Controls.Add(heading, 0, 0);
            container.Controls.Add(body, 0, 1);
            container.Controls.Add(userLabel, 0, 2);
            container.Controls.Add(userField, 0, 3);
            container.Controls.Add(passLabel, 0, 4);
            container.Controls.Add(passField, 0, 5);
            container.Controls.Add(_showPasswordCheckBox, 0, 6);
            container.Controls.Add(button, 0, 7);
            container.Controls.Add(exitButton, 0, 8);
            container.Controls.Add(_statusLabel, 0, 9);

            card.Controls.Add(container);
            panel.Controls.Add(card);
            
            panel.Resize += (s, e) => 
            {
                CenterCard(panel, card);
                panel.Invalidate(); // Repaint shadow on resize
            };
            CenterCard(panel, card);
            
            // Set focus
            this.Shown += (s, e) => userRounded.Focus();
            return panel;
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void CenterCard(Control parent, Control card)
        {
            card.Left = Math.Max(24, (parent.ClientSize.Width - card.Width) / 2);
            card.Top = Math.Max(24, (parent.ClientSize.Height - card.Height) / 2);
        }

        private Control BuildIconInputField(string iconChar, RoundedTextBox input, Color iconColor)
        {
            var wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 4, 0, 0),
                MinimumSize = new Size(0, 40)
            };

            var iconLabel = new Label
            {
                Dock = DockStyle.Left,
                Width = 42,
                Text = iconChar,
                Font = new Font("Segoe MDL2 Assets", 13F, FontStyle.Regular),
                ForeColor = iconColor,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            var fieldHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            input.Dock = DockStyle.Fill;
            fieldHost.Controls.Add(input);

            wrapper.Controls.Add(fieldHost);
            wrapper.Controls.Add(iconLabel);
            return wrapper;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            _statusLabel.Text = string.Empty;
            var username = _usernameBox.Text.Trim();
            var password = _passwordBox.Text;

            try
            {
                var auth = new BLL.AuthService();
                var user = auth.Authenticate(username, password);
                if (user == null)
                {
                    _statusLabel.Text = "Sai tên đăng nhập hoặc mật khẩu.";
                    return;
                }

                var main = new MainForm(user);
                main.FormClosed += (s, args) =>
                {
                    Show();
                    _usernameBox.Focus();
                };

                Hide();
                main.Show();
            }
            catch (Exception ex)
            {
                _statusLabel.Text = "Lỗi: " + ex.Message;
            }
        }
    }
}
