using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BLL;
using DAL;

namespace GUI.WinForms
{
    public class ReportsPage : UserControl
    {
        private readonly ReportService _service = new ReportService();
        private readonly DateTimePicker _fromPicker = new DateTimePicker();
        private readonly DateTimePicker _toPicker = new DateTimePicker();
        private readonly DataGridView _revenueGrid = new DataGridView();
        private readonly DataGridView _topProductsGrid = new DataGridView();
        private readonly DataGridView _alertsGrid = new DataGridView();
        private readonly Chart _revenueChart = new Chart();
        private readonly Chart _categoryChart = new Chart();
        private readonly Label _totalRevenueLabel = new Label();
        private readonly Label _salesCountLabel = new Label();
        private readonly Label _avgOrderValueLabel = new Label();
        private readonly Label _revenueGrowthLabel = new Label();
        private readonly Label _salesGrowthLabel = new Label();
        private readonly Label _avgGrowthLabel = new Label();
        private readonly ComboBox _categoryFilter = new ComboBox();
        private readonly ComboBox _employeeFilter = new ComboBox();
        private readonly ComboBox _chartTypeCombo = new ComboBox();
        private readonly EmptyStateControl _emptyState = new EmptyStateControl();

        // Store KPI data for export
        private ReportKpiData _currentKpi;
        private ReportKpiData _prevKpi;

        private TabControl _tabControl;

