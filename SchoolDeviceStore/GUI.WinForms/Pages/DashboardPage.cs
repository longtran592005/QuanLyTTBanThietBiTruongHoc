using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BLL;
using DAL;
using DTO;

namespace GUI.WinForms
{
    public class DashboardPage : UserControl
    {
        private readonly Employee _currentUser;
        private readonly Action<string> _navigate;
        private readonly ReportService _reportService = new ReportService();
        private readonly Label _headerLabel = new Label();
        private readonly Chart _revenueChart = new Chart();
        private readonly DataGridView _recentInvoicesGrid = new DataGridView();
        private readonly DataGridView _lowStockGrid = new DataGridView();
        private readonly KpiCardControl _revenueCard = new KpiCardControl();
        private readonly KpiCardControl _salesCard = new KpiCardControl();
        private readonly KpiCardControl _productsCard = new KpiCardControl();
        private readonly KpiCardControl _lowStockCard = new KpiCardControl();

        public DashboardPage(Employee currentUser, Action<string> navigate)
        {
            _currentUser = currentUser;
            _navigate = navigate;
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UITheme.BackgroundColor,
                Padding = new Padding(0)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 148F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            root.Controls.Add(BuildHeader(), 0, 0);
            root.Controls.Add(BuildKpiRow(), 0, 1);
            root.Controls.Add(BuildContentArea(), 0, 2);

            Controls.Add(root);
            Load += (s, e) => RefreshDashboard();
        }

        private Control BuildHeader()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            _headerLabel.Dock = DockStyle.Fill;
            _headerLabel.Font = UITheme.TitleFont;
            _headerLabel.ForeColor = UITheme.TextPrimaryColor;
            _headerLabel.TextAlign = ContentAlignment.MiddleLeft;
            _headerLabel.Text = "Tổng quan";

            var sub = new Label
            {
                Dock = DockStyle.Right,
                Width = 320,
                Font = UITheme.CaptionFont,
                ForeColor = UITheme.TextSecondaryColor,
                TextAlign = ContentAlignment.MiddleRight,
                Text = _currentUser == null ? string.Empty : $"Chào mừng, {_currentUser.FullName}"
            };

            panel.Controls.Add(sub);
            panel.Controls.Add(_headerLabel);
            return panel;
        }

