using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public static class ExportHelper
    {
        public static void ExportDataGridViewToCsv(DataGridView grid, string defaultFileName)
        {
            if (grid == null || grid.Rows.Count == 0)
            {
                UiDialogs.ShowWarning("No data to export.");
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv";
                dialog.FileName = defaultFileName;
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                var builder = new StringBuilder();
                var headers = grid.Columns.Cast<DataGridViewColumn>().Select(column => EscapeCsv(column.HeaderText));
                builder.AppendLine(string.Join(",", headers));

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    var values = row.Cells.Cast<DataGridViewCell>().Select(cell => EscapeCsv(Convert.ToString(cell.Value)));
                    builder.AppendLine(string.Join(",", values));
                }

                File.WriteAllText(dialog.FileName, builder.ToString(), Encoding.UTF8);
                UiDialogs.ShowInfo("CSV export completed.");
            }
        }

        public static void ExportDataGridViewToExcelFriendlyXls(DataGridView grid, string defaultFileName)
        {
            if (grid == null || grid.Rows.Count == 0)
            {
                UiDialogs.ShowWarning("No data to export.");
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel files (*.xls)|*.xls";
                dialog.FileName = defaultFileName;
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                var builder = new StringBuilder();
                builder.AppendLine("<html><head><meta charset=\"utf-8\"></head><body><table border=\"1\">");
                builder.Append("<tr>");
                foreach (DataGridViewColumn column in grid.Columns)
                {
                    builder.Append("<th>").Append(System.Security.SecurityElement.Escape(column.HeaderText)).AppendLine("</th>");
                }
                builder.AppendLine("</tr>");

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    builder.Append("<tr>");
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        var value = Convert.ToString(cell.Value) ?? string.Empty;
                        builder.Append("<td>").Append(System.Security.SecurityElement.Escape(value)).AppendLine("</td>");
                    }
                    builder.AppendLine("</tr>");
                }

                builder.AppendLine("</table></body></html>");
                File.WriteAllText(dialog.FileName, builder.ToString(), Encoding.UTF8);
                UiDialogs.ShowInfo("Excel export completed.");
            }
        }

        private static string EscapeCsv(string value)
        {
            value = value ?? string.Empty;
            if (value.Contains(",") || value.Contains('"') || value.Contains("\n"))
            {
                return '"' + value.Replace("\"", "\"\"") + '"';
            }

            return value;
        }
    }
}
