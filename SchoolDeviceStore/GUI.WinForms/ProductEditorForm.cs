using System;
using System.ComponentModel;
using System.Windows.Forms;
using DTO;

namespace GUI.WinForms
{
    public class ProductEditorForm : Form
    {
        public Product Product { get; private set; }

        private TextBox txtCode, txtName, txtQty, txtUnitPrice, txtPurchasePrice, txtStatus;
        private ComboBox cmbCategory, cmbSupplier;
        private Button btnOk, btnCancel;
        private BLL.CategoryService _catService = new BLL.CategoryService();
        private BLL.SupplierService _supService = new BLL.SupplierService();
        private readonly ErrorProvider _errorProvider = new ErrorProvider();

        public ProductEditorForm(Product p = null)
        {
            try
            {
                Product = p ?? new Product();
                InitializeComponents();
                LoadLookups();
                if (p != null) LoadProduct(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi khởi tạo form sản phẩm:\n" + ex.Message + "\n\nStackTrace:\n" + ex.StackTrace, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppLogger.Error("ProductEditorForm constructor failed", ex);
                throw; // Re-throw để form không mở
            }
        }

        private void InitializeComponents()
        {
            this.Text = Product.ProductId == 0 ? "Thêm sản phẩm" : "Sửa sản phẩm";
            this.Width = 540;
            this.Height = 440;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            UIHelper.ApplyFormTheme(this);
            FontHelper.ApplyVietnameseFontToForm(this);
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 7,
                Padding = new Padding(16),
                BackColor = UITheme.BackgroundColor
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F)); // Row 0: Mã SP
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F)); // Row 1: Tên SP
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F)); // Row 2: Danh mục + Số lượng
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F)); // Row 3: Giá bán + Giá nhập
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F)); // Row 4: NCC + Tình trạng
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Row 5: Buttons
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            txtCode = new TextBox { Dock = DockStyle.Fill };
            txtName = new TextBox { Dock = DockStyle.Fill };
            txtQty = new TextBox { Dock = DockStyle.Fill };
            txtUnitPrice = new TextBox { Dock = DockStyle.Fill };
            txtPurchasePrice = new TextBox { Dock = DockStyle.Fill };
            txtStatus = new TextBox { Dock = DockStyle.Fill };
            cmbCategory = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSupplier = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

            // Row 0: Mã SP
            layout.Controls.Add(new Label { Text = "Mã SP", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 0);
            layout.Controls.Add(txtCode, 1, 0);
            layout.SetColumnSpan(txtCode, 3);

            // Row 1: Tên SP
            layout.Controls.Add(new Label { Text = "Tên SP", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 1);
            layout.Controls.Add(txtName, 1, 1);
            layout.SetColumnSpan(txtName, 3);

            // Row 2: Danh mục + Số lượng
            layout.Controls.Add(new Label { Text = "Danh mục", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 2);
            layout.Controls.Add(cmbCategory, 1, 2);
            layout.Controls.Add(new Label { Text = "Số lượng", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 2, 2);
            layout.Controls.Add(txtQty, 3, 2);

            // Row 3: Giá bán + Giá nhập
            layout.Controls.Add(new Label { Text = "Giá bán", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 3);
            layout.Controls.Add(txtUnitPrice, 1, 3);
            layout.Controls.Add(new Label { Text = "Giá nhập", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 2, 3);
            layout.Controls.Add(txtPurchasePrice, 3, 3);

            // Row 4: NCC + Tình trạng
            layout.Controls.Add(new Label { Text = "Nhà cung cấp", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, 4);
            layout.Controls.Add(cmbSupplier, 1, 4);
            layout.Controls.Add(new Label { Text = "Tình trạng", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 2, 4);
            layout.Controls.Add(txtStatus, 3, 4);

            // Row 5: Buttons
            var buttonFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(0, 8, 0, 0) };
            btnOk = UIHelper.CreatePrimaryButton("Lưu");
            btnOk.Width = 100;
            btnOk.Click += BtnOk_Click;
            btnCancel = UIHelper.CreateSecondaryButton("Hủy");
            btnCancel.Width = 100;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            buttonFlow.Controls.Add(btnOk);
            buttonFlow.Controls.Add(btnCancel);
            layout.Controls.Add(buttonFlow, 1, 5);
            layout.SetColumnSpan(buttonFlow, 3);

            this.Controls.Add(layout);
        }

        private void LoadLookups()
        {
            try
            {
                var cats = _catService.GetAll();
                cmbCategory.DataSource = cats;
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryId";

                var sups = _supService.GetAll();
                cmbSupplier.DataSource = sups;
                cmbSupplier.DisplayMember = "SupplierName";
                cmbSupplier.ValueMember = "SupplierId";
            }
            catch (Exception ex)
            {
                AppLogger.Error("LoadLookups failed in ProductEditorForm", ex);
                MessageBox.Show("Không thể tải dữ liệu tham chiếu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadProduct(Product p)
        {
            txtCode.Text = p.ProductCode;
            txtName.Text = p.ProductName;
            txtQty.Text = p.Quantity.ToString();
            txtUnitPrice.Text = p.UnitPrice.ToString();
            txtPurchasePrice.Text = p.PurchasePrice.ToString();
            txtStatus.Text = p.Status;
            if (p.CategoryId.HasValue && cmbCategory.Items.Count > 0) cmbCategory.SelectedValue = p.CategoryId.Value;
            if (p.SupplierId.HasValue && cmbSupplier.Items.Count > 0) cmbSupplier.SelectedValue = p.SupplierId.Value;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                ClearValidationErrors();

                Product.ProductCode = BLL.ValidationHelper.RequireText(txtCode.Text, "Product code");
                Product.ProductName = BLL.ValidationHelper.RequireText(txtName.Text, "Product name");
                Product.Quantity = ParseInt(txtQty, "Quantity", allowZero: true);
                Product.UnitPrice = ParseDecimal(txtUnitPrice, "Unit price");
                Product.PurchasePrice = ParseDecimal(txtPurchasePrice, "Purchase price");
                Product.Status = txtStatus.Text.Trim();
                Product.CategoryId = (cmbCategory.SelectedValue == null) ? (int?)null : Convert.ToInt32(cmbCategory.SelectedValue);
                Product.SupplierId = (cmbSupplier.SelectedValue == null) ? (int?)null : Convert.ToInt32(cmbSupplier.SelectedValue);

                if (Product.UnitPrice < 0m || Product.PurchasePrice < 0m || Product.Quantity < 0)
                {
                    MessageBox.Show("Các giá trị số không được nhỏ hơn 0.");
                    return;
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (ArgumentException argumentException)
            {
                MessageBox.Show(argumentException.Message, "Xác nhận", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                AppLogger.Error("Product editor validation failure", ex);
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private int ParseInt(TextBox textBox, string fieldName, bool allowZero)
        {
            if (!int.TryParse(textBox.Text.Trim(), out var value))
            {
                _errorProvider.SetError(textBox, fieldName + " phải là một số nguyên hợp lệ.");
                throw new ArgumentException(fieldName + " phải là một số nguyên hợp lệ.");
            }

            if (!allowZero && value <= 0)
            {
                _errorProvider.SetError(textBox, fieldName + " phải lớn hơn 0.");
                throw new ArgumentException(fieldName + " phải lớn hơn 0.");
            }

            return value;
        }

        private decimal ParseDecimal(TextBox textBox, string fieldName)
        {
            if (!decimal.TryParse(textBox.Text.Trim(), out var value))
            {
                _errorProvider.SetError(textBox, fieldName + " phải là một số hợp lệ.");
                throw new ArgumentException(fieldName + " phải là một số hợp lệ.");
            }

            if (value < 0m)
            {
                _errorProvider.SetError(textBox, fieldName + " không được nhỏ hơn 0.");
                throw new ArgumentException(fieldName + " không được nhỏ hơn 0.");
            }

            return value;
        }

        private void ClearValidationErrors()
        {
            _errorProvider.SetError(txtCode, string.Empty);
            _errorProvider.SetError(txtName, string.Empty);
            _errorProvider.SetError(txtQty, string.Empty);
            _errorProvider.SetError(txtUnitPrice, string.Empty);
            _errorProvider.SetError(txtPurchasePrice, string.Empty);
            _errorProvider.SetError(txtStatus, string.Empty);
        }
    }
}
