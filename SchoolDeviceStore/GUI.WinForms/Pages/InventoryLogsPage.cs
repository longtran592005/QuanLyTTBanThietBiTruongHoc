using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLL;
using DAL.Repositories;
using DTO;

namespace GUI.WinForms
{
    public class InventoryLogsPage : UserControl
    {
        private readonly InventoryLogRepository _repo = new InventoryLogRepository();
        private readonly ProductService _productService = new ProductService();
        private readonly EmployeeService _employee_service = new EmployeeService();

        private Dictionary<int, Product> _productMap = new Dictionary<int, Product>();
        private Dictionary<int, string> _employeeMap = new Dictionary<int, string>();
        private int _totalCount = 0;
        private int _totalPages = 1;

        private readonly ComboBox _productFilter = new ComboBox();
        private readonly DateTimePicker _fromPicker = new DateTimePicker();
        private readonly DateTimePicker _toPicker = new DateTimePicker();
        private readonly Button _refreshButton = new Button();
        private readonly DataGridView _grid = new DataGridView();
        private readonly Button _prevButton = new Button();
        private readonly Button _nextButton = new Button();
        private readonly Label _pageLabel = new Label();

        private int _page = 1;
        private const int PageSize = 50;

        public InventoryLogsPage()
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;
            BuildLayout();
            LoadProducts();
            LoadLogs();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));

            // Toolbar
            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(8) };
            _productFilter.Width = 240;
            _productFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _fromPicker.Format = DateTimePickerFormat.Custom;
            _fromPicker.CustomFormat = "yyyy-MM-dd";
            _toPicker.Format = DateTimePickerFormat.Custom;
            _toPicker.CustomFormat = "yyyy-MM-dd";
            _refreshButton.Text = "Lọc";
            _refreshButton.Click += (s, e) => { _page = 1; LoadLogs(); };

            toolbar.Controls.Add(new Label { Text = "Sản phẩm:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(6,8,0,0) });
            toolbar.Controls.Add(_productFilter);
            toolbar.Controls.Add(new Label { Text = "Từ:", AutoSize = true, Padding = new Padding(6,8,0,0) });
            toolbar.Controls.Add(_fromPicker);
            toolbar.Controls.Add(new Label { Text = "Đến:", AutoSize = true, Padding = new Padding(6,8,0,0) });
            toolbar.Controls.Add(_toPicker);
            toolbar.Controls.Add(_refreshButton);

            // Grid
            DataGridViewHelper.ApplyProfessionalStyle(_grid);
            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.AllowUserToAddRows = false;

            // Paging
            var pager = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
            _nextButton.Text = "Tiếp";
            _prevButton.Text = "Trước";
            _nextButton.Click += (s, e) => { _page++; LoadLogs(); };
            _prevButton.Click += (s, e) => { if (_page > 1) _page--; LoadLogs(); };
            _pageLabel.AutoSize = true;
            pager.Controls.Add(_nextButton);
            pager.Controls.Add(_prevButton);
            pager.Controls.Add(_pageLabel);

            root.Controls.Add(toolbar, 0, 0);
            root.Controls.Add(_grid, 0, 1);
            root.Controls.Add(pager, 0, 2);
            Controls.Add(root);
        }

        private void LoadProducts()
        {
            var products = _productService.GetAll() ?? new List<Product>();
            _productMap = products.ToDictionary(p => p.ProductId, p => p);

            _productFilter.Items.Clear();
            _productFilter.Items.Add(new { Id = (int?)null, Name = "Tất cả" });
            foreach (var p in products)
                _productFilter.Items.Add(new { Id = (int?)p.ProductId, Name = p.ProductName + " (" + p.ProductCode + ")" });
            _productFilter.DisplayMember = "Name";
            _productFilter.SelectedIndex = 0;

            // Preload employees map for fast name lookup
            var emps = _employee_service.GetAll() ?? new List<DTO.Employee>();
            _employeeMap = emps.ToDictionary(e => e.EmployeeId, e => e.FullName);
        }

        private void LoadLogs()
        {
            try
            {
                int? productId = null;
                if (_productFilter.SelectedItem != null)
                {
                    var prop = _productFilter.SelectedItem.GetType().GetProperty("Id");
                    productId = (int?)prop.GetValue(_productFilter.SelectedItem);
                }

                DateTime? from = _fromPicker.Value.Date;
                DateTime? to = _toPicker.Value.Date.AddDays(1).AddSeconds(-1);

                var offset = (_page - 1) * PageSize;
                _totalCount = _repo.GetCountByFilter(productId, from, to);
                _totalPages = Math.Max(1, (int)Math.Ceiling(_totalCount / (double)PageSize));

                var logs = _repo.GetByFilter(productId, from, to, offset, PageSize);

                var view = logs.Select(l =>
                {
                    _productMap.TryGetValue(l.ProductId, out var prod);
                    var prodName = prod?.ProductName ?? l.ProductId.ToString();
                    var prodCode = prod?.ProductCode ?? "-";
                    var changedBy = l.ChangedBy.HasValue && _employeeMap.TryGetValue(l.ChangedBy.Value, out var name) ? name : (l.ChangedBy.HasValue ? l.ChangedBy.Value.ToString() : "-");
                    return new
                    {
                        l.InventoryLogId,
                        ProductName = prodName,
                        ProductCode = prodCode,
                        Time = l.ChangedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        Change = l.Change,
                        Reason = l.Reason,
                        ChangedBy = changedBy
                    };
                }).ToList();

                _grid.DataSource = view;
                _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                _pageLabel.Text = $"Trang {_page}/{_totalPages} (Tổng {_totalCount})";
                _nextButton.Enabled = _page < _totalPages;
                _prevButton.Enabled = _page > 1;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Failed to load inventory logs page", ex);
                UiDialogs.ShowError("Không thể tải dữ liệu lịch sử tồn kho.");
            }
        }
    }
}
