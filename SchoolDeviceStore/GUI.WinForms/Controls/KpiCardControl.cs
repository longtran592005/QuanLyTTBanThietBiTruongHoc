using System.Drawing;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public class KpiCardControl : UserControl
    {
        private readonly Label _titleLabel;
        private readonly Label _valueLabel;
        private readonly Label _subtitleLabel;
        private Color _accentColor = UITheme.PrimaryColor;
        private string _iconChar = "\uE128";

        public KpiCardControl()
        {
            BackColor = UITheme.SurfaceColor;
            Padding = new Padding(24, 24, 24, 24);
            Margin = new Padding(8);
            Dock = DockStyle.Fill;
            Height = 140;

            _titleLabel = new Label { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 8), Font = UITheme.BodyBoldFont, ForeColor = UITheme.TextSecondaryColor, Text = "Chỉ số" };
            _valueLabel = new Label { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 8), Font = UITheme.HeroFont, ForeColor = UITheme.TextPrimaryColor, Text = "0" };
            _subtitleLabel = new Label { Dock = DockStyle.Top, AutoSize = true, Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor, Text = string.Empty };

            var content = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            content.Controls.Add(_subtitleLabel);
            content.Controls.Add(_valueLabel);
            content.Controls.Add(_titleLabel);

            Controls.Add(content);

            UIHelper.StyleCard(this);
            Resize += (s, e) => Invalidate();
        }

        public void SetData(string title, string value, string subtitle, Color accentColor)
        {
            _titleLabel.Text = title;
            _valueLabel.Text = value;
            _subtitleLabel.Text = subtitle;
            _accentColor = accentColor;
            
            // Assign icon based on content
            if (title.ToLower().Contains("doanh thu")) _iconChar = "\uE128"; // Dollar
            else if (title.ToLower().Contains("hóa đơn")) _iconChar = "\uE825"; // Receipt
            else if (title.ToLower().Contains("sản phẩm")) _iconChar = "\uE14C"; // Box
            else if (title.ToLower().Contains("hết hàng")) _iconChar = "\uE171"; // Alert/Warning
            else _iconChar = "\uE128";
            
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw top-right modern icon bubble
            int size = 48;
            int x = Width - size - 20;
            int y = 20;
            
            // Soft colored background (10% opacity)
            using (var bgBrush = new SolidBrush(Color.FromArgb(25, _accentColor)))
            {
                e.Graphics.FillEllipse(bgBrush, x, y, size, size);
            }

            // Crisp vector icon
            using (var font = new Font("Segoe MDL2 Assets", 18F))
            using (var textBrush = new SolidBrush(_accentColor))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(_iconChar, font, textBrush, new RectangleF(x, y, size, size), sf);
            }
        }
    }
}
