using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BLL;

namespace GUI.WinForms
{
    /// <summary>
    /// Promotion management page with CRUD operations, status filtering, and detailed promotion info.
    /// </summary>
    public class PromotionManagementPage : UserControl
    {
        private readonly PromotionService _service = new PromotionService();
        private readonly DataGridView _grid = new DataGridView();
        private readonly ComboBox _statusFilter = new ComboBox();
        private readonly Panel _detailPanel = new Panel();
        private readonly Label _detailTitle = new Label();
        private readonly Label _detailInfo = new Label();

        public PromotionManagementPage()
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;
            BuildLayout();
            _grid.CellFormatting += Grid_CellFormatting;
            Load += (s, e) => RefreshGrid();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UITheme.BackgroundColor
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 170F));
            root.Controls.Add(BuildToolbar(), 0, 0);
            root.Controls.Add(BuildGridCard(), 0, 1);
            root.Controls.Add(BuildDetailCard(), 0, 2);
            Controls.Add(root);
        }

        private Control BuildToolbar()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16, 8, 16, 8) };
            UIHelper.StyleCard(panel);

            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };

            var addBtn = UIHelper.CreatePrimaryButton("Tạo khuyến mãi");
            addBtn.Click += (s, e) => ShowPromotionEditor(null);
            layout.Controls.Add(addBtn);

            var editBtn = UIHelper.CreateSecondaryButton("Sửa");
            editBtn.Click += (s, e) => EditSelectedPromotion();
            layout.Controls.Add(editBtn);

            var deleteBtn = UIHelper.CreateOutlineDangerButton("Xóa");
            deleteBtn.Click += (s, e) => DeleteSelectedPromotion();
            layout.Controls.Add(deleteBtn);

            var statusLbl = new Label { Text = "  Trạng thái:", AutoSize = true, TextAlign = ContentAlignment.MiddleCenter, Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor, Margin = new Padding(12, 10, 4, 0) };
            layout.Controls.Add(statusLbl);

            _statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _statusFilter.Width = 160;
            _statusFilter.Items.AddRange(new object[] { "Tất cả", "Đang hoạt động", "Sắp diễn ra", "Đã hết hạn", "Ngừng hoạt động", "Đã hết lượt" });
            _statusFilter.SelectedIndex = 0;
            _statusFilter.SelectedIndexChanged += (s, e) => RefreshGrid();
            layout.Controls.Add(_statusFilter);

            panel.Controls.Add(layout);
            return panel;
        }

        private Control BuildGridCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16), Margin = new Padding(0, 12, 0, 0) };
            UIHelper.StyleCard(card);
            UIHelper.StyleDataGridView(_grid);
            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AllowUserToAddRows = false;
            _grid.SelectionChanged += (s, e) => ShowDetail();
            _grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) EditSelectedPromotion(); };
            card.Controls.Add(_grid);
            return card;
        }

        private Control BuildDetailCard()
        {
            _detailPanel.Dock = DockStyle.Fill;
            _detailPanel.BackColor = UITheme.SurfaceColor;
            _detailPanel.Padding = new Padding(16);
            UIHelper.StyleCard(_detailPanel);

            _detailTitle.Dock = DockStyle.Top;
            _detailTitle.Height = 28;
            _detailTitle.Font = UITheme.SectionTitleFont;
            _detailTitle.ForeColor = UITheme.TextPrimaryColor;
            _detailTitle.Text = "Chi tiết khuyến mãi";

            _detailInfo.Dock = DockStyle.Fill;
            _detailInfo.Font = UITheme.BaseFont;
            _detailInfo.ForeColor = UITheme.TextSecondaryColor;
            _detailInfo.Text = "Chọn một chương trình khuyến mãi để xem chi tiết.";

            _detailPanel.Controls.Add(_detailInfo);
            _detailPanel.Controls.Add(_detailTitle);
            return _detailPanel;
        }

        private void RefreshGrid()
        {
            try
            {
                var dt = _service.GetAllAsDataTable();
                var view = new DataView(dt);

                if (_statusFilter.SelectedIndex > 0)
                {
                    var status = _statusFilter.SelectedItem.ToString();
                    view.RowFilter = $"[Trạng thái] = '{status}'";
                }

                _grid.DataSource = view;

                if (_grid.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn col in _grid.Columns)
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    if (_grid.Columns.Contains("Tên chương trình"))
                        _grid.Columns["Tên chương trình"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Promotion grid refresh failed", ex);
                UiDialogs.ShowError("Không thể tải danh sách khuyến mãi.");
            }
        }

        private void ShowDetail()
        {
            if (_grid.SelectedRows.Count == 0)
            {
                _detailInfo.Text = "Chọn một chương trình khuyến mãi để xem chi tiết.";
                return;
            }

            var row = _grid.SelectedRows[0];
            var id = Convert.ToInt32(row.Cells["Mã KM"].Value);
            var promo = _service.GetById(id);
            if (promo == null) return;

            var discountDesc = promo.DiscountType == "Percentage"
                ? $"Giảm {promo.DiscountValue}%" + (promo.MaxDiscountAmount.HasValue ? $" (tối đa {promo.MaxDiscountAmount.Value:N0} ₫)" : "")
                : $"Giảm {promo.DiscountValue:N0} ₫";

            var usageDesc = promo.UsageLimit.HasValue
                ? $"{promo.UsageCount}/{promo.UsageLimit.Value} lần"
                : $"{promo.UsageCount} lần (không giới hạn)";

            var appliesDesc = promo.AppliesTo == "All" ? "Tất cả sản phẩm"
                : promo.AppliesTo == "Category" ? $"Danh mục #{promo.ApplyTargetId}"
                : $"Sản phẩm #{promo.ApplyTargetId}";

            _detailTitle.Text = $"{promo.PromotionName} ({promo.PromotionCode})";
            _detailInfo.Text = $"Mô tả: {promo.Description ?? "(không có)"}\n" +
                               $"Giảm giá: {discountDesc}\n" +
                               $"Đơn tối thiểu: {promo.MinOrderAmount:N0} ₫  |  Áp dụng cho: {appliesDesc}\n" +
                               $"Thời gian: {promo.StartDate:dd/MM/yyyy} → {promo.EndDate:dd/MM/yyyy}  |  Đã dùng: {usageDesc}\n" +
                               $"Trạng thái: {promo.StatusDisplay}";
        }

        private void ShowPromotionEditor(int? promotionId)
        {
            using (var dialog = new PromotionEditorForm(promotionId))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    RefreshGrid();
            }
        }

        private void EditSelectedPromotion()
        {
            if (_grid.SelectedRows.Count == 0) { UiDialogs.ShowWarning("Vui lòng chọn khuyến mãi."); return; }
            var id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Mã KM"].Value);
            ShowPromotionEditor(id);
        }

        private void DeleteSelectedPromotion()
        {
            if (_grid.SelectedRows.Count == 0) { UiDialogs.ShowWarning("Vui lòng chọn khuyến mãi."); return; }
            var name = _grid.SelectedRows[0].Cells["Tên chương trình"].Value.ToString();
            var id = Convert.ToInt32(_grid.SelectedRows[0].Cells["Mã KM"].Value);

            if (UiDialogs.Confirm($"Xóa chương trình khuyến mãi \"{name}\"?\n\nThao tác này không thể hoàn tác.", "Xác nhận xóa"))
            {
                try
                {
                    _service.DeletePromotion(id);
                    UiDialogs.ShowInfo("Đã xóa khuyến mãi.");
                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    AppLogger.Error("Delete promotion failed", ex);
                    UiDialogs.ShowError("Không thể xóa khuyến mãi.");
                }
            }
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var colName = _grid.Columns[e.ColumnIndex].Name;
                if (colName == "Giá trị" && e.Value != null)
                {
                    var row = _grid.Rows[e.RowIndex];
                    var type = row.Cells["Loại giảm giá"].Value?.ToString();
                    if (decimal.TryParse(e.Value.ToString(), out decimal val))
                    {
                        if (type == "Phần trăm (%)")
                        {
                            e.Value = $"{val:N0}%";
                        }
                        else
                        {
                            e.Value = $"{val:N0} đ";
                        }
                        e.FormattingApplied = true;
                    }
                }
                else if (colName == "Đơn tối thiểu" && e.Value != null)
                {
                    if (decimal.TryParse(e.Value.ToString(), out decimal val))
                    {
                        e.Value = $"{val:N0} đ";
                        e.FormattingApplied = true;
                    }
                }
                else if ((colName == "Ngày bắt đầu" || colName == "Ngày kết thúc") && e.Value != null)
                {
                    if (DateTime.TryParse(e.Value.ToString(), out DateTime date))
                    {
                        e.Value = date.ToString("dd/MM/yyyy");
                        e.FormattingApplied = true;
                    }
                }
            }
        }
    }
}
