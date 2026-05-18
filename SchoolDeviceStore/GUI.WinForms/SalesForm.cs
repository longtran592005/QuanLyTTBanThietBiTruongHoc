using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BLL;
using DAL.Repositories;
using DTO;

namespace GUI.WinForms
{
    public class SalesForm : Form
    {
        private readonly Employee _currentUser;
        private ComboBox cmbProduct;
        private NumericUpDown nudQuantity;
        private NumericUpDown nudDiscount;
        private NumericUpDown nudVat;
        private TextBox txtCustomerId;
        private DataGridView dgvCart;
        private Label lblSubtotal;
        private Label lblTotal;
        private readonly List<SalesCartItem> _cart = new List<SalesCartItem>();
        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly SalesService _salesService = new SalesService();

        public SalesForm(Employee currentUser)
        {
            _currentUser = currentUser;
            InitializeComponents();
            LoadProducts();
            RefreshCart();
        }

        private void InitializeComponents()
        {
            Text = "Bán hàng / Hóa đơn";
            Width = 980;
            Height = 640;
            StartPosition = FormStartPosition.CenterParent;
            FontHelper.ApplyVietnameseFontToForm(this);

            Controls.Add(new Label { Left = 20, Top = 20, Text = "Mã KH", Width = 80 });
            txtCustomerId = new TextBox { Left = 100, Top = 16, Width = 120 };
            Controls.Add(txtCustomerId);

            Controls.Add(new Label { Left = 240, Top = 20, Text = "Sản phẩm", Width = 100 });
            cmbProduct = new ComboBox { Left = 340, Top = 16, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cmbProduct);

            Controls.Add(new Label { Left = 620, Top = 20, Text = "SL", Width = 50 });
            nudQuantity = new NumericUpDown { Left = 670, Top = 16, Width = 70, Minimum = 1, Maximum = 1000, Value = 1 };
            Controls.Add(nudQuantity);

            var btnAdd = new Button { Left = 750, Top = 14, Width = 110, Text = "Thêm" };
            btnAdd.Click += BtnAdd_Click;
            Controls.Add(btnAdd);

            dgvCart = new DataGridView { Left = 20, Top = 80, Width = 920, Height = 340, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            Controls.Add(dgvCart);

            Controls.Add(new Label { Left = 20, Top = 450, Text = "Giảm giá", Width = 80 });
            nudDiscount = new NumericUpDown { Left = 100, Top = 446, Width = 120, Maximum = 100000000, DecimalPlaces = 0 };
            Controls.Add(nudDiscount);

            Controls.Add(new Label { Left = 240, Top = 450, Text = "VAT %", Width = 80 });
            nudVat = new NumericUpDown { Left = 320, Top = 446, Width = 80, Maximum = 100, DecimalPlaces = 2, Value = 10 };
            Controls.Add(nudVat);

            lblSubtotal = new Label { Left = 420, Top = 450, Width = 200, Text = "Tạm tính: 0" };
            lblTotal = new Label { Left = 630, Top = 450, Width = 200, Text = "Tổng tiền: 0" };
            Controls.Add(lblSubtotal);
            Controls.Add(lblTotal);

            var btnCreate = new Button { Left = 850, Top = 442, Width = 140, Height = 32, Text = "Tạo hóa đơn" };
            btnCreate.Click += BtnCreate_Click;
            Controls.Add(btnCreate);
        }

        private void LoadProducts()
        {
            try
            {
                var products = _productRepository.GetAll();
                cmbProduct.DataSource = products;
                cmbProduct.DisplayMember = "ProductName";
                cmbProduct.ValueMember = "ProductId";
                if (products == null || products.Count == 0)
                {
                    // Inform user there are no products
                    // but keep UI usable
                    MessageBox.Show("Danh sách sản phẩm trống. Vui lòng thêm sản phẩm trước khi bán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("LoadProducts failed in SalesForm", ex);
                MessageBox.Show("Không thể tải danh sách sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProduct.DataSource = new System.Collections.Generic.List<Product>();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var product = cmbProduct.SelectedItem as Product;
            if (product == null) return;

            var quantity = (int)nudQuantity.Value;
            if (quantity <= 0) return;

            var existing = _cart.FirstOrDefault(x => x.ProductId == product.ProductId);
            if (existing != null)
            {
                existing.Quantity += quantity;
                existing.LineTotal = existing.Quantity * existing.UnitPrice;
            }
            else
            {
                _cart.Add(new SalesCartItem
                {
                    ProductId = product.ProductId,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    Quantity = quantity,
                    UnitPrice = product.UnitPrice,
                    LineTotal = quantity * product.UnitPrice
                });
            }

            RefreshCart();
        }

        private void RefreshCart()
        {
            dgvCart.DataSource = null;
            dgvCart.DataSource = _cart.Select(x => new { x.ProductId, x.ProductCode, x.ProductName, x.Quantity, x.UnitPrice, x.LineTotal }).ToList();
            if (dgvCart.Columns.Count >= 6)
            {
                dgvCart.Columns["ProductId"].HeaderText = "Mã SP";
                dgvCart.Columns["ProductCode"].HeaderText = "Mã hàng";
                dgvCart.Columns["ProductName"].HeaderText = "Tên sản phẩm";
                dgvCart.Columns["Quantity"].HeaderText = "Số lượng";
                dgvCart.Columns["UnitPrice"].HeaderText = "Đơn giá";
                dgvCart.Columns["LineTotal"].HeaderText = "Thành tiền";
            }

            var subtotal = _cart.Sum(x => x.LineTotal);
            var discount = nudDiscount.Value;
            var vat = nudVat.Value;
            var total = subtotal - discount + (subtotal * vat / 100m);
            lblSubtotal.Text = "Tạm tính: " + subtotal.ToString("N0");
            lblTotal.Text = "Tổng tiền: " + total.ToString("N0");
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_cart.Count == 0)
                {
                    MessageBox.Show("Chưa có sản phẩm trong giỏ.");
                    return;
                }

                int? customerId = null;
                if (int.TryParse(txtCustomerId.Text.Trim(), out var parsedCustomerId))
                    customerId = parsedCustomerId;

                var orderId = _salesService.CreateInvoice(customerId, _currentUser.EmployeeId, nudDiscount.Value, nudVat.Value, _cart);
                MessageBox.Show("Tạo hóa đơn thành công. Mã hóa đơn: " + orderId);
                _cart.Clear();
                RefreshCart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tạo hóa đơn: " + ex.Message);
            }
        }
    }
}