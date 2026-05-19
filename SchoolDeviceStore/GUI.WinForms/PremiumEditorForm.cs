using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GUI.WinForms
{
    /// <summary>
    /// Premium base class for all editor modal forms.
    /// Provides top-aligned labels with no background (transparent), inline validation,
    /// real-time currency formatting, focus highlighting border, unified 40px input height,
    /// and clean grid alignment.
    /// </summary>
    public abstract class PremiumEditorForm : Form
    {
        // ── Shared State ──
        private bool _isDirty;
        private bool _isSaving;
        private Button _primaryButton;
        private Button _cancelButton;
        private string _primaryButtonOriginalText;
        private readonly Dictionary<Control, Label> _errorLabels = new Dictionary<Control, Label>();
        private readonly Dictionary<Control, InputWrapperPanel> _wrappers = new Dictionary<Control, InputWrapperPanel>();
        private readonly List<Control> _allInputs = new List<Control>();
        private Control _firstInput;

        // ── Layout ──
        protected TableLayoutPanel ContentLayout { get; private set; }
        private Panel _footerPanel;

        // ── Constants ──
        private const int FieldHeight = 76;   // label(22) + input(40) + error(14)
        private const int FieldSpacing = 6;
        private static readonly Color ErrorColor = Color.FromArgb(239, 68, 68);

        protected PremiumEditorForm()
        {
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            KeyPreview = true;
            DoubleBuffered = true;
        }

        protected void InitializePremiumLayout(int columnCount = 1)
        {
            UIHelper.ApplyFormTheme(this);
            FontHelper.ApplyVietnameseFontToForm(this);
            BackColor = UITheme.SurfaceColor;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UITheme.SurfaceColor,
                Padding = new Padding(0)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));  // Header
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Content
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));  // Footer (extra padding)

            root.Controls.Add(BuildHeader(), 0, 0);

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = UITheme.SurfaceColor,
                Padding = new Padding(28, 12, 28, 12)
            };

            ContentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = columnCount,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent
            };
            for (int i = 0; i < columnCount; i++)
                ContentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / columnCount));

            scrollPanel.Controls.Add(ContentLayout);
            root.Controls.Add(scrollPanel, 0, 1);
            root.Controls.Add(BuildFooter(), 0, 2);
            Controls.Add(root);

            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) { e.SuppressKeyPress = true; TryClose(); }
            };

            FormClosing += OnFormClosing;

            Load += (s, e) =>
            {
                if (_firstInput != null)
                {
                    _firstInput.Focus();
                    if (_firstInput is TextBox tb) tb.SelectAll();
                }
                _isDirty = false;
            };
        }

        private Control BuildHeader()
        {
            var header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.SurfaceColor,
                Padding = new Padding(28, 0, 28, 0)
            };
            header.Paint += (s, e) =>
            {
                using (var pen = new Pen(UITheme.BorderColor))
                    e.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            var titleLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = Text,
                Font = UITheme.SectionTitleFont,
                ForeColor = UITheme.TextPrimaryColor,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            var iconLabel = new Label
            {
                Dock = DockStyle.Left,
                Width = 40,
                Text = GetFormIcon(),
                Font = new Font("Segoe UI", 18F),
                ForeColor = UITheme.PrimaryColor,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            header.Controls.Add(titleLabel);
            header.Controls.Add(iconLabel);
            return header;
        }

        protected virtual string GetFormIcon() => "📝";

        private Control BuildFooter()
        {
            _footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.SubtleSurfaceColor,
                Padding = new Padding(28, 20, 28, 20) // Clean gap above buttons
            };
            _footerPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(UITheme.BorderColor))
                    e.Graphics.DrawLine(pen, 0, 0, _footerPanel.Width, 0);
            };

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };

            _primaryButton = UIHelper.CreatePrimaryButton(GetPrimaryButtonText());
            _primaryButton.Width = 130;
            _primaryButton.Height = 40;
            _primaryButton.Font = UITheme.BodyBoldFont;
            _primaryButton.Click += async (s, e) => await PerformSaveAsync();
            _primaryButtonOriginalText = _primaryButton.Text;

            _cancelButton = UIHelper.CreateSecondaryButton("Hủy");
            _cancelButton.Width = 100;
            _cancelButton.Height = 40;
            _cancelButton.Click += (s, e) => TryClose();

            flow.Controls.Add(_primaryButton);
            flow.Controls.Add(_cancelButton);
            _footerPanel.Controls.Add(flow);

            AcceptButton = _primaryButton;
            return _footerPanel;
        }

        protected virtual string GetPrimaryButtonText() => "Lưu";

        // ════════════════════════════════════════
        // FIELD BUILDERS (Clean 40px Input Heights)
        // ════════════════════════════════════════

        protected TextBox AddTextField(string label, bool required = false, bool multiline = false, bool readOnly = false, int row = 0, int column = 0, int colSpan = 1)
        {
            var tb = new TextBox
            {
                Font = UITheme.BaseFont,
                BackColor = readOnly ? UITheme.SubtleSurfaceColor : Color.White,
                ReadOnly = readOnly,
                Multiline = multiline
            };
            tb.TextChanged += (s, e) => { _isDirty = true; ClearFieldError(tb); };
            return (TextBox)AddFieldInternal(label, tb, required, row, column, colSpan);
        }

        protected TextBox AddPasswordField(string label, bool required = false, int row = 0, int column = 0, int colSpan = 1)
        {
            var tb = new TextBox
            {
                Font = UITheme.BaseFont,
                UseSystemPasswordChar = true
            };
            tb.TextChanged += (s, e) => { _isDirty = true; ClearFieldError(tb); };
            return (TextBox)AddFieldInternal(label, tb, required, row, column, colSpan);
        }

        protected TextBox AddCurrencyField(string label, bool required = false, int row = 0, int column = 0, int colSpan = 1)
        {
            var tb = new TextBox
            {
                Font = UITheme.BaseFont,
                TextAlign = HorizontalAlignment.Right
            };

            tb.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
                    e.Handled = true;
            };
            tb.TextChanged += (s, e) =>
            {
                _isDirty = true;
                ClearFieldError(tb);
                FormatCurrencyField(tb);
            };

            return (TextBox)AddFieldInternal(label, tb, required, row, column, colSpan, isCurrency: true);
        }

        protected ComboBox AddDropdownField(string label, bool required = false, int row = 0, int column = 0, int colSpan = 1)
        {
            var cb = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = UITheme.BaseFont
            };
            cb.SelectedIndexChanged += (s, e) => { _isDirty = true; ClearFieldError(cb); };
            return (ComboBox)AddFieldInternal(label, cb, required, row, column, colSpan);
        }

        protected DateTimePicker AddDateField(string label, bool required = false, int row = 0, int column = 0, int colSpan = 1)
        {
            var dtp = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = UITheme.BaseFont
            };
            dtp.ValueChanged += (s, e) => { _isDirty = true; ClearFieldError(dtp); };
            return (DateTimePicker)AddFieldInternal(label, dtp, required, row, column, colSpan);
        }

        protected NumericUpDown AddNumericField(string label, decimal max = 100000000, int decimals = 0, bool required = false, int row = 0, int column = 0, int colSpan = 1)
        {
            var nud = new NumericUpDown
            {
                Font = UITheme.BaseFont,
                Maximum = max,
                Minimum = 0,
                DecimalPlaces = decimals,
                ThousandsSeparator = true
            };
            nud.ValueChanged += (s, e) => { _isDirty = true; ClearFieldError(nud); };
            return (NumericUpDown)AddFieldInternal(label, nud, required, row, column, colSpan);
        }

        protected CheckBox AddCheckboxField(string text, int row = 0, int column = 0, int colSpan = 1)
        {
            var cb = new CheckBox
            {
                Text = text,
                Dock = DockStyle.Fill,
                Font = UITheme.BaseFont,
                ForeColor = UITheme.TextPrimaryColor,
                BackColor = Color.Transparent
            };
            cb.CheckedChanged += (s, e) => _isDirty = true;

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 20, 0, 0)
            };
            panel.Controls.Add(cb);

            EnsureLayoutCapacity(row);
            ContentLayout.Controls.Add(panel, column, row);
            if (colSpan > 1) ContentLayout.SetColumnSpan(panel, colSpan);

            _allInputs.Add(cb);
            return cb;
        }

        protected void AddSectionHeader(string title, int row = 0, int colSpan = 1)
        {
            var lbl = new Label
            {
                Text = title,
                Dock = DockStyle.Bottom,
                Height = 24,
                Font = UITheme.BodyBoldFont,
                ForeColor = UITheme.PrimaryColor,
                BackColor = Color.Transparent
            };

            var fieldPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            fieldPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(UITheme.BorderColor))
                    e.Graphics.DrawLine(pen, 0, fieldPanel.Height - 1, fieldPanel.Width, fieldPanel.Height - 1);
            };
            fieldPanel.Controls.Add(lbl);

            EnsureLayoutCapacity(row, height: 38);
            ContentLayout.Controls.Add(fieldPanel, 0, row);
            if (colSpan > 1) ContentLayout.SetColumnSpan(fieldPanel, colSpan);
        }

        private void EnsureLayoutCapacity(int row, int height = FieldHeight)
        {
            while (ContentLayout.RowStyles.Count <= row)
            {
                ContentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, height + FieldSpacing));
            }
            if (ContentLayout.RowCount <= row)
            {
                ContentLayout.RowCount = row + 1;
            }
        }

        // ── Internal field builder ──
        private Control AddFieldInternal(string label, Control inputControl, bool required, int row, int column, int colSpan, bool isCurrency = false)
        {
            var fieldPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = FieldHeight,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 8, FieldSpacing)
            };

            // Transparent Header layout with no black BG issues
            var labelPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 20,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            var lblText = new Label
            {
                Text = label,
                AutoSize = true,
                Font = UITheme.CaptionFont,
                ForeColor = Color.FromArgb(45, 55, 72), // #2D3748
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 2, 0)
            };
            labelPanel.Controls.Add(lblText);

            if (required)
            {
                var lblAsterisk = new Label
                {
                    Text = "*",
                    AutoSize = true,
                    Font = new Font(UITheme.CaptionFont.FontFamily, 9.5F, FontStyle.Bold),
                    ForeColor = ErrorColor,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0)
                };
                labelPanel.Controls.Add(lblAsterisk);
            }

            var errorLabel = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 14,
                Font = new Font(UITheme.CaptionFont.FontFamily, 7.5F),
                ForeColor = ErrorColor,
                Text = "",
                BackColor = Color.Transparent,
                Visible = false
            };
            _errorLabels[inputControl] = errorLabel;

            // Custom wrapped Input control with unified 40px height & focus highlights
            var wrapper = new InputWrapperPanel(inputControl, isCurrency);
            wrapper.Dock = DockStyle.Top;
            _wrappers[inputControl] = wrapper;

            fieldPanel.Controls.Add(wrapper);
            fieldPanel.Controls.Add(errorLabel);
            fieldPanel.Controls.Add(labelPanel);

            EnsureLayoutCapacity(row);
            ContentLayout.Controls.Add(fieldPanel, column, row);
            if (colSpan > 1) ContentLayout.SetColumnSpan(fieldPanel, colSpan);

            _allInputs.Add(inputControl);
            if (_firstInput == null) _firstInput = inputControl;
            return inputControl;
        }

        // ════════════════════════════════════════
        // INLINE VALIDATION & ERROR BORDERS
        // ════════════════════════════════════════

        protected void SetFieldError(Control field, string message)
        {
            if (_errorLabels.TryGetValue(field, out var lbl))
            {
                lbl.Text = message;
                lbl.Visible = true;
            }
            if (_wrappers.TryGetValue(field, out var wrap))
            {
                wrap.SetError(true);
            }
        }

        protected void ClearFieldError(Control field)
        {
            if (_errorLabels.TryGetValue(field, out var lbl))
            {
                lbl.Text = "";
                lbl.Visible = false;
            }
            if (_wrappers.TryGetValue(field, out var wrap))
            {
                wrap.SetError(false);
            }
        }

        protected void ClearAllErrors()
        {
            foreach (var key in _errorLabels.Keys)
            {
                ClearFieldError(key);
            }
        }

        protected string ValidateRequired(TextBox field, string fieldName)
        {
            var val = field.Text.Trim();
            if (string.IsNullOrWhiteSpace(val))
            {
                SetFieldError(field, $"{fieldName} không được để trống.");
                field.Focus();
                return null;
            }
            return val;
        }

        protected bool ValidateDropdown(ComboBox field, string fieldName)
        {
            if (field.SelectedIndex < 0 || field.SelectedValue == null)
            {
                SetFieldError(field, $"Vui lòng chọn {fieldName}.");
                field.Focus();
                return false;
            }
            return true;
        }

        protected decimal? ParseCurrency(TextBox field, string fieldName, bool required = true)
        {
            var text = field.Text.Replace(".", "").Replace(",", "").Replace("₫", "").Trim();
            if (string.IsNullOrEmpty(text))
            {
                if (required)
                {
                    SetFieldError(field, $"{fieldName} không được để trống.");
                    field.Focus();
                }
                return required ? (decimal?)null : 0;
            }
            decimal val;
            if (!decimal.TryParse(text, out val) || val < 0)
            {
                SetFieldError(field, $"{fieldName} phải là số hợp lệ ≥ 0.");
                field.Focus();
                return null;
            }
            return val;
        }

        private void FormatCurrencyField(TextBox tb)
        {
            var raw = tb.Text.Replace(".", "").Replace(",", "").Trim();
            if (string.IsNullOrEmpty(raw)) return;
            long val;
            if (long.TryParse(raw, out val))
            {
                var cursorPos = tb.SelectionStart;
                var oldLen = tb.Text.Length;
                tb.Text = val.ToString("N0").Replace(",", ".");
                var newLen = tb.Text.Length;
                tb.SelectionStart = Math.Max(0, cursorPos + (newLen - oldLen));
            }
        }

        protected void SetCurrencyValue(TextBox tb, decimal value)
        {
            tb.Text = ((long)value).ToString("N0").Replace(",", ".");
        }

        protected decimal GetCurrencyValue(TextBox tb)
        {
            var raw = tb.Text.Replace(".", "").Replace(",", "").Replace("₫", "").Trim();
            decimal val;
            return decimal.TryParse(raw, out val) ? val : 0;
        }

        private async System.Threading.Tasks.Task PerformSaveAsync()
        {
            if (_isSaving) return;

            ClearAllErrors();

            _isSaving = true;
            _primaryButton.Enabled = false;
            _primaryButton.Text = "Đang lưu...";
            _cancelButton.Enabled = false;

            try
            {
                bool success = OnSave();
                if (success)
                {
                    _isDirty = false;
                    DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Save failed in " + GetType().Name, ex);
                UiDialogs.ShowError(ex.Message);
            }
            finally
            {
                _isSaving = false;
                _primaryButton.Enabled = true;
                _primaryButton.Text = _primaryButtonOriginalText;
                _cancelButton.Enabled = true;
            }
        }

        protected abstract bool OnSave();

        private void TryClose()
        {
            if (_isDirty)
            {
                if (!UiDialogs.Confirm("Bạn có chắc chắn muốn thoát?\n\nCác thay đổi chưa được lưu sẽ bị mất.", "Xác nhận thoát"))
                    return;
            }
            _isDirty = false;
            DialogResult = DialogResult.Cancel;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK) return;
            if (_isDirty && e.CloseReason == CloseReason.UserClosing)
            {
                if (!UiDialogs.Confirm("Bạn có chắc chắn muốn thoát?\n\nCác thay đổi chưa được lưu sẽ bị mất.", "Xác nhận thoát"))
                {
                    e.Cancel = true;
                }
            }
        }
    }

    /// <summary>
    /// Custom input container rendering 40px height, 6px border-radius,
    /// dynamic focus highlighting, and clean right-aligned currency suffix.
    /// </summary>
    public class InputWrapperPanel : Panel
    {
        public Control InputControl { get; }
        private bool _isFocused;
        private bool _hasError;

        public InputWrapperPanel(Control inputControl, bool isCurrency = false)
        {
            InputControl = inputControl;
            Height = 40;
            BackColor = Color.White;
            Padding = new Padding(12, 10, 12, 10);
            DoubleBuffered = true;

            inputControl.Dock = DockStyle.Fill;
            inputControl.Font = UITheme.BaseFont;
            inputControl.BackColor = Color.White;

            if (inputControl is TextBox tb)
            {
                tb.BorderStyle = BorderStyle.None;
                tb.Enter += (s, e) => { _isFocused = true; Invalidate(); };
                tb.Leave += (s, e) => { _isFocused = false; Invalidate(); };
            }
            else if (inputControl is ComboBox cb)
            {
                cb.FlatStyle = FlatStyle.Flat;
                cb.Enter += (s, e) => { _isFocused = true; Invalidate(); };
                cb.Leave += (s, e) => { _isFocused = false; Invalidate(); };
            }
            else if (inputControl is NumericUpDown nud)
            {
                nud.BorderStyle = BorderStyle.None;
                nud.Enter += (s, e) => { _isFocused = true; Invalidate(); };
                nud.Leave += (s, e) => { _isFocused = false; Invalidate(); };
            }
            else if (inputControl is DateTimePicker dtp)
            {
                dtp.Enter += (s, e) => { _isFocused = true; Invalidate(); };
                dtp.Leave += (s, e) => { _isFocused = false; Invalidate(); };
            }

            if (isCurrency)
            {
                var suffix = new Label
                {
                    Dock = DockStyle.Right,
                    Width = 24,
                    Text = "đ",
                    Font = UITheme.BaseFont,
                    ForeColor = Color.FromArgb(160, 174, 192), // #A0AEC0
                    TextAlign = ContentAlignment.MiddleRight,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0)
                };
                Controls.Add(inputControl);
                Controls.Add(suffix);
            }
            else
            {
                Controls.Add(inputControl);
            }
        }

        public void SetError(bool hasError)
        {
            _hasError = hasError;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            using (var path = GetRoundedPath(rect, 6))
            {
                Color borderColor = Color.FromArgb(226, 232, 240); // #E2E8F0
                int borderWidth = 1;

                if (_hasError)
                {
                    borderColor = Color.FromArgb(239, 68, 68); // Red
                }
                else if (_isFocused)
                {
                    borderColor = UITheme.PrimaryColor; // Blue #2563EB
                    borderWidth = 2;
                }

                using (var pen = new Pen(borderColor, borderWidth))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        private static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
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
    }
}
