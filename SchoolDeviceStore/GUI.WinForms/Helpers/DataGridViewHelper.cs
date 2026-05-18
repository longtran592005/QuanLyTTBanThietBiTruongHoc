using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace GUI.WinForms
{
    public static class DataGridViewHelper
    {
        private static readonly CultureInfo VietnamCulture = new CultureInfo("vi-VN");

        public static void ApplyProfessionalStyle(DataGridView grid)
        {
            UIHelper.StyleDataGridView(grid);
            grid.DefaultCellStyle.Padding = new Padding(10, 4, 10, 4);
            grid.RowTemplate.Height = 36;
            grid.ColumnHeadersHeight = 44;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
        }

        public static DataGridViewTextBoxColumn CreateTextColumn(string propertyName, string headerText, int width, bool readOnly = false, string format = null)
        {
            var column = new DataGridViewTextBoxColumn
            {
                DataPropertyName = propertyName,
                Name = propertyName,
                HeaderText = headerText,
                ReadOnly = readOnly,
                Width = width,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            if (!string.IsNullOrWhiteSpace(format))
            {
                column.DefaultCellStyle.Format = format;
            }

            return column;
        }

        public static DataGridViewTextBoxColumn CreateCurrencyColumn(string propertyName, string headerText, int width, bool readOnly = true)
        {
            var column = new DataGridViewTextBoxColumn
            {
                DataPropertyName = propertyName,
                Name = propertyName,
                HeaderText = headerText,
                ReadOnly = readOnly,
                Width = width,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            column.DefaultCellStyle.Format = "N0";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.DefaultCellStyle.ForeColor = UITheme.TextPrimaryColor;

            return column;
        }

        public static DataGridViewTextBoxColumn CreateDateColumn(string propertyName, string headerText, int width, bool readOnly = true)
        {
            var column = new DataGridViewTextBoxColumn
            {
                DataPropertyName = propertyName,
                Name = propertyName,
                HeaderText = headerText,
                ReadOnly = readOnly,
                Width = width,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            column.DefaultCellStyle.Format = "dd/MM/yyyy";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            return column;
        }

        public static DataGridViewTextBoxColumn CreateStatusColumn(string propertyName, string headerText, int width)
        {
            var column = new DataGridViewTextBoxColumn
            {
                DataPropertyName = propertyName,
                Name = propertyName,
                HeaderText = headerText,
                ReadOnly = true,
                Width = width,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            return column;
        }

        public static DataGridViewButtonColumn CreateActionColumn(string headerText, int width, string buttonText = "Action")
        {
            var column = new DataGridViewButtonColumn
            {
                Name = "Action",
                HeaderText = headerText,
                Text = buttonText,
                UseColumnTextForButtonValue = true,
                Width = width,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            return column;
        }

        public static void ApplyStatusColors(DataGridView grid, string columnName, Dictionary<string, Color> statusColorMap)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells[columnName].Value == null)
                    continue;

                var status = row.Cells[columnName].Value.ToString();
                if (statusColorMap.ContainsKey(status))
                {
                    row.DefaultCellStyle.BackColor = statusColorMap[status];
                }
            }
        }

        public static void ShowEmptyState(DataGridView grid, EmptyStateControl emptyState, int itemCount)
        {
            var hasItems = itemCount > 0;
            grid.Visible = hasItems;
            emptyState.Visible = !hasItems;

            if (hasItems)
            {
                grid.BringToFront();
            }
            else
            {
                emptyState.BringToFront();
            }
        }

        public static void ShowLoadingOverlay(LoadingOverlayControl overlay, bool show, string message = "Loading...")
        {
            if (show)
            {
                overlay.SetMessage(message);
                overlay.ShowOverlay();
                overlay.BringToFront();
            }
            else
            {
                overlay.HideOverlay();
            }
        }

        public static string FormatCurrencyVN(decimal value)
        {
            return value.ToString("N0", VietnamCulture) + " ₫";
        }

        public static string FormatCurrencyVNShort(decimal value)
        {
            return value.ToString("N0", VietnamCulture);
        }

        public static string FormatDateVN(DateTime date)
        {
            return date.ToString("dd/MM/yyyy", VietnamCulture);
        }

        public static string FormatDateTimeVN(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm", VietnamCulture);
        }
    }
}
