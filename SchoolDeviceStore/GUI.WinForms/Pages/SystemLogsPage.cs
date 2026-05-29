using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public class SystemLogsPage : UserControl
    {
        private readonly TextBox _logBox = new TextBox();
        private readonly Label _statusLabel = new Label();

        private string LogFilePath => AppLogger.CurrentLogFilePath;

        public SystemLogsPage()
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;
            BuildLayout();
            Load += (s, e) => RefreshLogs();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UITheme.BackgroundColor
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            root.Controls.Add(BuildToolbar(), 0, 0);
            root.Controls.Add(BuildLogCard(), 0, 1);
            Controls.Add(root);
        }

        private Control BuildToolbar()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16, 8, 16, 8) };
            UIHelper.StyleCard(panel);

            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };

            var refreshBtn = UIHelper.CreatePrimaryButton("Làm mới");
            refreshBtn.Click += (s, e) => RefreshLogs();
            layout.Controls.Add(refreshBtn);

            var openFolderBtn = UIHelper.CreateSecondaryButton("Mở thư mục log");
            openFolderBtn.Click += (s, e) => OpenLogFolder();
            layout.Controls.Add(openFolderBtn);

            _statusLabel.AutoSize = true;
            _statusLabel.Font = UITheme.CaptionFont;
            _statusLabel.ForeColor = UITheme.TextSecondaryColor;
            _statusLabel.Margin = new Padding(12, 10, 0, 0);
            _statusLabel.Text = "Sẵn sàng";
            layout.Controls.Add(_statusLabel);

            panel.Controls.Add(layout);
            return panel;
        }

        private Control BuildLogCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16), Margin = new Padding(0, 12, 0, 0) };
            UIHelper.StyleCard(card);

            _logBox.Dock = DockStyle.Fill;
            _logBox.Multiline = true;
            _logBox.ReadOnly = true;
            _logBox.ScrollBars = ScrollBars.Both;
            _logBox.WordWrap = false;
            _logBox.Font = new Font("Consolas", 9F, FontStyle.Regular);
            _logBox.BackColor = Color.White;
            _logBox.ForeColor = Color.FromArgb(34, 39, 46);
            _logBox.BorderStyle = BorderStyle.FixedSingle;

            var title = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "application.log",
                Font = UITheme.SectionTitleFont,
                ForeColor = UITheme.TextPrimaryColor
            };

            card.Controls.Add(_logBox);
            card.Controls.Add(title);
            return card;
        }

        private void RefreshLogs()
        {
            try
            {
                if (!File.Exists(LogFilePath))
                {
                    _logBox.Text = "Chưa có file log hệ thống.";
                    _statusLabel.Text = "Không tìm thấy file log.";
                    return;
                }

                _logBox.Text = File.ReadAllText(LogFilePath, Encoding.UTF8);
                _statusLabel.Text = $"Đã tải {_logBox.Lines.Length:N0} dòng log.";
            }
            catch (Exception ex)
            {
                AppLogger.Error("SystemLogsPage.RefreshLogs failed", ex);
                _logBox.Text = "Không thể tải file log hệ thống.";
                _statusLabel.Text = "Lỗi khi tải log.";
            }
        }

        private void OpenLogFolder()
        {
            try
            {
                var folder = AppLogger.CurrentLogDirectory;
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = folder,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                AppLogger.Error("SystemLogsPage.OpenLogFolder failed", ex);
                UiDialogs.ShowError("Không thể mở thư mục log.");
            }
        }
    }
}