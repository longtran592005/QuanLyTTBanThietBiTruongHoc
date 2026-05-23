using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLL;
using DTO;

namespace GUI.WinForms
{
    public class SupplierManagementPage : UserControl
    {
        private readonly SupplierService _service = new SupplierService();
        private readonly DataGridView _grid = new DataGridView();
        private readonly Label _details = new Label();
        private readonly Label _footer = new Label();
        private readonly EmptyStateControl _emptyState = new EmptyStateControl();
        private List<Supplier> _items = new List<Supplier>();

        public SupplierManagementPage()
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;
            BuildLayout();
            LoadData();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = UITheme.BackgroundColor };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            root.Controls.Add(BuildToolbar(), 0, 0);
            root.Controls.Add(BuildBody(), 0, 1);
            root.Controls.Add(BuildFooter(), 0, 2);
            Controls.Add(root);
        }

        private Control BuildToolbar()
        {
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Padding = new Padding(0, 10, 0, 0) };
            var add = UIHelper.CreatePrimaryButton("Thêm"); add.Click += (s, e) => AddItem();
            var edit = UIHelper.CreateSecondaryButton("Sửa"); edit.Click += (s, e) => EditItem();
            var delete = UIHelper.CreateOutlineDangerButton("Xóa"); delete.Click += (s, e) => DeleteItem();
            var refresh = UIHelper.CreateSecondaryButton("Làm mới"); refresh.Click += (s, e) => LoadData();
            flow.Controls.AddRange(new Control[] { add, edit, delete, refresh });
            return flow;
        }

        private Control BuildBody()
        {
            var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 680, BackColor = UITheme.BackgroundColor };
            var gridCard = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(gridCard);
            UIHelper.StyleDataGridView(_grid);
            _grid.Dock = DockStyle.Fill;
            _grid.SelectionChanged += (s, e) => UpdateDetails();
            
            _emptyState.SetContent("Chưa có nhà cung cấp", "Thêm nhà cung cấp mới để bắt đầu.");
            _emptyState.Dock = DockStyle.Fill;
            
            gridCard.Controls.Add(_emptyState);
            gridCard.Controls.Add(_grid);
            split.Panel1.Padding = new Padding(0, 0, 6, 0);
            split.Panel1.Controls.Add(gridCard);

            var detailCard = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(detailCard);
            var title = new Label { Dock = DockStyle.Top, Height = 28, Text = "Chi tiết nhà cung cấp", Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            _details.Dock = DockStyle.Fill;
            _details.ForeColor = UITheme.TextSecondaryColor;
            detailCard.Controls.Add(_details);
            detailCard.Controls.Add(title);
            split.Panel2.Padding = new Padding(6, 0, 0, 0);
            split.Panel2.Controls.Add(detailCard);
            return split;
        }

        private Control BuildFooter()
        {
            _footer.Dock = DockStyle.Fill;
            _footer.ForeColor = UITheme.TextSecondaryColor;
            _footer.Font = UITheme.CaptionFont;
            _footer.TextAlign = ContentAlignment.MiddleLeft;
            return _footer;
        }

        private void LoadData()
        {
            _items = _service.GetAll();
            _grid.DataSource = _items.Select(x => new { x.SupplierId, x.SupplierName, x.ContactName, x.Phone, x.Email, x.Address }).ToList();
            if (_grid.Columns.Count >= 6)
            {
                _grid.Columns["SupplierId"].HeaderText = "Mã NCC";
                _grid.Columns["SupplierName"].HeaderText = "Tên NCC";
                _grid.Columns["ContactName"].HeaderText = "Người liên hệ";
                _grid.Columns["Phone"].HeaderText = "Điện thoại";
                _grid.Columns["Email"].HeaderText = "Email";
                _grid.Columns["Address"].HeaderText = "Địa chỉ";
            }
            DataGridViewHelper.ApplyProfessionalStyle(_grid);
            DataGridViewHelper.ShowEmptyState(_grid, _emptyState, _items.Count);
            _footer.Text = $"Bản ghi: {_items.Count}";
            UpdateDetails();
        }

        private Supplier GetSelected()
        {
            if (_grid.SelectedRows.Count == 0) return null;
            var id = Convert.ToInt32(_grid.SelectedRows[0].Cells["SupplierId"].Value);
            return _service.GetById(id);
        }

        private void UpdateDetails()
        {
            var selected = GetSelected();
            _details.Text = selected == null ? "Chọn một nhà cung cấp để xem chi tiết." : $"Tên: {selected.SupplierName}\nNgười liên hệ: {selected.ContactName}\nĐiện thoại: {selected.Phone}\nEmail: {selected.Email}\nĐịa chỉ: {selected.Address}";
        }

        private void AddItem()
        {
            using (var editor = new SupplierEditorForm())
            {
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    _service.Create(editor.Supplier);
                    LoadData();
                }
            }
        }

        private void EditItem()
        {
            var selected = GetSelected();
            if (selected == null) return;
            using (var editor = new SupplierEditorForm(selected))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    editor.Supplier.SupplierId = selected.SupplierId;
                    _service.Update(editor.Supplier);
                    LoadData();
                }
            }
        }

        private void DeleteItem()
        {
            var selected = GetSelected();
            if (selected == null) return;
            
            if (!UiDialogs.Confirm($"Bạn có chắc chắn muốn xóa nhà cung cấp \"{selected.SupplierName}\"?\n\nThao tác này không thể hoàn tác.", "Xóa nhà cung cấp"))
                return;

            try
            {
                _service.Delete(selected.SupplierId);
                LoadData();
                UiDialogs.ShowSuccess("Đã xóa nhà cung cấp thành công.", "Xóa thành công");
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Failed to delete supplier {selected.SupplierId}", ex);
                UiDialogs.ShowError($"Không thể xóa nhà cung cấp.\n\n{ex.Message}", "Xóa thất bại");
            }
        }
    }
}
