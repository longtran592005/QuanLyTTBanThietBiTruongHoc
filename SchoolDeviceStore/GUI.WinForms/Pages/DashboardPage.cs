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
                RowCount = 4,
                BackColor = UITheme.BackgroundColor,
                Padding = new Padding(0)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 148F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            root.Controls.Add(BuildHeader(), 0, 0);
            root.Controls.Add(BuildKpiRow(), 0, 1);
            root.Controls.Add(BuildMiddleRow(), 0, 2);
            root.Controls.Add(BuildBottomRow(), 0, 3);

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

        private Control BuildMiddleRow()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UITheme.BackgroundColor,
                Padding = new Padding(0, 12, 0, 0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));

            var chartCard = BuildChartCard("Xu hướng doanh thu", _revenueChart);
            var rightStack = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(12, 0, 0, 0)
            };
            rightStack.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var lowStockCard = BuildGridCard("Sản phẩm sắp hết hàng", _lowStockGrid);

            rightStack.Controls.Add(lowStockCard, 0, 0);

            layout.Controls.Add(chartCard, 0, 0);
            layout.Controls.Add(rightStack, 1, 0);
            return layout;
        }

        private Control BuildBottomRow()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UITheme.BackgroundColor,
                Padding = new Padding(0, 12, 0, 0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            var invoiceCard = BuildGridCard("Hóa đơn gần đây", _recentInvoicesGrid);
            var noticeCard = BuildNoticeCard();

            layout.Controls.Add(invoiceCard, 0, 0);
            layout.Controls.Add(noticeCard, 1, 0);
            return layout;
        }

        private Control BuildChartCard(string title, Chart chart)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(card);
            var titleLabel = new Label { Dock = DockStyle.Top, Height = 28, Text = title, Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            chart.Dock = DockStyle.Fill;
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
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(card);
            var titleLabel = new Label { Dock = DockStyle.Top, Height = 28, Text = title, Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            UIHelper.StyleDataGridView(grid);
            grid.Dock = DockStyle.Fill;
            card.Controls.Add(grid);
            card.Controls.Add(titleLabel);
            return card;
        }

        private Control BuildQuickActionCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(card);
            var titleLabel = new Label { Dock = DockStyle.Top, Height = 28, Text = "Thao tác nhanh", Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 8, 0, 0)
            };

            flow.Controls.Add(CreateActionButton("Bán hàng mới", UITheme.PrimaryColor, () => _navigate?.Invoke("sales")));
            flow.Controls.Add(CreateActionButton("Sản phẩm", UITheme.SuccessColor, () => _navigate?.Invoke("products")));
            flow.Controls.Add(CreateActionButton("Báo cáo", UITheme.WarningColor, () => _navigate?.Invoke("reports")));
            flow.Controls.Add(CreateActionButton("Sao lưu", UITheme.TextSecondaryColor, () => _navigate?.Invoke("backup")));

            card.Controls.Add(flow);
            card.Controls.Add(titleLabel);
            return card;
        }

        private Control BuildNoticeCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(card);
            var titleLabel = new Label { Dock = DockStyle.Top, Height = 28, Text = "Ghi chú vận hành", Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            var noticeLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = UITheme.BaseFont,
                ForeColor = UITheme.TextSecondaryColor,
                Text = "Các tiện ích tổng quan nên lấy dữ liệu trực tiếp, sau đó mở rộng bằng cảnh báo, xu hướng và tùy chọn xuất file."
            };
            card.Controls.Add(noticeLabel);
            card.Controls.Add(titleLabel);
            return card;
        }

        private Button CreateActionButton(string text, Color color, Action action)
        {
            var button = UIHelper.CreatePrimaryButton(text);
            button.BackColor = color;
            button.FlatAppearance.BorderColor = color;
            button.Width = 180;
            button.Margin = new Padding(0, 0, 0, 8);
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Click += (s, e) => action();
            return button;
        }

        private void RefreshDashboard()
        {
            var revenue = DbHelper.ExecuteScalar("SELECT IFNULL(SUM(TotalAmount),0) FROM SalesOrders WHERE date(OrderDate) >= date('now','-30 day')");
            var salesCount = DbHelper.ExecuteScalar("SELECT COUNT(1) FROM SalesOrders WHERE date(OrderDate) >= date('now','-30 day')");
            var productsCount = DbHelper.ExecuteScalar("SELECT COUNT(1) FROM Products");
            var lowStockCount = DbHelper.ExecuteScalar("SELECT COUNT(1) FROM Products WHERE Quantity <= 5");

            _revenueCard.SetData("Doanh thu (30 ngày)", Convert.ToDecimal(revenue).ToString("N0"), "Tổng hợp vận hành trực tiếp", UITheme.PrimaryColor);
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

            _lowStockGrid.DataSource = DbHelper.ExecuteQuery("SELECT ProductCode, ProductName, Quantity, UnitPrice FROM Products WHERE Quantity <= 5 ORDER BY Quantity ASC, ProductName ASC LIMIT 10");
            if (_lowStockGrid.Columns.Count >= 4)
            {
                _lowStockGrid.Columns["ProductCode"].HeaderText = "Mã hàng";
                _lowStockGrid.Columns["ProductName"].HeaderText = "Tên sản phẩm";
                _lowStockGrid.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                
                _lowStockGrid.Columns["Quantity"].HeaderText = "Số lượng";
                _lowStockGrid.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                
                _lowStockGrid.Columns["UnitPrice"].HeaderText = "Đơn giá";
                _lowStockGrid.Columns["UnitPrice"].DefaultCellStyle.Format = "#,##0 đ";
                _lowStockGrid.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            _recentInvoicesGrid.DataSource = DbHelper.ExecuteQuery("SELECT SalesOrderCode, datetime(OrderDate) AS OrderDate, TotalAmount, Discount, VAT FROM SalesOrders ORDER BY datetime(OrderDate) DESC LIMIT 10");
            if (_recentInvoicesGrid.Columns.Count >= 5)
            {
                _recentInvoicesGrid.Columns["SalesOrderCode"].HeaderText = "Mã hóa đơn";
                _recentInvoicesGrid.Columns["OrderDate"].HeaderText = "Ngày lập";
                _recentInvoicesGrid.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                
                _recentInvoicesGrid.Columns["TotalAmount"].HeaderText = "Tổng tiền";
                _recentInvoicesGrid.Columns["TotalAmount"].DefaultCellStyle.Format = "#,##0 đ";
                _recentInvoicesGrid.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                
                _recentInvoicesGrid.Columns["Discount"].HeaderText = "Giảm giá";
                _recentInvoicesGrid.Columns["Discount"].DefaultCellStyle.Format = "#,##0 đ";
                _recentInvoicesGrid.Columns["Discount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                
                _recentInvoicesGrid.Columns["VAT"].HeaderText = "VAT (%)";
                _recentInvoicesGrid.Columns["VAT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
    }
}
