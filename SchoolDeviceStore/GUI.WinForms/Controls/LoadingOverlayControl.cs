using System.Drawing;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public class LoadingOverlayControl : UserControl
    {
        private readonly ProgressBar _progressBar;
        private readonly Label _messageLabel;

        public LoadingOverlayControl()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(180, 245, 247, 251);
            Visible = false;

            _progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                Width = 220,
                Height = 20
            };

            _messageLabel = new Label
            {
                AutoSize = true,
                Font = UITheme.BodyBoldFont,
                ForeColor = UITheme.TextPrimaryColor,
                Text = "Đang tải..."
            };

            var panel = new Panel
            {
                Width = 260,
                Height = 72,
                BackColor = UITheme.SurfaceColor,
                Padding = new Padding(16)
            };
            UIHelper.StyleCard(panel);
            _progressBar.Dock = DockStyle.Bottom;
            _messageLabel.Dock = DockStyle.Top;
            panel.Controls.Add(_progressBar);
            panel.Controls.Add(_messageLabel);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1
            };
            layout.Controls.Add(panel, 0, 0);
            layout.SetCellPosition(panel, new TableLayoutPanelCellPosition(0, 0));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Controls.Add(layout);
            Resize += (s, e) => CenterPanel(panel);
        }

        public void SetMessage(string message)
        {
            _messageLabel.Text = message;
        }

        public void ShowOverlay(string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                _messageLabel.Text = message;
            }

            Visible = true;
            BringToFront();
        }

        public void HideOverlay()
        {
            Visible = false;
        }

        private void CenterPanel(Control panel)
        {
            panel.Left = (ClientSize.Width - panel.Width) / 2;
            panel.Top = (ClientSize.Height - panel.Height) / 2;
        }
    }
}
