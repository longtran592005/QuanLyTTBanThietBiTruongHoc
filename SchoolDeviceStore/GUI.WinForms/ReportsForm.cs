using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BLL;

namespace GUI.WinForms
{
    public class ReportsForm : Form
    {
        private readonly ReportService _service = new ReportService();
        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private DataGridView dgvRevenue;
        private DataGridView dgvTopProducts;
        private Chart chartRevenue;

        public ReportsForm()
        {
            InitializeComponents();
            LoadReports();
        }

        private void InitializeComponents()
        {
            Text = "Báo cáo thống kê";
            Width = 1100;
            Height = 700;
            StartPosition = FormStartPosition.CenterParent;
            FontHelper.ApplyVietnameseFontToForm(this);

            Controls.Add(new Label { Left = 20, Top = 20, Text = "Từ ngày", Width = 80 });
            dtFrom = new DateTimePicker { Left = 100, Top = 16, Width = 120 };
            Controls.Add(dtFrom);
            Controls.Add(new Label { Left = 240, Top = 20, Text = "Đến ngày", Width = 80 });
            dtTo = new DateTimePicker { Left = 320, Top = 16, Width = 120 };
            Controls.Add(dtTo);

            var btnLoad = new Button { Left = 460, Top = 14, Width = 130, Text = "Tải báo cáo" };
            btnLoad.Click += (s, e) => LoadReports();
            Controls.Add(btnLoad);

            chartRevenue = new Chart { Left = 20, Top = 60, Width = 1040, Height = 250 };
            chartRevenue.ChartAreas.Add(new ChartArea("Main"));
            chartRevenue.Series.Add(new Series("Revenue") { ChartType = SeriesChartType.Column, ChartArea = "Main" });
            Controls.Add(chartRevenue);

            dgvRevenue = new DataGridView { Left = 20, Top = 350, Width = 500, Height = 300, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvTopProducts = new DataGridView { Left = 540, Top = 350, Width = 520, Height = 300, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            Controls.Add(dgvRevenue);
            Controls.Add(dgvTopProducts);

            dtTo.Value = DateTime.Today;
            dtFrom.Value = DateTime.Today.AddMonths(-1);
        }

        private void LoadReports()
        {
            var revenueTable = _service.GetRevenueByDay(dtFrom.Value, dtTo.Value);
            dgvRevenue.DataSource = revenueTable;

            chartRevenue.Series[0].Points.Clear();
            foreach (DataRow row in revenueTable.Rows)
            {
                chartRevenue.Series[0].Points.AddXY(Convert.ToDateTime(row["ReportDate"]).ToString("dd/MM"), Convert.ToDecimal(row["Revenue"]));
            }

            dgvTopProducts.DataSource = _service.GetTopProducts(dtFrom.Value, dtTo.Value);
        }
    }
}