        public ReportsPage()
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;
            BuildLayout();
            _revenueGrid.CellFormatting += RevenueGrid_CellFormatting;
            Load += (s, e) => LoadReports();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 1, BackColor = UITheme.BackgroundColor };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.Normal,
                HotTrack = true,
                Padding = new Point(14, 6)
            };

            var overviewTab = new TabPage("Tổng quan") { BackColor = UITheme.BackgroundColor, Padding = new Padding(0) };
            var detailsTab = new TabPage("Chi tiết") { BackColor = UITheme.BackgroundColor, Padding = new Padding(0) };

            overviewTab.Controls.Add(BuildOverviewLayout());
            detailsTab.Controls.Add(BuildDetailsLayout());

            _tabControl.TabPages.Add(overviewTab);
            _tabControl.TabPages.Add(detailsTab);

            root.Controls.Add(_tabControl, 0, 0);
            Controls.Add(root);
        }

        private Control BuildOverviewLayout()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, BackColor = UITheme.BackgroundColor, Padding = new Padding(0) };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.Controls.Add(BuildQuickFilters(), 0, 0);
            layout.Controls.Add(BuildFilterBar(), 0, 1);
            layout.Controls.Add(BuildKpiCards(), 0, 2);
            layout.Controls.Add(BuildChartRow(), 0, 3);
            return layout;
        }

        private Control BuildDetailsLayout()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 1, BackColor = UITheme.BackgroundColor, Padding = new Padding(0, 8, 0, 0) };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.Controls.Add(BuildGridRow(), 0, 0);
            return layout;
        }

        // ── Quick Filter Buttons ──
        private Control BuildQuickFilters()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16, 6, 16, 6) };
            UIHelper.StyleCard(panel);
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };

            var lbl = new Label { Text = "Lọc nhanh:", AutoSize = true, Font = UITheme.BodyBoldFont, ForeColor = UITheme.TextPrimaryColor, Margin = new Padding(0, 8, 8, 0) };
            flow.Controls.Add(lbl);

            string[] labels = { "Hôm nay", "Hôm qua", "7 ngày", "Tháng này", "Quý này", "Năm nay" };
            foreach (var text in labels)
            {
                var btn = new Button
                {
                    Text = text, Width = 80, Height = 30, FlatStyle = FlatStyle.Flat,
                    Font = UITheme.CaptionFont, ForeColor = UITheme.PrimaryColor, BackColor = UITheme.SurfaceColor,
                    Margin = new Padding(2, 4, 2, 0), Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderColor = UITheme.BorderColor;
                var capturedText = text;
                btn.Click += (s, e) => ApplyQuickFilter(capturedText);
                flow.Controls.Add(btn);
            }

            panel.Controls.Add(flow);
            return panel;
        }

        private void ApplyQuickFilter(string filterName)
        {
            var today = DateTime.Today;
            switch (filterName)
            {
                case "Hôm nay": _fromPicker.Value = today; _toPicker.Value = today; break;
                case "Hôm qua": _fromPicker.Value = today.AddDays(-1); _toPicker.Value = today.AddDays(-1); break;
                case "7 ngày": _fromPicker.Value = today.AddDays(-7); _toPicker.Value = today; break;
                case "Tháng này": _fromPicker.Value = new DateTime(today.Year, today.Month, 1); _toPicker.Value = today; break;
                case "Quý này":
                    int qm = ((today.Month - 1) / 3) * 3 + 1;
                    _fromPicker.Value = new DateTime(today.Year, qm, 1); _toPicker.Value = today; break;
                case "Năm nay": _fromPicker.Value = new DateTime(today.Year, 1, 1); _toPicker.Value = today; break;
            }
            LoadReports();
        }

        // ── Advanced Filter Bar ──
        private Control BuildFilterBar()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16, 6, 16, 6) };
            UIHelper.StyleCard(panel);
            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };

            layout.Controls.Add(MakeLabeledPicker("Từ ngày", _fromPicker));
            layout.Controls.Add(MakeLabeledPicker("Đến ngày", _toPicker));

            // Category filter
            _categoryFilter.DropDownStyle = ComboBoxStyle.DropDownList; _categoryFilter.Width = 140;
            _categoryFilter.Margin = new Padding(8, 8, 0, 0);
            layout.Controls.Add(MakeLabeledCombo("Danh mục", _categoryFilter));

            // Employee filter
            _employeeFilter.DropDownStyle = ComboBoxStyle.DropDownList; _employeeFilter.Width = 150;
            _employeeFilter.Margin = new Padding(8, 8, 0, 0);
            layout.Controls.Add(MakeLabeledCombo("Nhân viên BH", _employeeFilter));

            var loadBtn = UIHelper.CreatePrimaryButton("Tải báo cáo");
            loadBtn.Margin = new Padding(12, 8, 0, 0);
            loadBtn.Click += (s, e) => LoadReports();
            layout.Controls.Add(loadBtn);

            // Export buttons
            var csvBtn = UIHelper.CreateSecondaryButton("CSV");
            csvBtn.Width = 55; csvBtn.Margin = new Padding(4, 8, 0, 0);
            csvBtn.Click += (s, e) => ExportHelper.ExportDataGridViewToCsv(_revenueGrid, "revenue-report.csv");
            layout.Controls.Add(csvBtn);

            var xlsBtn = UIHelper.CreateSecondaryButton("Excel");
            xlsBtn.Width = 55; xlsBtn.Margin = new Padding(4, 8, 0, 0);
            xlsBtn.Click += (s, e) => ExportHelper.ExportDataGridViewToExcelFriendlyXls(_revenueGrid, "revenue-report.xls");
            layout.Controls.Add(xlsBtn);

            var pdfBtn = UIHelper.CreateSecondaryButton("PDF");
            pdfBtn.Width = 55; pdfBtn.Margin = new Padding(4, 8, 0, 0);
            pdfBtn.Click += (s, e) => ExportPdf();
            layout.Controls.Add(pdfBtn);

            _fromPicker.Value = DateTime.Today.AddDays(-30);
            _toPicker.Value = DateTime.Today;

            // Load filter dropdowns
            LoadFilterDropdowns();

            panel.Controls.Add(layout);
            return panel;
        }

        private void LoadFilterDropdowns()
        {
            try
            {
                // Categories
                _categoryFilter.Items.Clear();
                _categoryFilter.Items.Add("Tất cả");
                var cats = DbHelper.ExecuteQuery("SELECT CategoryId, CategoryName FROM Categories ORDER BY CategoryName");
                foreach (DataRow r in cats.Rows) _categoryFilter.Items.Add(new ComboItem(Convert.ToInt32(r["CategoryId"]), r["CategoryName"].ToString()));
                _categoryFilter.SelectedIndex = 0;

                // Employees
                _employeeFilter.Items.Clear();
                _employeeFilter.Items.Add("Tất cả");
                var emps = _service.GetSalesEmployees();
                foreach (DataRow r in emps.Rows) _employeeFilter.Items.Add(new ComboItem(Convert.ToInt32(r["EmployeeId"]), r["FullName"].ToString()));
                _employeeFilter.SelectedIndex = 0;
            }
            catch { /* Ignore filter load errors */ }
        }

        private Control MakeLabeledPicker(string label, DateTimePicker picker)
        {
            var wrapper = new Panel { Width = 180, Height = 42, Margin = new Padding(0, 0, 8, 0) };
            var text = new Label { Dock = DockStyle.Top, Height = 16, Text = label, ForeColor = UITheme.TextSecondaryColor, Font = UITheme.CaptionFont };
            picker.Dock = DockStyle.Bottom;
            wrapper.Controls.Add(picker);
            wrapper.Controls.Add(text);
            return wrapper;
        }

        private Control MakeLabeledCombo(string label, ComboBox combo)
        {
            var wrapper = new Panel { Width = combo.Width + 4, Height = 42, Margin = new Padding(4, 0, 4, 0) };
            var text = new Label { Dock = DockStyle.Top, Height = 16, Text = label, ForeColor = UITheme.TextSecondaryColor, Font = UITheme.CaptionFont };
            combo.Dock = DockStyle.Bottom;
            wrapper.Controls.Add(combo);
            wrapper.Controls.Add(text);
            return wrapper;
        }

        // ── KPI Cards with Growth ──
        private Control BuildKpiCards()
        {
            var container = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.BackgroundColor, Padding = new Padding(0, 4, 0, 4) };
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            layout.Controls.Add(BuildKpiCard("Tổng doanh thu", _totalRevenueLabel, _revenueGrowthLabel, UITheme.PrimaryColor), 0, 0);
            layout.Controls.Add(BuildKpiCard("Tổng số hóa đơn", _salesCountLabel, _salesGrowthLabel, UITheme.SuccessColor), 1, 0);
            layout.Controls.Add(BuildKpiCard("Giá trị đơn TB", _avgOrderValueLabel, _avgGrowthLabel, UITheme.WarningColor), 2, 0);
            container.Controls.Add(layout);
            return container;
        }

        private Panel BuildKpiCard(string title, Label valueLabel, Label growthLabel, Color accentColor)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16, 12, 16, 12), Margin = new Padding(0, 0, 8, 0) };
            UIHelper.StyleCard(card);
            
            var titleLabel = new Label { Dock = DockStyle.Top, Height = 18, Text = title, Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor, Margin = Padding.Empty };
            
            valueLabel.Dock = DockStyle.Top; 
            valueLabel.Height = 32; 
            valueLabel.Font = UITheme.TitleFont; 
            valueLabel.ForeColor = accentColor; 
            valueLabel.Text = "0"; 
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;
            valueLabel.Margin = Padding.Empty;
            
            growthLabel.Dock = DockStyle.Top; 
            growthLabel.Height = 18; 
            growthLabel.Font = UITheme.CaptionFont; 
            growthLabel.ForeColor = UITheme.TextSecondaryColor; 
            growthLabel.Text = ""; 
            growthLabel.TextAlign = ContentAlignment.MiddleLeft;
            growthLabel.Margin = Padding.Empty;

            card.Controls.Add(growthLabel);
            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);
            return card;
        }

        // ── Chart Row ──
        private Control BuildChartRow()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = UITheme.BackgroundColor, Padding = new Padding(0, 8, 0, 0) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            layout.Controls.Add(BuildRevenueChartCard(), 0, 0);
            layout.Controls.Add(BuildCategoryChartCard(), 1, 0);
            return layout;
        }

        private Control BuildRevenueChartCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(card);

            // Header with chart type toggle
            var header = new Panel { Dock = DockStyle.Top, Height = 32, BackColor = Color.Transparent };
            var title = new Label { Dock = DockStyle.Left, Width = 200, Text = "Biểu đồ doanh thu", Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            _chartTypeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            _chartTypeCombo.Items.AddRange(new object[] { "Cột", "Đường", "Vùng" });
            _chartTypeCombo.SelectedIndex = 0;
            _chartTypeCombo.Width = 80;
            _chartTypeCombo.Dock = DockStyle.Right;
            _chartTypeCombo.SelectedIndexChanged += (s, e) => UpdateChartType();
            header.Controls.Add(_chartTypeCombo);
            header.Controls.Add(title);

            _revenueChart.Dock = DockStyle.Fill;
            _revenueChart.BackColor = Color.White;
            _revenueChart.MinimumSize = new Size(100, 100);
            _revenueChart.ChartAreas.Clear(); _revenueChart.Series.Clear(); _revenueChart.Legends.Clear();
            var area = new ChartArea("Main");
            area.AxisX.MajorGrid.LineColor = UITheme.BorderColor;
            area.AxisY.MajorGrid.LineColor = UITheme.BorderColor;
            _revenueChart.ChartAreas.Add(area);
            _revenueChart.Series.Add(new Series("Revenue") { ChartType = SeriesChartType.Column, ChartArea = "Main", Color = UITheme.PrimaryColor });
            _revenueChart.Series.Add(new Series("PrevRevenue") { ChartType = SeriesChartType.Column, ChartArea = "Main", Color = Color.FromArgb(80, UITheme.PrimaryColor), IsVisibleInLegend = true, LegendText = "Kỳ trước" });
            var legend = new Legend("Main") { Docking = Docking.Top, Font = UITheme.CaptionFont };
            _revenueChart.Legends.Add(legend);
            _revenueChart.Series["Revenue"].LegendText = "Kỳ hiện tại";
            _revenueChart.FormatNumber += (s, e) =>
            {
                if (e.ElementType == ChartElementType.AxisLabels && e.Value >= 1000000)
                    e.LocalizedValue = (e.Value / 1000000D).ToString("0.#") + " Tr";
            };

            card.Controls.Add(_revenueChart);
            card.Controls.Add(header);
            return card;
        }

        private Control BuildCategoryChartCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16), Margin = new Padding(8, 0, 0, 0) };
            UIHelper.StyleCard(card);
            var title = new Label { Dock = DockStyle.Top, Height = 28, Text = "Cơ cấu danh mục", Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            _categoryChart.Dock = DockStyle.Fill;
            _categoryChart.BackColor = Color.White;
            _categoryChart.ChartAreas.Clear(); _categoryChart.Series.Clear(); _categoryChart.Legends.Clear();
            _categoryChart.ChartAreas.Add(new ChartArea("Donut"));
            var donut = new Series("Categories") { ChartType = SeriesChartType.Doughnut, ChartArea = "Donut" };
            donut["DoughnutRadius"] = "40";
            donut["PieLabelStyle"] = "Outside";
            _categoryChart.Series.Add(donut);
            var leg = new Legend("Cat") { Docking = Docking.Bottom, Font = UITheme.CaptionFont };
            _categoryChart.Legends.Add(leg);
            card.Controls.Add(_categoryChart);
            card.Controls.Add(title);
            return card;
        }

        private void UpdateChartType()
        {
            var type = _chartTypeCombo.SelectedIndex;
            var chartType = type == 0 ? SeriesChartType.Column : type == 1 ? SeriesChartType.Line : SeriesChartType.SplineArea;
            _revenueChart.Series["Revenue"].ChartType = chartType;
            _revenueChart.Series["PrevRevenue"].ChartType = chartType;
        }

        // ── Grid Row with Alerts ──
        private Control BuildGridRow()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, BackColor = UITheme.BackgroundColor, Padding = new Padding(0, 8, 0, 0) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27F));
            layout.Controls.Add(BuildSortableGridCard("Doanh thu theo ngày", _revenueGrid), 0, 0);
            layout.Controls.Add(BuildSortableGridCard("Sản phẩm bán chạy", _topProductsGrid), 1, 0);
            layout.Controls.Add(BuildAlertCard(), 2, 0);
            return layout;
        }

        private Control BuildSortableGridCard(string title, DataGridView grid)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(12), Margin = new Padding(0, 0, 8, 0) };
            UIHelper.StyleCard(card);
            var header = new Label { Dock = DockStyle.Top, Height = 26, Text = title, Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            UIHelper.StyleDataGridView(grid);
            grid.Dock = DockStyle.Fill;
            // Enable sorting
            grid.ColumnHeaderMouseClick += (s, e) =>
            {
                if (grid.DataSource is DataTable dt)
                {
                    var col = grid.Columns[e.ColumnIndex];
                    var current = grid.Tag as string;
                    var dir = current == col.Name + "_ASC" ? "DESC" : "ASC";
                    dt.DefaultView.Sort = col.DataPropertyName + " " + dir;
                    grid.Tag = col.Name + "_" + dir;
                }
            };
            card.Controls.Add(grid);
            card.Controls.Add(header);
            return card;
        }

        private Control BuildAlertCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(12) };
            UIHelper.StyleCard(card);
            var title = new Label { Dock = DockStyle.Top, Height = 26, Text = "⚠ Cảnh báo tồn kho", Font = UITheme.SectionTitleFont, ForeColor = UITheme.ErrorColor };
            UIHelper.StyleDataGridView(_alertsGrid);
            _alertsGrid.Dock = DockStyle.Fill;
            card.Controls.Add(_alertsGrid);
            card.Controls.Add(title);
            return card;
        }

        // ── Data Loading ──
        private void LoadReports()
        {
            try
            {
                var from = _fromPicker.Value.Date;
                var to = _toPicker.Value.Date;

                int? catId = (_categoryFilter.SelectedItem is ComboItem ci) ? ci.Id : (int?)null;
                int? empId = (_employeeFilter.SelectedItem is ComboItem ei) ? ei.Id : (int?)null;

                // Revenue by day (filtered)
                var revenueTable = _service.GetRevenueByDayFiltered(from, to, catId, null, empId);
                _revenueGrid.DataSource = revenueTable;

                if (_revenueGrid.Columns.Contains("ReportDate"))
                {
                    _revenueGrid.Columns["ReportDate"].HeaderText = "Ngày";
                    _revenueGrid.Columns["ReportDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (_revenueGrid.Columns.Contains("Revenue"))
                {
                    _revenueGrid.Columns["Revenue"].HeaderText = "Doanh thu";
                    _revenueGrid.Columns["Revenue"].DefaultCellStyle.Format = "#,##0 đ";
                    _revenueGrid.Columns["Revenue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (_revenueGrid.Columns.Contains("OrderCount"))
                {
                    _revenueGrid.Columns["OrderCount"].HeaderText = "Số đơn";
                    _revenueGrid.Columns["OrderCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (_revenueGrid.Columns.Contains("SubTotal")) _revenueGrid.Columns["SubTotal"].Visible = false;
                if (_revenueGrid.Columns.Contains("Discount")) _revenueGrid.Columns["Discount"].Visible = false;
                if (_revenueGrid.Columns.Contains("VAT")) _revenueGrid.Columns["VAT"].Visible = false;

                // Chart - current period
                var currentSeries = _revenueChart.Series["Revenue"];
                currentSeries.Points.Clear();
                foreach (DataRow row in revenueTable.Rows)
                {
                    currentSeries.Points.AddXY(Convert.ToDateTime(row["ReportDate"]).ToString("dd/MM"), Convert.ToDecimal(row["Revenue"]));
                }

                // Chart - previous period comparison
                var span = to - from;
                var prevFrom = from.AddDays(-span.TotalDays);
                var prevTo = from;
                var prevRevenueTable = _service.GetRevenueByDay(prevFrom, prevTo);
                var prevSeries = _revenueChart.Series["PrevRevenue"];
                prevSeries.Points.Clear();
                foreach (DataRow row in prevRevenueTable.Rows)
                {
                    prevSeries.Points.AddXY(Convert.ToDateTime(row["ReportDate"]).ToString("dd/MM"), Convert.ToDecimal(row["Revenue"]));
                }

                // Top products
                _topProductsGrid.DataSource = _service.GetTopProducts(from, to);

                if (_topProductsGrid.Columns.Contains("ProductCode")) _topProductsGrid.Columns["ProductCode"].Visible = false;
                if (_topProductsGrid.Columns.Contains("ProductName")) _topProductsGrid.Columns["ProductName"].HeaderText = "Sản phẩm";
                if (_topProductsGrid.Columns.Contains("SoldQuantity"))
                {
                    _topProductsGrid.Columns["SoldQuantity"].HeaderText = "Đã bán";
                    _topProductsGrid.Columns["SoldQuantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (_topProductsGrid.Columns.Contains("SalesAmount"))
                {
                    _topProductsGrid.Columns["SalesAmount"].HeaderText = "Doanh số";
                    _topProductsGrid.Columns["SalesAmount"].DefaultCellStyle.Format = "#,##0 đ";
                    _topProductsGrid.Columns["SalesAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                // Category donut chart
                var catData = _service.GetRevenueByCategory(from, to);
                var donutSeries = _categoryChart.Series["Categories"];
                donutSeries.Points.Clear();
                Color[] palette = { UITheme.PrimaryColor, UITheme.SuccessColor, UITheme.WarningColor, UITheme.ErrorColor, Color.FromArgb(139, 92, 246), Color.FromArgb(236, 72, 153), Color.FromArgb(14, 165, 233) };
                int ci2 = 0;
                foreach (DataRow row in catData.Rows)
                {
                    var pt = donutSeries.Points.AddXY(row["CategoryName"].ToString(), Convert.ToDecimal(row["Revenue"]));
                    donutSeries.Points[pt].Color = palette[ci2 % palette.Length];
                    ci2++;
                }

                // KPIs with growth
                _currentKpi = _service.GetKpis(from, to);
                _prevKpi = _service.GetPreviousPeriodKpis(from, to);

                _totalRevenueLabel.Text = DataGridViewHelper.FormatCurrencyVN(_currentKpi.TotalRevenue);
                _salesCountLabel.Text = _currentKpi.OrderCount.ToString();
                _avgOrderValueLabel.Text = DataGridViewHelper.FormatCurrencyVN(_currentKpi.AvgOrderValue);

                SetGrowthLabel(_revenueGrowthLabel, _currentKpi.TotalRevenue, _prevKpi.TotalRevenue);
                SetGrowthLabel(_salesGrowthLabel, _currentKpi.OrderCount, _prevKpi.OrderCount);
                SetGrowthLabel(_avgGrowthLabel, _currentKpi.AvgOrderValue, _prevKpi.AvgOrderValue);

                // Alerts
                _alertsGrid.DataSource = _service.GetAlerts();

                if (_alertsGrid.Columns.Contains("AlertType")) _alertsGrid.Columns["AlertType"].HeaderText = "Loại";
                if (_alertsGrid.Columns.Contains("ProductCode")) _alertsGrid.Columns["ProductCode"].Visible = false;
                if (_alertsGrid.Columns.Contains("ProductName")) _alertsGrid.Columns["ProductName"].HeaderText = "Sản phẩm";
                if (_alertsGrid.Columns.Contains("CurrentValue"))
                {
                    _alertsGrid.Columns["CurrentValue"].HeaderText = "Tồn";
                    _alertsGrid.Columns["CurrentValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (_alertsGrid.Columns.Contains("Detail")) _alertsGrid.Columns["Detail"].Visible = false;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Report load failed", ex);
                UiDialogs.ShowError("Không thể tải dữ liệu báo cáo.");
            }
        }

        private void SetGrowthLabel(Label label, decimal current, decimal previous)
        {
            if (previous == 0) { label.Text = "N/A kỳ trước"; label.ForeColor = UITheme.TextSecondaryColor; return; }
            var pct = ((current - previous) / previous) * 100m;
            var arrow = pct >= 0 ? "↑" : "↓";
            label.Text = $"{arrow} {Math.Abs(pct):F1}% so với kỳ trước";
            label.ForeColor = pct >= 0 ? UITheme.SuccessColor : UITheme.ErrorColor;
        }

        // ── PDF Export ──
        private void ExportPdf()
        {
            try
            {
                var doc = new PrintDocument();
                doc.PrintPage += (s, e) =>
                {
                    var g = e.Graphics;
                    var y = 40f;
                    using (var titleFont = new Font("Segoe UI", 16, FontStyle.Bold))
                    using (var headerFont = new Font("Segoe UI", 11, FontStyle.Bold))
                    using (var bodyFont = new Font("Segoe UI", 9))
                    using (var blackBrush = new SolidBrush(Color.Black))
                    using (var grayBrush = new SolidBrush(Color.Gray))
                    {
                        g.DrawString("BÁO CÁO DOANH THU", titleFont, blackBrush, 40, y); y += 36;
                        g.DrawString($"Từ {_fromPicker.Value:dd/MM/yyyy} đến {_toPicker.Value:dd/MM/yyyy}", bodyFont, grayBrush, 40, y); y += 24;
                        g.DrawLine(Pens.Gray, 40, y, 560, y); y += 16;

                        if (_currentKpi != null)
                        {
                            g.DrawString($"Tổng doanh thu: {_currentKpi.TotalRevenue:N0} ₫", headerFont, blackBrush, 40, y); y += 22;
                            g.DrawString($"Tổng hóa đơn: {_currentKpi.OrderCount}", headerFont, blackBrush, 40, y); y += 22;
                            g.DrawString($"Giá trị đơn TB: {_currentKpi.AvgOrderValue:N0} ₫", headerFont, blackBrush, 40, y); y += 30;
                        }

                        // Revenue table
                        g.DrawString("Doanh thu theo ngày:", headerFont, blackBrush, 40, y); y += 22;
                        if (_revenueGrid.DataSource is DataTable dt)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                if (y > 720) break;
                                g.DrawString($"  {row["ReportDate"]}    {Convert.ToDecimal(row["Revenue"]):N0} ₫", bodyFont, blackBrush, 40, y);
                                y += 16;
                            }
                        }
                    }
                    e.HasMorePages = false;
                };

                using (var preview = new PrintPreviewDialog { Document = doc, Width = 800, Height = 600 })
                {
                    preview.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("PDF export failed", ex);
                UiDialogs.ShowError("Không thể xuất PDF.");
            }
        }

        private void RevenueGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var colName = _revenueGrid.Columns[e.ColumnIndex].Name;
                if (colName == "ReportDate" && e.Value != null)
                {
                    if (DateTime.TryParse(e.Value.ToString(), out DateTime date))
                    {
                        e.Value = date.ToString("dd/MM/yyyy");
                        e.FormattingApplied = true;
                    }
                }
            }
        }

        /// <summary>Helper class for filter dropdown items with Id.</summary>
        private class ComboItem
        {
            public int Id { get; }
            public string Name { get; }
            public ComboItem(int id, string name) { Id = id; Name = name; }
            public override string ToString() { return Name; }
        }
    }
}
