using System.Drawing;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public class EmptyStateControl : UserControl
    {
        private readonly Label _titleLabel;
        private readonly Label _messageLabel;

        public EmptyStateControl()
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.SurfaceColor;
            Padding = new Padding(24);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _titleLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = UITheme.SectionTitleFont,
                ForeColor = UITheme.TextPrimaryColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Không có dữ liệu"
            };
            _messageLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = UITheme.BaseFont,
                ForeColor = UITheme.TextSecondaryColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Hiện chưa có bản ghi nào để hiển thị."
            };

            layout.Controls.Add(_titleLabel, 0, 0);
            layout.Controls.Add(_messageLabel, 0, 1);
            Controls.Add(layout);
            UIHelper.StyleCard(this);
        }

        public void SetContent(string title, string message)
        {
            _titleLabel.Text = title;
            _messageLabel.Text = message;
        }
    }
}
