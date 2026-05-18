using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BLL;

namespace GUI.WinForms
{
    public class ReportsPage : UserControl
    {
        private readonly ReportService _service = new ReportService();
        private readonly DateTimePicker _fromPicker = new DateTimePicker();
        private readonly DateTimePicker _toPicker = new DateTimePicker();
        private readonly DataGridView _revenueGrid = new DataGridView();
        private readonly DataGridView _topProductsGrid = new DataGridView();
        private readonly Chart _chart = new Chart();
        private readonly Label _totalRevenueLabel = new Label();
        private readonly Label _salesCountLabel = new Label();
        private readonly Label _avgOrderValueLabel = new Label();
        private readonly EmptyStateControl _emptyState = new EmptyStateControl();

        public ReportsPage()
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;
            BuildLayout();
            Load += (s, e) => LoadReports();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, BackColor = UITheme.BackgroundColor };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));   // Filter bar
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));  // KPI cards
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 280F));  // Chart (fixed to avoid 0-height crash)
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // Grid row
            root.Controls.Add(BuildFilterBar(), 0, 0);
            root.Controls.Add(BuildKpiCards(), 0, 1);
            root.Controls.Add(BuildChartRow(), 0, 2);
            root.Controls.Add(BuildGridRow(), 0, 3);
            Controls.Add(root);
        }

        private Control BuildFilterBar()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16, 12, 16, 12) };
            UIHelper.StyleCard(panel);

            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            layout.Controls.Add(MakeLabeledPicker("Từ ngày", _fromPicker));
            layout.Controls.Add(MakeLabeledPicker("Đến ngày", _toPicker));

            var loadButton = UIHelper.CreatePrimaryButton("Tải báo cáo");
            loadButton.Click += (s, e) => LoadReports();
            layout.Controls.Add(loadButton);

            var exportRevenueCsvButton = UIHelper.CreateSecondaryButton("Xuất CSV doanh thu");
            exportRevenueCsvButton.Click += (s, e) => ExportHelper.ExportDataGridViewToCsv(_revenueGrid, "revenue-report.csv");
            layout.Controls.Add(exportRevenueCsvButton);

            var exportProductsCsvButton = UIHelper.CreateSecondaryButton("Xuất CSV sản phẩm");
            exportProductsCsvButton.Click += (s, e) => ExportHelper.ExportDataGridViewToCsv(_topProductsGrid, "top-products.csv");
            layout.Controls.Add(exportProductsCsvButton);

            _fromPicker.Value = DateTime.Today.AddDays(-30);
            _toPicker.Value = DateTime.Today;

            panel.Controls.Add(layout);
            return panel;
        }

        private Control MakeLabeledPicker(string label, DateTimePicker picker)
        {
            var wrapper = new Panel { Width = 220, Height = 46, Margin = new Padding(0, 0, 12, 0) };
            var text = new Label { Dock = DockStyle.Top, Height = 18, Text = label, ForeColor = UITheme.TextSecondaryColor, Font = UITheme.CaptionFont };
            picker.Dock = DockStyle.Bottom;
            wrapper.Controls.Add(picker);
            wrapper.Controls.Add(text);
            return wrapper;
        }

        private Control BuildKpiCards()
        {
            var container = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.BackgroundColor, Padding = new Padding(0, 8, 0, 8) };
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));

            var kpiTotalRevenue = BuildKpiCard("Tổng doanh thu", _totalRevenueLabel, UITheme.PrimaryColor);
            var kpiSalesCount = BuildKpiCard("Tổng số hóa đơn", _salesCountLabel, UITheme.SuccessColor);
            var kpiAvgOrder = BuildKpiCard("Giá trị đơn hàng trung bình", _avgOrderValueLabel, UITheme.WarningColor);

            layout.Controls.Add(kpiTotalRevenue, 0, 0);
            layout.Controls.Add(kpiSalesCount, 1, 0);
            layout.Controls.Add(kpiAvgOrder, 2, 0);

            container.Controls.Add(layout);
            return container;
        }

        private Panel BuildKpiCard(string title, Label valueLabel, Color accentColor)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16), Margin = new Padding(0, 0, 12, 0) };
            UIHelper.StyleCard(card);

            var titleLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 22,
                Text = title,
                Font = UITheme.CaptionFont,
                ForeColor = UITheme.TextSecondaryColor
            };

            valueLabel.Dock = DockStyle.Fill;
            valueLabel.Font = UITheme.TitleFont;
            valueLabel.ForeColor = accentColor;
            valueLabel.Text = "0";
            valueLabel.TextAlign = ContentAlignment.MiddleCenter;

            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);
            return card;
        }

        private Control BuildChartRow()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(card);
            var title = new Label { Dock = DockStyle.Top, Height = 28, Text = "Biểu đồ doanh thu", Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            _chart.Dock = DockStyle.Fill;
            _chart.BackColor = Color.White;
            _chart.MinimumSize = new Size(100, 100);
            _chart.ChartAreas.Clear();
            _chart.Series.Clear();
            _chart.ChartAreas.Add(new ChartArea("Main"));
            _chart.Series.Add(new Series("Revenue") { ChartType = SeriesChartType.Column, ChartArea = "Main", Color = UITheme.PrimaryColor });
            _chart.Legends.Clear();
            card.Controls.Add(_chart);
            card.Controls.Add(title);
            return card;
        }

        private Control BuildGridRow()
        {
            var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 560, BackColor = UITheme.BackgroundColor };
            split.Panel1.Padding = new Padding(0, 12, 12, 0);
            split.Panel2.Padding = new Padding(0, 12, 0, 0);
            split.Panel1.Controls.Add(BuildGridCard("Doanh thu theo ngày", _revenueGrid));
            split.Panel2.Controls.Add(BuildGridCard("Sản phẩm bán chạy", _topProductsGrid));
            return split;
        }

        private Control BuildGridCard(string title, DataGridView grid)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(card);
            var header = new Label { Dock = DockStyle.Top, Height = 28, Text = title, Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            UIHelper.StyleDataGridView(grid);
            grid.Dock = DockStyle.Fill;
            card.Controls.Add(grid);
            card.Controls.Add(header);
            return card;
        }

        private void LoadReports()
        {
            try
            {
                var revenueTable = _service.GetRevenueByDay(_fromPicker.Value, _toPicker.Value);
                _revenueGrid.DataSource = revenueTable;

                var series = _chart.Series[0];
                series.Points.Clear();
                decimal totalRevenue = 0;
                int salesCount = 0;

                foreach (DataRow row in revenueTable.Rows)
                {
                    var revenue = Convert.ToDecimal(row["Revenue"]);
                    totalRevenue += revenue;
                    series.Points.AddXY(Convert.ToDateTime(row["ReportDate"]).ToString("dd/MM"), revenue);
                    salesCount++;
                }

                _topProductsGrid.DataSource = _service.GetTopProducts(_fromPicker.Value, _toPicker.Value);

                _totalRevenueLabel.Text = DataGridViewHelper.FormatCurrencyVN(totalRevenue);
                _salesCountLabel.Text = salesCount.ToString();
                _avgOrderValueLabel.Text = salesCount > 0 ? DataGridViewHelper.FormatCurrencyVN(totalRevenue / salesCount) : "0 ₫";
            }
            catch (Exception ex)
            {
                AppLogger.Error("Report load failed", ex);
                UiDialogs.ShowError("Không thể tải dữ liệu báo cáo.");
            }
        }
    }
}
