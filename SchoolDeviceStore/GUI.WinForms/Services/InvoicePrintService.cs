using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using DTO;

namespace GUI.WinForms
{
    public class InvoicePrintService
    {
        private readonly string _storeName;
        private readonly string _storeSubtitle;
        private readonly string _storePhone;
        private readonly string _storeAddress;

        private InvoiceSnapshot _snapshot;
        private int _printLineIndex;
        private float _currentY;
        private readonly Font _titleFont = new Font("Segoe UI", 14F, FontStyle.Bold);
        private readonly Font _bodyFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        private readonly Font _boldFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        private readonly Font _smallFont = new Font("Segoe UI", 8.5F, FontStyle.Regular);

        public InvoicePrintService(
            string storeName = "School Device Store",
            string storeSubtitle = "Business Management System",
            string storePhone = "(000) 000-0000",
            string storeAddress = "Địa chỉ cửa hàng")
        {
            _storeName = storeName;
            _storeSubtitle = storeSubtitle;
            _storePhone = storePhone;
            _storeAddress = storeAddress;
        }

        public void SetInvoice(InvoiceSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public bool HasInvoice => _snapshot != null;

        public void ShowPrintPreview(IWin32Window owner)
        {
            if (_snapshot == null)
            {
                UiDialogs.ShowWarning("Chưa có hóa đơn sẵn sàng để in.");
                return;
            }

            using (var preview = new PrintPreviewDialog())
            using (var document = CreateDocument())
            {
                preview.Document = document;
                preview.Width = 1200;
                preview.Height = 900;
                preview.ShowDialog(owner);
            }
        }

        public void PrintNow(IWin32Window owner)
        {
            if (_snapshot == null)
            {
                UiDialogs.ShowWarning("Chưa có hóa đơn sẵn sàng để in.");
                return;
            }

            using (var document = CreateDocument())
            using (var dialog = new PrintDialog())
            {
                dialog.Document = document;
                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    document.Print();
                }
            }
        }

        private PrintDocument CreateDocument()
        {
            var document = new PrintDocument();
            document.PrintPage += PrintPage;
            document.BeginPrint += (s, e) =>
            {
                _printLineIndex = 0;
                _currentY = 0;
            };
            return document;
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            var graphics = e.Graphics;
            var marginLeft = e.MarginBounds.Left;
            var width = e.MarginBounds.Width;
            float cursorY = e.MarginBounds.Top;
            var lineHeight = 18F;
            var pageBottom = e.MarginBounds.Bottom;

            void DrawText(string text, Font font, Brush brush, float x, ref float y, float height = 18F)
            {
                graphics.DrawString(text ?? string.Empty, font, brush, x, y);
                y += height;
            }

            if (_printLineIndex == 0)
            {
                DrawText(_storeName, _titleFont, Brushes.Black, marginLeft, ref cursorY, 24F);
                DrawText(_storeSubtitle, _bodyFont, Brushes.Black, marginLeft, ref cursorY, 20F);
                DrawText(_storeAddress, _smallFont, Brushes.Black, marginLeft, ref cursorY, 16F);
                DrawText($"Phone: {_storePhone}", _smallFont, Brushes.Black, marginLeft, ref cursorY, 16F);
                cursorY += 8;
                graphics.DrawLine(Pens.Black, marginLeft, cursorY, marginLeft + width, cursorY);
                cursorY += 12;
                DrawText($"Invoice No: {_snapshot.InvoiceCode}", _boldFont, Brushes.Black, marginLeft, ref cursorY, 18F);
                DrawText($"Date: {_snapshot.InvoiceDate:dd/MM/yyyy HH:mm}", _bodyFont, Brushes.Black, marginLeft, ref cursorY, 18F);
                DrawText($"Cashier: {_snapshot.CreatedBy}", _bodyFont, Brushes.Black, marginLeft, ref cursorY, 18F);
                if (!string.IsNullOrWhiteSpace(_snapshot.CustomerName))
                {
                    DrawText($"Customer: {_snapshot.CustomerName}", _bodyFont, Brushes.Black, marginLeft, ref cursorY, 18F);
                }
                cursorY += 8;
                graphics.DrawLine(Pens.Black, marginLeft, cursorY, marginLeft + width, cursorY);
                cursorY += 12;
                DrawText("Item", _boldFont, Brushes.Black, marginLeft, ref cursorY, 18F);
                graphics.DrawString("Qty", _boldFont, Brushes.Black, marginLeft + width - 190, cursorY - 18F);
                graphics.DrawString("Price", _boldFont, Brushes.Black, marginLeft + width - 135, cursorY - 18F);
                graphics.DrawString("Total", _boldFont, Brushes.Black, marginLeft + width - 70, cursorY - 18F);
                cursorY += 6;
                graphics.DrawLine(Pens.Black, marginLeft, cursorY, marginLeft + width, cursorY);
                cursorY += 8;
            }

            for (; _printLineIndex < _snapshot.Items.Count; _printLineIndex++)
            {
                var item = _snapshot.Items[_printLineIndex];
                if (cursorY > pageBottom - 120)
                {
                    e.HasMorePages = true;
                    _currentY = cursorY;
                    return;
                }

                graphics.DrawString(TrimToWidth(item.ProductName, _bodyFont, width - 200, graphics), _bodyFont, Brushes.Black, marginLeft, cursorY);
                graphics.DrawString(item.Quantity.ToString(), _bodyFont, Brushes.Black, marginLeft + width - 190, cursorY);
                graphics.DrawString(item.UnitPrice.ToString("N0"), _bodyFont, Brushes.Black, marginLeft + width - 135, cursorY);
                graphics.DrawString(item.LineTotal.ToString("N0"), _bodyFont, Brushes.Black, marginLeft + width - 70, cursorY);
                cursorY += lineHeight;
            }

            cursorY += 8;
            graphics.DrawLine(Pens.Black, marginLeft, cursorY, marginLeft + width, cursorY);
            cursorY += 10;
            DrawText($"Subtotal: {_snapshot.SubTotal:N0}", _bodyFont, Brushes.Black, marginLeft + width - 190, ref cursorY, 18F);
            DrawText($"Discount: {_snapshot.Discount:N0}", _bodyFont, Brushes.Black, marginLeft + width - 190, ref cursorY, 18F);
            DrawText($"VAT: {_snapshot.VatAmount:N0}", _bodyFont, Brushes.Black, marginLeft + width - 190, ref cursorY, 18F);
            DrawText($"Grand Total: {_snapshot.TotalAmount:N0}", _boldFont, Brushes.Black, marginLeft + width - 190, ref cursorY, 20F);
            DrawText($"Payment: {_snapshot.PaymentStatus}", _bodyFont, Brushes.Black, marginLeft, ref cursorY, 18F);

            e.HasMorePages = false;
        }

        private static string TrimToWidth(string text, Font font, int width, Graphics graphics)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var trimmed = text;
            while (trimmed.Length > 0 && graphics.MeasureString(trimmed, font).Width > width)
            {
                trimmed = trimmed.Substring(0, trimmed.Length - 1);
            }

            return trimmed == text ? text : trimmed + "...";
        }
    }

    public class InvoiceSnapshot
    {
        public string InvoiceCode { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string CreatedBy { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public List<InvoiceSnapshotItem> Items { get; set; } = new List<InvoiceSnapshotItem>();
    }

    public class InvoiceSnapshotItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
