using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public class RoundedTextBox : UserControl
    {
        private TextBox _textBox;
        private string _placeholderText = "";
        private bool _isFocused = false;
        private Color _borderColor = Color.FromArgb(226, 232, 240); // #E2E8F0
        private Color _focusedBorderColor = Color.FromArgb(37, 99, 235); // #2563EB (Primary)

        public event EventHandler TextChangedEvent;

        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                UpdatePlaceholder();
            }
        }

        public override string Text
        {
            get => _textBox.ForeColor == UITheme.TextSecondaryColor ? "" : _textBox.Text;
            set
            {
                _textBox.Text = value;
                _textBox.ForeColor = UITheme.TextPrimaryColor;
                if (_isPassword && value != "")
                    _textBox.PasswordChar = '●';
            }
        }

        private bool _isPassword = false;
        public bool UseSystemPasswordChar
        {
            get => _isPassword;
            set
            {
                _isPassword = value;
                if (!_isPassword || _textBox.ForeColor == UITheme.TextSecondaryColor)
                    _textBox.PasswordChar = '\0';
                else
                    _textBox.PasswordChar = '●';
            }
        }

        public RoundedTextBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.White;
            Padding = new Padding(12, 10, 12, 10);
            Height = 40;
            Cursor = Cursors.IBeam;

            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Font = UITheme.BaseFont,
                ForeColor = UITheme.TextPrimaryColor
            };

            _textBox.GotFocus += TextBox_GotFocus;
            _textBox.LostFocus += TextBox_LostFocus;
            _textBox.TextChanged += (s, e) => TextChangedEvent?.Invoke(this, e);

            Controls.Add(_textBox);

            this.Click += (s, e) => _textBox.Focus();
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            _isFocused = true;
            if (_textBox.Text == _placeholderText && _textBox.ForeColor == UITheme.TextSecondaryColor)
            {
                _textBox.Text = "";
                _textBox.ForeColor = UITheme.TextPrimaryColor;
                if (_isPassword) _textBox.PasswordChar = '●';
            }
            Invalidate(); // Redraw border
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            _isFocused = false;
            UpdatePlaceholder();
            Invalidate();
        }

        private void UpdatePlaceholder()
        {
            if (string.IsNullOrWhiteSpace(_textBox.Text) || (_textBox.Text == _placeholderText && _textBox.ForeColor == UITheme.TextSecondaryColor))
            {
                _textBox.PasswordChar = '\0';
                _textBox.Text = _placeholderText;
                _textBox.ForeColor = UITheme.TextSecondaryColor;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = GetRoundedPath(rect, 8)) // 8px border radius
            {
                // Draw focus glow
                if (_isFocused)
                {
                    var glowRect = new Rectangle(1, 1, Width - 3, Height - 3);
                    using (var glowPath = GetRoundedPath(glowRect, 8))
                    using (var glowPen = new Pen(Color.FromArgb(60, _focusedBorderColor), 2))
                    {
                        e.Graphics.DrawPath(glowPen, glowPath);
                    }
                }

                // Draw solid border
                var penColor = _isFocused ? _focusedBorderColor : _borderColor;
                using (var pen = new Pen(penColor, 1.5f))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdatePlaceholder();
        }
    }
}
