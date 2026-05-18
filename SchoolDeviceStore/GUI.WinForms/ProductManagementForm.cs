using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLL;
using DTO;

namespace GUI.WinForms
{
    public class ProductManagementForm : Form
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private Button btnSearch, btnAdd, btnEdit, btnDelete, btnRefresh;
        private ProductService _service = new ProductService();

        public ProductManagementForm()
        {
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            this.Text = "Quản lý sản phẩm";
            this.Width = 1100;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
            UIHelper.ApplyFormTheme(this);
            FontHelper.ApplyVietnameseFontToForm(this);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(20),
                BackColor = UITheme.BackgroundColor
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            
            txtSearch = new TextBox { Width = 250, Margin = new Padding(0, 5, 10, 0) };
            UIHelper.StyleTextBox(txtSearch);
            
            btnSearch = UIHelper.CreatePrimaryButton("Tìm kiếm");
            btnSearch.Width = 100;
            btnSearch.Click += (s, e) => LoadData(txtSearch.Text.Trim());

            btnRefresh = UIHelper.CreateSecondaryButton("Làm mới");
            btnRefresh.Width = 100;
            btnRefresh.Click += (s, e) => { txtSearch.Text = ""; LoadData(); };

            btnAdd = UIHelper.CreatePrimaryButton("Thêm sản phẩm");
            btnAdd.Width = 150;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = UIHelper.CreateSecondaryButton("Sửa");
            btnEdit.Width = 80;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = UIHelper.CreateDangerButton("Xóa");
            btnDelete.Width = 80;
            btnDelete.Click += BtnDelete_Click;

            toolbar.Controls.AddRange(new Control[] { txtSearch, btnSearch, btnRefresh, btnAdd, btnEdit, btnDelete });

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            UIHelper.StyleDataGridView(dgv);

            root.Controls.Add(toolbar, 0, 0);
            root.Controls.Add(dgv, 0, 1);

            this.Controls.Add(root);
        }

        private void LoadData(string keyword = null)
        {
            List<Product> list;
            try
            {
                list = string.IsNullOrWhiteSpace(keyword) ? _service.GetAll() : _service.Search(keyword);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải sản phẩm: " + ex.Message, "Cảnh báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgv.DataSource = list.Select(p => new
            {
                p.ProductId,
                p.ProductCode,
                p.ProductName,
                p.Quantity,
                p.UnitPrice,
                p.PurchasePrice,
                p.Status
            }).ToList();
            if (dgv.Columns.Count >= 7)
            {
                dgv.Columns["ProductId"].HeaderText = "Mã SP";
                dgv.Columns["ProductCode"].HeaderText = "Mã hàng";
                dgv.Columns["ProductName"].HeaderText = "Tên sản phẩm";
                dgv.Columns["Quantity"].HeaderText = "Tồn kho";
                dgv.Columns["UnitPrice"].HeaderText = "Giá bán";
                dgv.Columns["PurchasePrice"].HeaderText = "Giá nhập";
                dgv.Columns["Status"].HeaderText = "Trạng thái";
            }
        }

        private Product GetSelectedProduct()
        {
            if (dgv.SelectedRows.Count == 0) return null;
            var id = Convert.ToInt32(dgv.SelectedRows[0].Cells["ProductId"].Value);
            return _service.GetById(id);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var editor = new ProductEditorForm();
            if (editor.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _service.Create(editor.Product);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tạo sản phẩm: " + ex.Message);
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedProduct();
            if (selected == null) { MessageBox.Show("Chọn sản phẩm để sửa."); return; }
            var editor = new ProductEditorForm(selected);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _service.Update(editor.Product);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật sản phẩm: " + ex.Message);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedProduct();
            if (selected == null) { MessageBox.Show("Chọn sản phẩm để xóa."); return; }
            var ok = MessageBox.Show($"Xóa sản phẩm {selected.ProductName}?", "Xác nhận", MessageBoxButtons.YesNo);
            if (ok != DialogResult.Yes) return;
            try
            {
                _service.Delete(selected.ProductId);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sản phẩm: " + ex.Message);
            }
        }
    }
}