        private Control BuildKpiRow()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = UITheme.BackgroundColor,
                Margin = new Padding(0),
                Padding = new Padding(0, 4, 0, 4)
            };
            for (var i = 0; i < 4; i++)
            {
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }

            layout.Controls.Add(_revenueCard, 0, 0);
            layout.Controls.Add(_salesCard, 1, 0);
            layout.Controls.Add(_productsCard, 2, 0);
            layout.Controls.Add(_lowStockCard, 3, 0);
            return layout;
        }

        private Control BuildContentArea()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = UITheme.BackgroundColor,
                Padding = new Padding(0, 12, 0, 0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));

            var chartCard = BuildChartCard("Xu hướng doanh thu", _revenueChart);
            var lowStockCard = BuildGridCard("Sản phẩm sắp hết hàng", _lowStockGrid);
            var invoiceCard = BuildGridCard("Hóa đơn gần đây", _recentInvoicesGrid);

            layout.Controls.Add(chartCard, 0, 0);
            layout.Controls.Add(lowStockCard, 1, 0);
            layout.SetRowSpan(lowStockCard, 2);
            layout.Controls.Add(invoiceCard, 0, 1);
            return layout;
        }

        private Control BuildChartCard(string title, Chart chart)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16), MinimumSize = new Size(100, 260) };
            UIHelper.StyleCard(card);
            var titleLabel = new Label { Dock = DockStyle.Top, Height = 28, Text = title, Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            chart.Dock = DockStyle.Fill;
            chart.MinimumSize = new Size(100, 220);
            chart.BackColor = Color.White;
            chart.ChartAreas.Clear();
            chart.Series.Clear();
            chart.ChartAreas.Add(new ChartArea("Main"));
            chart.Series.Add(new Series("Revenue") { 
                ChartType = SeriesChartType.Column, 
                ChartArea = "Main", 
                Color = UITheme.PrimaryColor,
                CustomProperties = "DrawingStyle=Cylinder" // Simulates slightly rounded look in WinForms
            });
            chart.Legends.Clear();
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = UITheme.BorderColor;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = UITheme.BorderColor;
            
            // Abbreviate Y Axis to 'Tr' (Millions)
            chart.FormatNumber += (s, e) =>
            {
                if (e.ElementType == ChartElementType.AxisLabels && e.Value >= 1000000)
                {
                    e.LocalizedValue = (e.Value / 1000000D).ToString("0.#") + " Tr";
                }
            };
            card.Controls.Add(chart);
            card.Controls.Add(titleLabel);
            return card;
        }

        private Control BuildGridCard(string title, DataGridView grid)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16), MinimumSize = new Size(100, 180) };
            UIHelper.StyleCard(card);
            var titleLabel = new Label { Dock = DockStyle.Top, Height = 28, Text = title, Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            UIHelper.StyleDataGridView(grid);
            grid.Dock = DockStyle.Fill;
            card.Controls.Add(grid);
            card.Controls.Add(titleLabel);
            return card;
        }

        private void RefreshDashboard()
        {
            var revenue = DbHelper.ExecuteScalar("SELECT IFNULL(SUM(TotalAmount),0) FROM SalesOrders WHERE date(OrderDate) >= date('now','-30 day')");
            var salesCount = DbHelper.ExecuteScalar("SELECT COUNT(1) FROM SalesOrders WHERE date(OrderDate) >= date('now','-30 day')");
            var productsCount = DbHelper.ExecuteScalar("SELECT COUNT(1) FROM Products");
            var lowStockCount = DbHelper.ExecuteScalar("SELECT COUNT(1) FROM Products WHERE Quantity <= 5");

            _revenueCard.SetData("Doanh thu (30 ngày)", Convert.ToDecimal(revenue).ToString("N0") + " đ", "Tổng hợp vận hành trực tiếp", UITheme.PrimaryColor);
            _salesCard.SetData("Tổng số hóa đơn", Convert.ToInt32(salesCount).ToString(), "Hóa đơn trong 30 ngày gần nhất", UITheme.SuccessColor);
            _productsCard.SetData("Sản phẩm", Convert.ToInt32(productsCount).ToString(), "Số lượng trong danh mục", UITheme.WarningColor);
            _lowStockCard.SetData("Sắp hết hàng", Convert.ToInt32(lowStockCount).ToString(), "Cần nhập bổ sung", UITheme.ErrorColor);

            var revenueTable = _reportService.GetRevenueByDay(DateTime.Today.AddDays(-14), DateTime.Today);
            var revenueSeries = _revenueChart.Series[0];
            revenueSeries.Points.Clear();
            foreach (DataRow row in revenueTable.Rows)
            {
                revenueSeries.Points.AddXY(Convert.ToDateTime(row["ReportDate"]).ToString("dd/MM"), Convert.ToDecimal(row["Revenue"]));
            }

            _lowStockGrid.DataSource = DbHelper.ExecuteQuery("SELECT ProductName, Quantity FROM Products WHERE Quantity <= 5 ORDER BY Quantity ASC, ProductName ASC LIMIT 10");
            if (_lowStockGrid.Columns.Count >= 2)
            {
                _lowStockGrid.Columns["ProductName"].HeaderText = "Sản phẩm";
                _lowStockGrid.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                
                _lowStockGrid.Columns["Quantity"].HeaderText = "Tồn";
                _lowStockGrid.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            _recentInvoicesGrid.DataSource = DbHelper.ExecuteQuery("SELECT SalesOrderCode, datetime(OrderDate) AS OrderDate, TotalAmount FROM SalesOrders ORDER BY datetime(OrderDate) DESC LIMIT 10");
            if (_recentInvoicesGrid.Columns.Count >= 3)
            {
                _recentInvoicesGrid.Columns["SalesOrderCode"].HeaderText = "Mã hóa đơn";
                _recentInvoicesGrid.Columns["OrderDate"].HeaderText = "Ngày lập";
                _recentInvoicesGrid.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                
                _recentInvoicesGrid.Columns["TotalAmount"].HeaderText = "Tổng tiền";
                _recentInvoicesGrid.Columns["TotalAmount"].DefaultCellStyle.Format = "#,##0 đ";
                _recentInvoicesGrid.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
    }
}
