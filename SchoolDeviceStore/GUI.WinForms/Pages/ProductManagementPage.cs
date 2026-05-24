using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLL;
using DTO;
using DAL.Repositories;

namespace GUI.WinForms
{
    public class ProductManagementPage : UserControl
    {
        private readonly ProductService _productService = new ProductService();
        private readonly CategoryService _categoryService = new CategoryService();
        private readonly SupplierService _supplierService = new SupplierService();
        private readonly DataGridView _grid = new DataGridView();
        private readonly TextBox _searchBox = new TextBox();
        private readonly ComboBox _statusFilter = new ComboBox();
        private readonly Label _recordCountLabel = new Label();
        private readonly Label _detailLabel = new Label();
        private readonly DataGridView _logGrid = new DataGridView();
        private readonly Button _logRefreshButton = new Button();
        private int? _currentProductId = null;
        private int _currentUserRoleId = 0;
        private readonly EmptyStateControl _emptyState = new EmptyStateControl();
        private List<Product> _items = new List<Product>();

        public ProductManagementPage(DTO.Employee currentUser = null)
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;
            if (currentUser != null)
                _currentUserRoleId = currentUser.RoleId;
            BuildLayout();
            LoadData();
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
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));

            root.Controls.Add(BuildToolbarAndFilters(), 0, 0);
            root.Controls.Add(BuildBody(), 0, 1);
            root.Controls.Add(BuildFooter(), 0, 2);
            Controls.Add(root);
        }

        private Control BuildToolbarAndFilters()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // Left: Action Buttons
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // Right: Filters

            // Left side: Buttons
            var flow = new FlowLayoutPanel 
            { 
                Dock = DockStyle.Fill, 
                FlowDirection = FlowDirection.LeftToRight, 
                WrapContents = false, 
                Padding = new Padding(0, 12, 0, 0)
            };
            var addButton = UIHelper.CreatePrimaryButton("Thêm");
            addButton.Click += (s, e) => AddItem();
            var editButton = UIHelper.CreateSecondaryButton("Sửa");
            editButton.Click += (s, e) => EditItem();
            var deleteButton = UIHelper.CreateOutlineDangerButton("Xóa");
            deleteButton.Click += (s, e) => DeleteItem();
            var refreshButton = UIHelper.CreateSecondaryButton("Làm mới");
            refreshButton.Click += (s, e) => LoadData();
            
            flow.Controls.AddRange(new Control[] { addButton, editButton, deleteButton, refreshButton });
            layout.Controls.Add(flow, 0, 0);

            // Right side: Search & Filter
            var filterLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(0, 12, 0, 0)
            };
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));

            _searchBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _searchBox.Height = 36;
            UIHelper.StyleTextBox(_searchBox);
            UIHelper.SetPlaceholder(_searchBox, "🔍 Tìm kiếm sản phẩm...");
            _searchBox.TextChanged += (s, e) => ApplyFilters();

            _statusFilter.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _statusFilter.Height = 36;
            _statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            UIHelper.StyleTextBox(_statusFilter);
            _statusFilter.Items.AddRange(new object[] { "Tất cả", "Còn hàng", "Hết hàng" });
            _statusFilter.SelectedIndex = 0;
            _statusFilter.SelectedIndexChanged += (s, e) => ApplyFilters();

            var exportCsvButton = UIHelper.CreateSecondaryButton("Xuất CSV");
            exportCsvButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            exportCsvButton.Height = 36;
            exportCsvButton.Click += (s, e) => ExportHelper.ExportDataGridViewToCsv(_grid, "products.csv");

            filterLayout.Controls.Add(_searchBox, 0, 0);
            filterLayout.Controls.Add(_statusFilter, 1, 0);
            filterLayout.Controls.Add(exportCsvButton, 2, 0);

            layout.Controls.Add(filterLayout, 1, 0);
            return layout;
        }

        private Control BuildBody()
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 760,
                BackColor = UITheme.BackgroundColor,
                FixedPanel = FixedPanel.Panel2
            };

            var gridCard = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(gridCard);
            DataGridViewHelper.ApplyProfessionalStyle(_grid);
            _grid.Dock = DockStyle.Fill;
            _grid.SelectionChanged += (s, e) => UpdateSelectionDetail();
            
            _emptyState.SetContent("Không tìm thấy sản phẩm", "Bắt đầu bằng cách tạo sản phẩm mới hoặc điều chỉnh bộ lọc tìm kiếm.");
            _emptyState.Dock = DockStyle.Fill;
            
            gridCard.Controls.Add(_emptyState);
            gridCard.Controls.Add(_grid);
            split.Panel1.Padding = new Padding(0, 0, 12, 0);
            split.Panel1.Controls.Add(gridCard);

            var detailTabs = new TabControl { Dock = DockStyle.Fill }; // will contain "Chi tiết" and "Lịch sử tồn kho"

            // Chi tiết tab
            var detailTab = new TabPage("Chi tiết");
            var detailCard = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(detailCard);
            var title = new Label { Dock = DockStyle.Top, Height = 28, Text = "Chi tiết sản phẩm", Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            _detailLabel.Dock = DockStyle.Fill;
            _detailLabel.Font = UITheme.BaseFont;
            _detailLabel.ForeColor = UITheme.TextSecondaryColor;
            _detailLabel.Text = "Chọn một sản phẩm để xem thông tin quản lý.";
            detailCard.Controls.Add(_detailLabel);
            detailCard.Controls.Add(title);
            detailTab.Controls.Add(detailCard);

            // Lịch sử tồn kho tab
            var logTab = new TabPage("Lịch sử tồn kho");
            var logPanelOuter = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(12) };
            UIHelper.StyleCard(logPanelOuter);

            // Top toolbar for log tab
            var logToolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 6, 0, 6) };
            _logRefreshButton.Text = "Làm mới";
            _logRefreshButton.Height = 30;
            _logRefreshButton.Click += (s, e) => { if (_currentProductId.HasValue) LoadInventoryLogs(_currentProductId.Value); };
            logToolbar.Controls.Add(_logRefreshButton);

            // Grid
            DataGridViewHelper.ApplyProfessionalStyle(_logGrid);
            _logGrid.Dock = DockStyle.Fill;
            _logGrid.ReadOnly = true;
            _logGrid.AllowUserToAddRows = false;
            _logGrid.AllowUserToDeleteRows = false;

            logPanelOuter.Controls.Add(_logGrid);
            logPanelOuter.Controls.Add(logToolbar);
            logTab.Controls.Add(logPanelOuter);

            detailTabs.TabPages.Add(detailTab);
            if (EmployeeService.HasPermission(_currentUserRoleId, "inventory_logs"))
                detailTabs.TabPages.Add(logTab);

            // When switching to log tab, load logs for current product
            detailTabs.SelectedIndexChanged += (s, e) =>
            {
                if (detailTabs.SelectedTab == logTab && _currentProductId.HasValue)
                {
                    LoadInventoryLogs(_currentProductId.Value);
                }
            };

            split.Panel2.Controls.Add(detailTabs);
            return split;
        }

        private void LoadInventoryLogs(int productId)
        {
            try
            {
                var repo = new InventoryLogRepository();
                var logs = repo.GetByProductId(productId);
                var empService = new EmployeeService();
                var emps = empService.GetAll().ToDictionary(x => x.EmployeeId, x => x.FullName);
                var view = logs.Select(l => new
                {
                    ChangedAt = l.ChangedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    Change = l.Change,
                    Reason = l.Reason,
                    ChangedBy = l.ChangedBy.HasValue ? (emps.TryGetValue(l.ChangedBy.Value, out var name) ? name : l.ChangedBy.Value.ToString()) : "-"
                }).ToList();

                _logGrid.DataSource = view;
                _logGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (_logGrid.Columns["ChangedAt"] != null)
                {
                    _logGrid.Columns["ChangedAt"].HeaderText = "Thời gian";
                    _logGrid.Columns["ChangedAt"].FillWeight = 30F;
                }
                if (_logGrid.Columns["Change"] != null)
                {
                    _logGrid.Columns["Change"].HeaderText = "Thay đổi";
                    _logGrid.Columns["Change"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    _logGrid.Columns["Change"].FillWeight = 15F;
                }
                if (_logGrid.Columns["Reason"] != null)
                {
                    _logGrid.Columns["Reason"].HeaderText = "Lý do";
                    _logGrid.Columns["Reason"].FillWeight = 40F;
                }
                if (_logGrid.Columns["ChangedBy"] != null)
                {
                    _logGrid.Columns["ChangedBy"].HeaderText = "Người thay đổi (ID)";
                    _logGrid.Columns["ChangedBy"].FillWeight = 15F;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Failed to load inventory logs", ex);
                UiDialogs.ShowError("Không thể tải lịch sử tồn kho.");
            }
        }

        private Control BuildFooter()
        {
            _recordCountLabel.Dock = DockStyle.Fill;
            _recordCountLabel.TextAlign = ContentAlignment.MiddleLeft;
            _recordCountLabel.Font = UITheme.CaptionFont;
            _recordCountLabel.ForeColor = UITheme.TextSecondaryColor;
            return _recordCountLabel;
        }

        private void LoadData()
        {
            try
            {
                _items = _productService.GetAll();
                if (_items == null || _items.Count == 0)
                {
                    MessageBox.Show("Danh sách sản phẩm rỗng. Vui lòng thêm sản phẩm.", "Thông báo");
                }
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải sản phẩm:\n" + ex.Message + "\n\nStackTrace:\n" + ex.StackTrace, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppLogger.Error("ProductManagementPage.LoadData failed", ex);
                _items = new List<Product>();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            IEnumerable<Product> filtered = _items;
            var keyword = _searchBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                filtered = filtered.Where(x =>
                    (x.ProductCode != null && x.ProductCode.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.ProductName != null && x.ProductName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            var status = _statusFilter.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(status) && status != "Tất cả")
            {
                if (status == "Còn hàng")
                {
                    filtered = filtered.Where(x => x.Quantity > 0 || string.Equals(x.Status, "Available", StringComparison.OrdinalIgnoreCase));
                }
                else if (status == "Hết hàng")
                {
                    filtered = filtered.Where(x => x.Quantity <= 0 || string.Equals(x.Status, "Out of Stock", StringComparison.OrdinalIgnoreCase));
                }
            }

            var view = filtered.Select(x => new
            {
                x.ProductId,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = DataGridViewHelper.FormatCurrencyVN(x.UnitPrice),
                PurchasePrice = DataGridViewHelper.FormatCurrencyVN(x.PurchasePrice),
                Status = (x.Status == "Available") ? "Còn hàng" : "Hết hàng"
            }).ToList();

            _grid.DataSource = view;
            
            // Clean up AutoGenerated Columns
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            if (_grid.Columns["ProductId"] != null)
                _grid.Columns["ProductId"].Visible = false;

            if (_grid.Columns["ProductCode"] != null)
                _grid.Columns["ProductCode"].HeaderText = "Mã sản phẩm";

            if (_grid.Columns["ProductName"] != null)
            {
                _grid.Columns["ProductName"].HeaderText = "Tên sản phẩm";
                _grid.Columns["ProductName"].FillWeight = 160F;
            }

            if (_grid.Columns["Quantity"] != null)
            {
                _grid.Columns["Quantity"].HeaderText = "Số lượng";
                _grid.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
                
            if (_grid.Columns["UnitPrice"] != null)
            {
                _grid.Columns["UnitPrice"].HeaderText = "Giá bán";
                _grid.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
                
            if (_grid.Columns["PurchasePrice"] != null)
            {
                _grid.Columns["PurchasePrice"].HeaderText = "Giá nhập";
                _grid.Columns["PurchasePrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (_grid.Columns["Status"] != null)
            {
                _grid.Columns["Status"].HeaderText = "Trạng thái";
                _grid.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            _grid.ClearSelection();
            
            DataGridViewHelper.ShowEmptyState(_grid, _emptyState, view.Count);
            _recordCountLabel.Text = $"Bản ghi: {view.Count}";
            UpdateSelectionDetail();
        }

        private Product GetSelectedProduct()
        {
            if (_grid.SelectedRows.Count == 0)
                return null;

            var id = Convert.ToInt32(_grid.SelectedRows[0].Cells["ProductId"].Value);
            return _productService.GetById(id);
        }

        private void UpdateSelectionDetail()
        {
            var selected = GetSelectedProduct();
            if (selected == null)
            {
                _detailLabel.Text = "Chọn một sản phẩm để xem thông tin quản lý.";
                return;
            }

            var category = selected.CategoryId.HasValue ? _categoryService.GetById(selected.CategoryId.Value) : null;
            var supplier = selected.SupplierId.HasValue ? _supplierService.GetById(selected.SupplierId.Value) : null;
            var stockStatus = selected.Quantity == 0 ? "Hết hàng" : (selected.Quantity <= 5 ? "Sắp hết" : "Còn hàng");
            
            _detailLabel.Text =
                $"Mã: {selected.ProductCode}\n" +
                $"Tên: {selected.ProductName}\n" +
                $"Số lượng: {selected.Quantity} sản phẩm ({stockStatus})\n" +
                $"Giá bán: {DataGridViewHelper.FormatCurrencyVN(selected.UnitPrice)}\n" +
                $"Giá nhập: {DataGridViewHelper.FormatCurrencyVN(selected.PurchasePrice)}\n" +
                $"Trạng thái: {selected.Status}\n" +
                $"Danh mục: {(category == null ? "-" : category.CategoryName)}\n" +
                $"Nhà cung cấp: {(supplier == null ? "-" : supplier.SupplierName)}";
        }

        private void AddItem()
        {
            try
            {
                using (var editor = new ProductEditorForm())
                {
                    var result = editor.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        _productService.Create(editor.Product);
                        MessageBox.Show("Đã thêm sản phẩm thành công.", "Thành công");
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm sản phẩm:\n" + ex.Message + "\n\nStackTrace:\n" + ex.StackTrace, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppLogger.Error("ProductManagementPage.AddItem failed", ex);
            }
        }

        private void EditItem()
        {
            var selected = GetSelectedProduct();
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để sửa.");
                return;
            }

            using (var editor = new ProductEditorForm(selected))
            {
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    _productService.Update(editor.Product);
                    LoadData();
                }
            }
        }

        private void DeleteItem()
        {
            var selected = GetSelectedProduct();
            if (selected == null)
            {
                UiDialogs.ShowWarning("Vui lòng chọn một sản phẩm để xóa.");
                return;
            }

            if (!UiDialogs.Confirm($"Bạn có chắc chắn muốn xóa '{selected.ProductName}'?\n\nThao tác này không thể hoàn tác.", "Xóa sản phẩm"))
            {
                return;
            }

            try
            {
                _productService.Delete(selected.ProductId);
                LoadData();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Product deletion failed", ex);
                UiDialogs.ShowError("Không thể xóa sản phẩm. Vui lòng thử lại.");
            }
        }

    }
}
