using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using BLL;
using DTO;

namespace GUI.WinForms
{
    public class CategoryManagementForm : Form
    {
        private DataGridView dgv;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private CategoryService _service = new CategoryService();

        public CategoryManagementForm()
        {
            InitializeComponents();
            LoadData();
        }

        private void InitializeComponents()
        {
            this.Text = "Quản lý danh mục";
            this.Width = 900;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterParent;
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
            
            btnAdd = UIHelper.CreatePrimaryButton("Thêm mới");
            btnAdd.Width = 110;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = UIHelper.CreateSecondaryButton("Sửa");
            btnEdit.Width = 90;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = UIHelper.CreateDangerButton("Xóa");
            btnDelete.Width = 90;
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = UIHelper.CreateSecondaryButton("Làm mới");
            btnRefresh.Width = 110;
            btnRefresh.Click += (s, e) => LoadData();

            toolbar.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnRefresh });

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

        private void LoadData()
        {
            try
            {
                var list = _service.GetAll();
                dgv.DataSource = list.Select(c => new { c.CategoryId, c.CategoryName, c.Description }).ToList();
                if (dgv.Columns.Count >= 3)
                {
                    dgv.Columns[nameof(Category.CategoryId)].HeaderText = "Mã DM";
                    dgv.Columns[nameof(Category.CategoryName)].HeaderText = "Tên danh mục";
                    dgv.Columns[nameof(Category.Description)].HeaderText = "Mô tả";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh mục: " + ex.Message);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var editor = new CategoryEditorForm();
            if (editor.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _service.Create(editor.Category);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm danh mục: " + ex.Message);
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Chọn danh mục để sửa."); return; }
            var id = Convert.ToInt32(dgv.SelectedRows[0].Cells["CategoryId"].Value);
            var cat = _service.GetById(id);
            var editor = new CategoryEditorForm(cat);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    editor.Category.CategoryId = id;
                    _service.Update(editor.Category);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sửa danh mục: " + ex.Message);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Chọn danh mục để xóa."); return; }
            var id = Convert.ToInt32(dgv.SelectedRows[0].Cells["CategoryId"].Value);
            if (MessageBox.Show("Xóa danh mục này?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                _service.Delete(id);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa danh mục: " + ex.Message);
            }
        }
    }
}
