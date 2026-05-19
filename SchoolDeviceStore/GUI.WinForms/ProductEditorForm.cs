using System;
using System.Drawing;
using System.Windows.Forms;
using DTO;

namespace GUI.WinForms
{
    /// <summary>
    /// Premium product editor form with top-aligned labels, currency formatting,
    /// inline validation, sell-price vs purchase-price warning, and modern UX.
    /// </summary>
    public class ProductEditorForm : PremiumEditorForm
    {
        public Product Product { get; private set; }

        private TextBox _codeField, _nameField, _unitPriceField, _purchasePriceField;
        private NumericUpDown _qtyField;
        private ComboBox _categoryField, _supplierField, _statusField;
        private Label _priceWarning;

        private readonly BLL.CategoryService _catService = new BLL.CategoryService();
        private readonly BLL.SupplierService _supService = new BLL.SupplierService();

        public ProductEditorForm(Product p = null)
        {
            Product = p ?? new Product();
            Text = Product.ProductId == 0 ? "Thêm sản phẩm mới" : "Sửa sản phẩm";
            Width = 560;
            Height = 580;

            InitializePremiumLayout(2); // 2-column layout grid
            BuildFields();
            LoadLookups();
            if (p != null) LoadProduct(p);
        }

        protected override string GetFormIcon() => "📦";
        protected override string GetPrimaryButtonText() => Product.ProductId == 0 ? "Tạo mới" : "Cập nhật";

        private void BuildFields()
        {
            // Row 0: Code (span 2)
            _codeField = AddTextField("Mã sản phẩm", required: true, row: 0, column: 0, colSpan: 2);

            // Row 1: Name (span 2)
            _nameField = AddTextField("Tên sản phẩm", required: true, row: 1, column: 0, colSpan: 2);

            // Row 2: Category + Quantity (side by side)
            _categoryField = AddDropdownField("Danh mục", required: true, row: 2, column: 0);
            _qtyField = AddNumericField("Số lượng tồn kho", max: 999999, row: 2, column: 1);

            // Row 3: Sell Price + Purchase Price (side by side)
            _unitPriceField = AddCurrencyField("Giá bán", required: true, row: 3, column: 0);
            _purchasePriceField = AddCurrencyField("Giá nhập", required: true, row: 3, column: 1);

            // Row 4: Price comparison warning (inline warning span 2)
            _priceWarning = new Label
            {
                Text = "",
                Dock = DockStyle.Fill,
                Font = new Font(UITheme.CaptionFont.FontFamily, 8.5F, FontStyle.Italic),
                ForeColor = UITheme.WarningColor,
                Visible = false,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            
            var warnPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            warnPanel.Controls.Add(_priceWarning);
            
            // Add row style manually
            ContentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            ContentLayout.Controls.Add(warnPanel, 0, 4);
            ContentLayout.SetColumnSpan(warnPanel, 2);

            // Monitor price changes for warning
            _unitPriceField.TextChanged += (s, e) => CheckPriceWarning();
            _purchasePriceField.TextChanged += (s, e) => CheckPriceWarning();

            // Row 5: Supplier + Status (side by side)
            _supplierField = AddDropdownField("Nhà cung cấp", row: 5, column: 0);
            _statusField = AddDropdownField("Tình trạng", row: 5, column: 1);
            _statusField.Items.AddRange(new object[] { "Available", "Unavailable", "Discontinued" });
            _statusField.SelectedIndex = 0;
        }

        private void CheckPriceWarning()
        {
            var sell = GetCurrencyValue(_unitPriceField);
            var purchase = GetCurrencyValue(_purchasePriceField);
            if (purchase > 0 && sell > 0 && sell < purchase)
            {
                _priceWarning.Text = "⚠ Giá bán đang thấp hơn giá nhập (Bán lỗ)";
                _priceWarning.Visible = true;
            }
            else
            {
                _priceWarning.Visible = false;
            }
        }

        private void LoadLookups()
        {
            try
            {
                var cats = _catService.GetAll();
                _categoryField.DataSource = cats;
                _categoryField.DisplayMember = "CategoryName";
                _categoryField.ValueMember = "CategoryId";

                var sups = _supService.GetAll();
                _supplierField.DataSource = sups;
                _supplierField.DisplayMember = "SupplierName";
                _supplierField.ValueMember = "SupplierId";
            }
            catch (Exception ex)
            {
                AppLogger.Error("LoadLookups failed in ProductEditorForm", ex);
            }
        }

        private void LoadProduct(Product p)
        {
            _codeField.Text = p.ProductCode;
            _nameField.Text = p.ProductName;
            _qtyField.Value = p.Quantity;
            SetCurrencyValue(_unitPriceField, p.UnitPrice);
            SetCurrencyValue(_purchasePriceField, p.PurchasePrice);
            if (p.CategoryId.HasValue && _categoryField.Items.Count > 0) _categoryField.SelectedValue = p.CategoryId.Value;
            if (p.SupplierId.HasValue && _supplierField.Items.Count > 0) _supplierField.SelectedValue = p.SupplierId.Value;
            var idx = _statusField.Items.IndexOf(p.Status ?? "Available");
            _statusField.SelectedIndex = idx >= 0 ? idx : 0;
        }

        protected override bool OnSave()
        {
            var code = ValidateRequired(_codeField, "Mã sản phẩm");
            var name = ValidateRequired(_nameField, "Tên sản phẩm");

            var unitPrice = ParseCurrency(_unitPriceField, "Giá bán");
            var purchasePrice = ParseCurrency(_purchasePriceField, "Giá nhập");

            if (code == null || name == null || !unitPrice.HasValue || !purchasePrice.HasValue) return false;

            Product.ProductCode = code;
            Product.ProductName = name;
            Product.Quantity = (int)_qtyField.Value;
            Product.UnitPrice = unitPrice.Value;
            Product.PurchasePrice = purchasePrice.Value;
            Product.Status = _statusField.SelectedItem?.ToString() ?? "Available";
            Product.CategoryId = _categoryField.SelectedValue == null ? (int?)null : Convert.ToInt32(_categoryField.SelectedValue);
            Product.SupplierId = _supplierField.SelectedValue == null ? (int?)null : Convert.ToInt32(_supplierField.SelectedValue);

            return true;
        }
    }
}
