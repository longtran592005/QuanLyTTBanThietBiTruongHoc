using System;
using System.Drawing;
using System.Windows.Forms;
using DTO;

namespace GUI.WinForms
{
    /// <summary>
    /// Premium promotion editor with top-aligned labels, inline validation, Date checks,
    /// dynamic "unlimited" usage limit toggle, and real-time currency formatting.
    /// </summary>
    public class PromotionEditorForm : PremiumEditorForm
    {
        private readonly BLL.PromotionService _service = new BLL.PromotionService();
        private readonly int? _promotionId;
        private Promotion _existing;

        private TextBox _codeField;
        private TextBox _nameField;
        private TextBox _descField;
        private ComboBox _discountTypeField;
        private TextBox _discountValueField;
        private TextBox _minOrderField;
        private TextBox _maxDiscountField;
        private DateTimePicker _startDateField;
        private DateTimePicker _endDateField;
        private NumericUpDown _usageLimitField;
        private CheckBox _unlimitedCheck;
        private CheckBox _isActiveCheck;

        public PromotionEditorForm(int? promotionId)
        {
            _promotionId = promotionId;
            Text = _promotionId.HasValue ? "Sửa chương trình khuyến mãi" : "Tạo chương trình khuyến mãi mới";
            Width = 560;
            Height = 650;

            InitializePremiumLayout(2); // 2-column layout
            BuildFields();
            LoadData();
        }

        protected override string GetFormIcon() => "🎁";
        protected override string GetPrimaryButtonText() => _promotionId.HasValue ? "Cập nhật" : "Tạo mới";

        private void BuildFields()
        {
            // Row 0: Code + Name
            _codeField = AddTextField("Mã khuyến mãi", required: true, row: 0, column: 0);
            _codeField.CharacterCasing = CharacterCasing.Upper;
            _nameField = AddTextField("Tên chương trình", required: true, row: 0, column: 1);

            // Row 1: Description (Span 2)
            _descField = AddTextField("Mô tả chi tiết", multiline: true, row: 1, column: 0, colSpan: 2);

            // Row 2: Discount Type + Discount Value
            _discountTypeField = AddDropdownField("Loại giảm giá", required: true, row: 2, column: 0);
            _discountValueField = AddCurrencyField("Giá trị giảm", required: true, row: 2, column: 1);
            _discountTypeField.Items.AddRange(new object[] { "Phần trăm (%)", "Số tiền cố định (₫)" });
            _discountTypeField.SelectedIndex = 0;

            // Row 3: Min Order + Max Discount Cap
            _minOrderField = AddCurrencyField("Đơn tối thiểu áp dụng", row: 3, column: 0);
            _maxDiscountField = AddCurrencyField("Giảm tối đa (Cap)", row: 3, column: 1);

            // Row 4: Start Date + End Date
            _startDateField = AddDateField("Ngày bắt đầu hiệu lực", required: true, row: 4, column: 0);
            _endDateField = AddDateField("Ngày kết thúc hiệu lực", required: true, row: 4, column: 1);

            // Validation event for EndDate >= StartDate
            _startDateField.ValueChanged += (s, e) => ValidateDates();
            _endDateField.ValueChanged += (s, e) => ValidateDates();

            // Row 5: Usage Limit + Unlimited toggle
            var usageWrapper = new Panel { Dock = DockStyle.Fill, Height = 40, BackColor = Color.Transparent };
            _usageLimitField = new NumericUpDown
            {
                Dock = DockStyle.Left,
                Width = 100,
                Minimum = 1,
                Maximum = 999999,
                Value = 100,
                Font = UITheme.BaseFont
            };
            
            _unlimitedCheck = new CheckBox
            {
                Dock = DockStyle.Fill,
                Text = "Không giới hạn",
                Font = UITheme.CaptionFont,
                ForeColor = UITheme.TextPrimaryColor,
                BackColor = Color.Transparent,
                Margin = new Padding(12, 0, 0, 0)
            };
            
            _unlimitedCheck.CheckedChanged += (s, e) =>
            {
                if (_unlimitedCheck.Checked)
                {
                    _usageLimitField.Enabled = false;
                    _usageLimitField.Value = 1;
                }
                else
                {
                    _usageLimitField.Enabled = true;
                    _usageLimitField.Value = 100;
                }
            };
            
            usageWrapper.Controls.Add(_unlimitedCheck);
            usageWrapper.Controls.Add(_usageLimitField);

            var lblPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            var lbl = new Label { Dock = DockStyle.Top, Height = 20, Text = "Giới hạn sử dụng", Font = UITheme.CaptionFont, ForeColor = Color.FromArgb(45, 55, 72), BackColor = Color.Transparent };
            
            // Put wrapper in a 40px height layout-friendly panel
            var inputWrap = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent, Padding = new Padding(0, 4, 0, 4) };
            usageWrapper.Dock = DockStyle.Fill;
            inputWrap.Controls.Add(usageWrapper);
            
