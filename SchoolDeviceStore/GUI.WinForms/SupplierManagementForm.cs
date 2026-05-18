using System;
using System.Linq;
using System.Windows.Forms;
using BLL;

namespace GUI.WinForms
{
    public class SupplierManagementForm : Form
    {
        private DataGridView dgv;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private SupplierService _service = new SupplierService();

        public SupplierManagementForm()
        {
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            this.Text = "Quản lý nhà cung cấp";
            this.Width = 900;
            this.Height = 520;
            this.StartPosition = FormStartPosition.CenterParent;
            FontHelper.ApplyVietnameseFontToForm(this);

            btnAdd = new Button { Text = "Thêm", Left = 20, Top = 20, Width = 100 };
            btnEdit = new Button { Text = "Sửa", Left = 130, Top = 20, Width = 100 };
            btnDelete = new Button { Text = "Xóa", Left = 240, Top = 20, Width = 100 };
            btnRefresh = new Button { Text = "Làm mới", Left = 350, Top = 20, Width = 100 };

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => LoadData();

            dgv = new DataGridView { Left = 20, Top = 60, Width = 840, Height = 420, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            this.Controls.Add(btnAdd); this.Controls.Add(btnEdit); this.Controls.Add(btnDelete); this.Controls.Add(btnRefresh); this.Controls.Add(dgv);
        }

        private void LoadData()
        {
            try
            {
                var list = _service.GetAll();
                dgv.DataSource = list.Select(s => new { s.SupplierId, s.SupplierName, s.ContactName, s.Phone, s.Email, s.Address }).ToList();
                if (dgv.Columns.Count >= 6)
                {
                    dgv.Columns[nameof(DTO.Supplier.SupplierId)].HeaderText = "Mã NCC";
                    dgv.Columns[nameof(DTO.Supplier.SupplierName)].HeaderText = "Tên NCC";
                    dgv.Columns[nameof(DTO.Supplier.ContactName)].HeaderText = "Người liên hệ";
                    dgv.Columns[nameof(DTO.Supplier.Phone)].HeaderText = "Điện thoại";
                    dgv.Columns[nameof(DTO.Supplier.Email)].HeaderText = "Email";
                    dgv.Columns[nameof(DTO.Supplier.Address)].HeaderText = "Địa chỉ";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải nhà cung cấp: " + ex.Message);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var editor = new SupplierEditorForm();
            if (editor.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _service.Create(editor.Supplier);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm nhà cung cấp: " + ex.Message);
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Chọn nhà cung cấp để sửa."); return; }
            var id = Convert.ToInt32(dgv.SelectedRows[0].Cells["SupplierId"].Value);
            var supplier = _service.GetById(id);
            var editor = new SupplierEditorForm(supplier);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    editor.Supplier.SupplierId = id;
                    _service.Update(editor.Supplier);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sửa nhà cung cấp: " + ex.Message);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Chọn nhà cung cấp để xóa."); return; }
            var id = Convert.ToInt32(dgv.SelectedRows[0].Cells["SupplierId"].Value);
            if (MessageBox.Show("Xóa nhà cung cấp này?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                _service.Delete(id);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa nhà cung cấp: " + ex.Message);
            }
        }
    }
}