            lblPanel.Controls.Add(inputWrap);
            lblPanel.Controls.Add(lbl);
            
            // Add RowStyle and Control
            ContentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            ContentLayout.Controls.Add(lblPanel, 0, 5);

            // Row 6: Status check
            _isActiveCheck = AddCheckboxField("Kích hoạt sử dụng ngay", row: 6, column: 0);
        }

        private bool ValidateDates()
        {
            ClearFieldError(_endDateField);
            if (_endDateField.Value.Date < _startDateField.Value.Date)
            {
                SetFieldError(_endDateField, "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
                return false;
            }
            return true;
        }

        private void LoadData()
        {
            if (_promotionId.HasValue)
            {
                _existing = _service.GetById(_promotionId.Value);
                if (_existing == null) return;

                _codeField.Text = _existing.PromotionCode;
                _nameField.Text = _existing.PromotionName;
                _descField.Text = _existing.Description;
                _discountTypeField.SelectedIndex = _existing.DiscountType == "Percentage" ? 0 : 1;
                SetCurrencyValue(_discountValueField, _existing.DiscountValue);
                SetCurrencyValue(_minOrderField, _existing.MinOrderAmount);
                SetCurrencyValue(_maxDiscountField, _existing.MaxDiscountAmount ?? 0);
                _startDateField.Value = _existing.StartDate;
                _endDateField.Value = _existing.EndDate;

                if (_existing.UsageLimit.HasValue)
                {
                    _unlimitedCheck.Checked = false;
                    _usageLimitField.Value = _existing.UsageLimit.Value;
                    _usageLimitField.Enabled = true;
                }
                else
                {
                    _unlimitedCheck.Checked = true;
                    _usageLimitField.Enabled = false;
                }

                _isActiveCheck.Checked = _existing.IsActive;
            }
        }

        protected override bool OnSave()
        {
            var code = ValidateRequired(_codeField, "Mã khuyến mãi");
            var name = ValidateRequired(_nameField, "Tên chương trình");
            var discountValue = ParseCurrency(_discountValueField, "Giá trị giảm");

            if (code == null || name == null || !discountValue.HasValue) return false;
            if (!ValidateDates()) return false;

            var promo = _existing ?? new Promotion();
            promo.PromotionCode = code.ToUpper();
            promo.PromotionName = name;
            promo.Description = _descField.Text.Trim();
            promo.DiscountType = _discountTypeField.SelectedIndex == 0 ? "Percentage" : "FixedAmount";
            promo.DiscountValue = discountValue.Value;
            promo.MinOrderAmount = GetCurrencyValue(_minOrderField);
            
            var maxCap = GetCurrencyValue(_maxDiscountField);
            promo.MaxDiscountAmount = maxCap > 0 ? maxCap : (decimal?)null;

            promo.StartDate = _startDateField.Value.Date;
            promo.EndDate = _endDateField.Value.Date;
            promo.UsageLimit = _unlimitedCheck.Checked ? (int?)null : (int)_usageLimitField.Value;
            promo.IsActive = _isActiveCheck.Checked;
            promo.AppliesTo = "All";

            try
            {
                if (_promotionId.HasValue)
                {
                    _service.UpdatePromotion(promo);
                }
                else
                {
                    _service.CreatePromotion(promo);
                }
                return true;
            }
            catch (Exception ex)
            {
                SetFieldError(_codeField, ex.Message);
                return false;
            }
        }
    }
}
