using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BLL;
using DAL;
using DAL.Repositories;
using DTO;

namespace GUI.WinForms
{
    public class SalesPage : UserControl
    {
        private readonly Employee _currentUser;
        private readonly ProductRepository _productRepository = new ProductRepository();
        private readonly SalesService _salesService = new SalesService();
        private readonly InvoicePrintService _invoicePrintService = new InvoicePrintService();
        private readonly BindingList<CartLine> _cartLines = new BindingList<CartLine>();
        private readonly List<ProductLookupItem> _productLookupItems = new List<ProductLookupItem>();
        private readonly List<CustomerLookupItem> _customerLookupItems = new List<CustomerLookupItem>();
        private readonly ErrorProvider _errorProvider = new ErrorProvider();
        private readonly LoadingOverlayControl _loadingOverlay = new LoadingOverlayControl();
        private readonly EmptyStateControl _emptyState = new EmptyStateControl();

        private readonly ComboBox _productSearchCombo = new ComboBox();
        private readonly ComboBox _customerSearchCombo = new ComboBox();
        private readonly NumericUpDown _quantityUpDown = new NumericUpDown();
        private readonly NumericUpDown _discountUpDown = new NumericUpDown();
        private readonly NumericUpDown _vatUpDown = new NumericUpDown();
        private readonly ComboBox _paymentStatusCombo = new ComboBox();
        private readonly DataGridView _cartGrid = new DataGridView();
        private readonly BindingSource _cartBindingSource = new BindingSource();
        private readonly Label _stockLabel = new Label();
        private readonly Label _invoiceNumberLabel = new Label();
        private readonly Label _statusLabel = new Label();
        private readonly Label _itemCountLabel = new Label();
        private readonly Label _subtotalValueLabel = new Label();
        private readonly Label _discountValueLabel = new Label();
        private readonly Label _vatValueLabel = new Label();
        private readonly Label _totalValueLabel = new Label();
        private readonly Label _paymentStatusValueLabel = new Label();
        private readonly Button _previewButton = new Button();
        private readonly Button _printButton = new Button();
        private readonly Button _createInvoiceButton = new Button();
        private readonly Panel _cartCard = new Panel();
        private readonly Panel _summaryCard = new Panel();
        private readonly Panel _cartHost = new Panel();
        private readonly Panel _summaryActions = new Panel();
        private InvoiceSnapshot _lastInvoiceSnapshot;

        public SalesPage(Employee currentUser)
        {
            _currentUser = currentUser;
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            BuildLayout();
            Load += (s, e) => LoadData();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UITheme.BackgroundColor,
                Padding = new Padding(0)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));

            root.Controls.Add(BuildHeaderCard(), 0, 0);
            root.Controls.Add(BuildBodyCard(), 0, 1);
            root.Controls.Add(BuildFooterCard(), 0, 2);
            Controls.Add(root);

            Controls.Add(_loadingOverlay);
            _loadingOverlay.BringToFront();
        }

        private Control BuildHeaderCard()
        {
            var card = CreateCard();
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));

            layout.Controls.Add(MakeField("Khách hàng", _customerSearchCombo), 0, 0);
            layout.Controls.Add(MakeField("Tìm sản phẩm", _productSearchCombo), 1, 0);
            layout.Controls.Add(MakeField("SL", _quantityUpDown), 2, 0);

            var addButton = UIHelper.CreatePrimaryButton("Thêm vào giỏ");
            addButton.Dock = DockStyle.Bottom;
            addButton.Height = 36;
            addButton.Margin = new Padding(8, 0, 0, 12);
            addButton.Click += (s, e) => AddProductToCart();
            layout.Controls.Add(addButton, 3, 0);

            _stockLabel.Dock = DockStyle.Fill;
            _stockLabel.Font = UITheme.CaptionFont;
            _stockLabel.ForeColor = UITheme.TextSecondaryColor;
            _stockLabel.Text = "Chọn một sản phẩm để xem thông tin tồn kho.";
            layout.Controls.Add(_stockLabel, 0, 1);
            layout.SetColumnSpan(_stockLabel, 2);

            _invoiceNumberLabel.Dock = DockStyle.Fill;
            _invoiceNumberLabel.Font = UITheme.CaptionFont;
            _invoiceNumberLabel.ForeColor = UITheme.TextSecondaryColor;
            _invoiceNumberLabel.TextAlign = ContentAlignment.MiddleRight;
            _invoiceNumberLabel.Text = "Hóa đơn nháp";
            layout.Controls.Add(_invoiceNumberLabel, 3, 1);

            card.Controls.Add(layout);
            return card;
        }

        private Control BuildBodyCard()
        {
            var container = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UITheme.BackgroundColor,
                Padding = new Padding(0, 12, 0, 0)
            };
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 380F));

            _cartCard.Dock = DockStyle.Fill;
            _cartCard.BackColor = UITheme.SurfaceColor;
            _cartCard.Padding = new Padding(16);
            UIHelper.StyleCard(_cartCard);

            var cartTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Giỏ hóa đơn",
                Font = UITheme.SectionTitleFont,
                ForeColor = UITheme.TextPrimaryColor
            };

            _cartHost.Dock = DockStyle.Fill;
            _cartHost.BackColor = UITheme.SurfaceColor;
            _cartHost.Padding = new Padding(0, 8, 0, 0);
            _emptyState.SetContent("Giỏ hàng trống", "Tìm sản phẩm, thêm vào giỏ và tiếp tục quy trình lập hóa đơn.");
            _emptyState.Dock = DockStyle.Fill;
            _cartGrid.Dock = DockStyle.Fill;
            ConfigureCartGrid();

            _cartHost.Controls.Add(_cartGrid);
            _cartHost.Controls.Add(_emptyState);

            _cartCard.Controls.Add(_cartHost);
            _cartCard.Controls.Add(cartTitle);
            container.Controls.Add(_cartCard, 0, 0);

            _summaryCard.Dock = DockStyle.Fill;
            _summaryCard.BackColor = UITheme.SurfaceColor;
            _summaryCard.Padding = new Padding(16);
            UIHelper.StyleCard(_summaryCard);
            _summaryCard.Controls.Add(BuildSummaryPanel());
            container.Controls.Add(_summaryCard, 1, 0);

            return container;
        }

        private Control BuildSummaryPanel()
        {
            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 10,
                Height = 460,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));  // Title
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));  // Discount input
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));  // VAT input
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));  // Payment status
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));   // Separator
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // Subtotal label
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // Discount label
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // VAT label
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));  // Total
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));  // Payment status value

            var title = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Tóm tắt thanh toán",
                Font = UITheme.SectionTitleFont,
                ForeColor = UITheme.TextPrimaryColor
            };

            var separator = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 1,
                BackColor = UITheme.BorderColor,
                Margin = new Padding(0, 4, 0, 4)
            };

            layout.Controls.Add(title, 0, 0);
            layout.Controls.Add(MakeMoneyField("Giảm giá", _discountUpDown), 0, 1);
            layout.Controls.Add(MakeMoneyField("VAT %", _vatUpDown), 0, 2);
            layout.Controls.Add(MakeComboField("Thanh toán", _paymentStatusCombo), 0, 3);
            layout.Controls.Add(separator, 0, 4);

            _subtotalValueLabel.Font = UITheme.BodyBoldFont;
            _discountValueLabel.Font = UITheme.BodyBoldFont;
            _vatValueLabel.Font = UITheme.BodyBoldFont;
            _totalValueLabel.Font = UITheme.TitleFont;
            _paymentStatusValueLabel.Font = UITheme.BodyBoldFont;
            _subtotalValueLabel.ForeColor = UITheme.TextSecondaryColor;
            _discountValueLabel.ForeColor = UITheme.TextSecondaryColor;
            _vatValueLabel.ForeColor = UITheme.TextSecondaryColor;
            _totalValueLabel.ForeColor = UITheme.PrimaryColor;
            _paymentStatusValueLabel.ForeColor = UITheme.SuccessColor;

            layout.Controls.Add(_subtotalValueLabel, 0, 5);
            layout.Controls.Add(_discountValueLabel, 0, 6);
            layout.Controls.Add(_vatValueLabel, 0, 7);
            layout.Controls.Add(_totalValueLabel, 0, 8);
            layout.Controls.Add(_paymentStatusValueLabel, 0, 9);

            scrollPanel.Controls.Add(layout);
            return scrollPanel;
        }

        private Control BuildFooterCard()
        {
            var card = CreateCard();
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            _itemCountLabel.Dock = DockStyle.Fill;
            _itemCountLabel.Font = UITheme.CaptionFont;
            _itemCountLabel.ForeColor = UITheme.TextSecondaryColor;
            layout.Controls.Add(_itemCountLabel, 0, 0);
            layout.SetColumnSpan(_itemCountLabel, 2);

            _statusLabel.Dock = DockStyle.Fill;
            _statusLabel.Font = UITheme.CaptionFont;
            _statusLabel.ForeColor = UITheme.TextSecondaryColor;
            layout.Controls.Add(_statusLabel, 2, 0);
            layout.SetColumnSpan(_statusLabel, 2);

            // Style buttons with UIHelper for consistent theme
            UIHelper.StyleActionButton(_createInvoiceButton, "Tạo hóa đơn", UITheme.PrimaryColor);
            _createInvoiceButton.Dock = DockStyle.Fill;
            _createInvoiceButton.Click += (s, e) => CreateInvoice();

            UIHelper.StyleActionButton(_previewButton, "Xem trước", UITheme.SurfaceColor);
            _previewButton.ForeColor = UITheme.TextPrimaryColor;
            _previewButton.FlatAppearance.BorderColor = UITheme.BorderColor;
            _previewButton.Dock = DockStyle.Fill;
            _previewButton.Enabled = false;
            _previewButton.Click += (s, e) => ShowPrintPreviewWithConfirmation();

            UIHelper.StyleActionButton(_printButton, "In hóa đơn", UITheme.SurfaceColor);
            _printButton.ForeColor = UITheme.TextPrimaryColor;
            _printButton.FlatAppearance.BorderColor = UITheme.BorderColor;
            _printButton.Dock = DockStyle.Fill;
            _printButton.Enabled = false;
            _printButton.Click += (s, e) => PrintInvoiceWithConfirmation();

            var clearButton = UIHelper.CreateOutlineDangerButton("Xóa giỏ hàng");
            clearButton.Dock = DockStyle.Fill;
            clearButton.Click += (s, e) => ClearCartWithConfirmation();

            layout.Controls.Add(_createInvoiceButton, 0, 1);
            layout.Controls.Add(_printButton, 1, 1);
            layout.Controls.Add(_previewButton, 2, 1);
            layout.Controls.Add(clearButton, 3, 1);

            card.Controls.Add(layout);
            return card;
        }

        private Control MakeField(string label, ComboBox control)
        {
            var wrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 12, 0) };
            var title = new Label { Dock = DockStyle.Top, Height = 16, Text = label, Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor };
            control.Dock = DockStyle.Bottom;
            control.DropDownStyle = ComboBoxStyle.DropDown;
            control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            control.AutoCompleteSource = AutoCompleteSource.ListItems;
            UIHelper.StyleTextBox(control);
            wrapper.Controls.Add(control);
            wrapper.Controls.Add(title);
            return wrapper;
        }

        private Control MakeField(string label, NumericUpDown control)
        {
            var wrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 12, 0) };
            var title = new Label { Dock = DockStyle.Top, Height = 16, Text = label, Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor };
            control.Dock = DockStyle.Bottom;
            control.Maximum = 100000000;
            control.DecimalPlaces = 0;
            control.Minimum = 0;
            UIHelper.StyleTextBox(control);
            wrapper.Controls.Add(control);
            wrapper.Controls.Add(title);
            return wrapper;
        }

        private Control MakeComboField(string label, ComboBox control)
        {
            var wrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 12, 0) };
            var title = new Label { Dock = DockStyle.Top, Height = 16, Text = label, Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor };
            control.Dock = DockStyle.Bottom;
            control.DropDownStyle = ComboBoxStyle.DropDownList;
            UIHelper.StyleTextBox(control);
            wrapper.Controls.Add(control);
            wrapper.Controls.Add(title);
            return wrapper;
        }

        private Panel CreateCard()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.SurfaceColor,
                Padding = new Padding(16)
            };
            UIHelper.StyleCard(panel);
            return panel;
        }

        private void LoadData()
        {
            ShowLoading(true, "Đang tải dữ liệu bán hàng...");
            try
            {
                _quantityUpDown.Value = 1;
                _discountUpDown.Value = 0;
                _vatUpDown.Value = 10;
                LoadProducts();
                LoadCustomers();
                InitializePaymentStatus();
                ConfigureCartSource();

                // Add a default item for demonstration purposes
                if (_productLookupItems.Count > 0)
                {
                    var defaultProduct = _productLookupItems[0];
                    var cartLine = new CartLine(defaultProduct.ProductId, defaultProduct.ProductCode, defaultProduct.ProductName, defaultProduct.StockOnHand, 1, defaultProduct.UnitPrice);
                    cartLine.PropertyChanged += CartLine_PropertyChanged;
                    _cartLines.Add(cartLine);
                    _customerSearchCombo.SelectedIndex = 0; // Select the first customer as well
                }

                UpdateSummary();
                UpdateStockHint();
                _statusLabel.Text = "Sẵn sàng";
            }
            catch (Exception ex)
            {
                AppLogger.Error("Sales page data load failed", ex);
                UiDialogs.ShowError("Không thể tải dữ liệu bán hàng. Vui lòng kiểm tra kết nối cơ sở dữ liệu.");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void LoadProducts()
        {
            var products = _productRepository.GetAll();
            _productLookupItems.Clear();
            foreach (var product in products)
            {
                _productLookupItems.Add(new ProductLookupItem(product));
            }

            _productSearchCombo.DataSource = null;
            _productSearchCombo.DisplayMember = nameof(ProductLookupItem.DisplayText);
            _productSearchCombo.ValueMember = nameof(ProductLookupItem.ProductId);
            _productSearchCombo.DataSource = _productLookupItems;
            _productSearchCombo.SelectedIndex = -1;
            _productSearchCombo.Text = string.Empty;
            _productSearchCombo.SelectedIndexChanged -= ProductSelectionChanged;
            _productSearchCombo.SelectedIndexChanged += ProductSelectionChanged;
            _productSearchCombo.KeyDown -= ProductSearchCombo_KeyDown;
            _productSearchCombo.KeyDown += ProductSearchCombo_KeyDown;
        }

        private void LoadCustomers()
        {
            var dataTable = DbHelper.ExecuteQuery("SELECT CustomerId, CustomerCode, FullName, Phone FROM Customers ORDER BY FullName");
            _customerLookupItems.Clear();
            foreach (DataRow row in dataTable.Rows)
            {
                _customerLookupItems.Add(new CustomerLookupItem
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    CustomerCode = Convert.ToString(row["CustomerCode"]),
                    FullName = Convert.ToString(row["FullName"]),
                    Phone = Convert.ToString(row["Phone"])
                });
            }

            _customerSearchCombo.DataSource = null;
            _customerSearchCombo.DisplayMember = nameof(CustomerLookupItem.DisplayText);
            _customerSearchCombo.ValueMember = nameof(CustomerLookupItem.CustomerId);
            _customerSearchCombo.DataSource = _customerLookupItems;
            _customerSearchCombo.SelectedIndex = -1;
            _customerSearchCombo.Text = string.Empty;
            _customerSearchCombo.KeyDown -= CustomerSearchCombo_KeyDown;
            _customerSearchCombo.KeyDown += CustomerSearchCombo_KeyDown;
        }

        private void InitializePaymentStatus()
        {
            _paymentStatusCombo.DataSource = null;
            _paymentStatusCombo.Items.Clear();
            _paymentStatusCombo.Items.AddRange(new object[] { "Đã thanh toán", "Chờ thanh toán", "Thanh toán một phần" });
            _paymentStatusCombo.SelectedIndex = 0;
            _paymentStatusCombo.SelectedIndexChanged -= PaymentStatusCombo_SelectedIndexChanged;
            _paymentStatusCombo.SelectedIndexChanged += PaymentStatusCombo_SelectedIndexChanged;
        }

        private void ConfigureCartSource()
        {
            _cartBindingSource.DataSource = _cartLines;
            _cartGrid.DataSource = _cartBindingSource;
            _cartGrid.DataError -= CartGrid_DataError;
            _cartGrid.DataError += CartGrid_DataError;
            _cartGrid.CellContentClick -= CartGrid_CellContentClick;
            _cartGrid.CellContentClick += CartGrid_CellContentClick;
            _cartGrid.CellEndEdit -= CartGrid_CellEndEdit;
            _cartGrid.CellEndEdit += CartGrid_CellEndEdit;
            _cartGrid.SelectionChanged -= CartGrid_SelectionChanged;
            _cartGrid.SelectionChanged += CartGrid_SelectionChanged;
            _cartGrid.CurrentCellDirtyStateChanged -= CartGrid_CurrentCellDirtyStateChanged;
            _cartGrid.CurrentCellDirtyStateChanged += CartGrid_CurrentCellDirtyStateChanged;
            _cartLines.ListChanged -= CartLines_ListChanged;
            _cartLines.ListChanged += CartLines_ListChanged;
            _cartGrid.KeyDown -= CartGrid_KeyDown;
            _cartGrid.KeyDown += CartGrid_KeyDown;
            _discountUpDown.ValueChanged -= DiscountOrVatChanged;
            _discountUpDown.ValueChanged += DiscountOrVatChanged;
            _vatUpDown.ValueChanged -= DiscountOrVatChanged;
            _vatUpDown.ValueChanged += DiscountOrVatChanged;
        }

        private void ConfigureCartGrid()
        {
            UIHelper.StyleDataGridView(_cartGrid);
            _cartGrid.AutoGenerateColumns = false;
            _cartGrid.AllowUserToAddRows = false;
            _cartGrid.AllowUserToDeleteRows = false;
            _cartGrid.ReadOnly = false;
            _cartGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _cartGrid.MultiSelect = false;
            _cartGrid.RowHeadersVisible = false;
            _cartGrid.Columns.Clear();

            var removeColumn = new DataGridViewButtonColumn
            {
                Name = "Remove",
                HeaderText = "Thao tác",
                Text = "Xóa",
                UseColumnTextForButtonValue = true,
                Width = 82,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            };

            _cartGrid.Columns.Add(removeColumn);
            _cartGrid.Columns.Add(CreateBoundColumn("ProductCode", "Mã", true, 110));
            _cartGrid.Columns.Add(CreateBoundColumn("ProductName", "Sản phẩm", true, 220));
            _cartGrid.Columns.Add(CreateBoundColumn("StockOnHand", "Tồn", true, 80));
            _cartGrid.Columns.Add(CreateBoundColumn("Quantity", "SL", false, 70));
            _cartGrid.Columns.Add(CreateBoundColumn("UnitPrice", "Đơn giá", true, 110, "N0"));
            _cartGrid.Columns.Add(CreateBoundColumn("LineTotal", "Thành tiền", true, 120, "N0"));
        }

        private DataGridViewTextBoxColumn CreateBoundColumn(string propertyName, string headerText, bool readOnly, int width, string format = null)
        {
            var column = new DataGridViewTextBoxColumn
            {
                DataPropertyName = propertyName,
                Name = propertyName,
                HeaderText = headerText,
                ReadOnly = readOnly,
                Width = width,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            if (!string.IsNullOrWhiteSpace(format))
            {
                column.DefaultCellStyle.Format = format;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            return column;
        }

        private void ProductSelectionChanged(object sender, EventArgs e)
        {
            UpdateStockHint();
        }

        private void PaymentStatusCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSummary();
        }

        private void CartLines_ListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateSummary();
            UpdateCartEmptyState();
            UpdateButtonStates();
        }

        private void CartGrid_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void CartGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (_cartGrid.IsCurrentCellDirty)
            {
                _cartGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void CartGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            UiDialogs.ShowWarning("Vui lòng nhập số lượng hợp lệ.");
        }

        private void ProductSearchCombo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddProductToCart();
            }
        }

        private void CustomerSearchCombo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                _quantityUpDown.Focus();
            }
        }

        private void AddProductToCart()
        {
            var product = ResolveSelectedProduct();
            if (product == null)
            {
                UiDialogs.ShowWarning("Vui lòng chọn một sản phẩm từ danh sách trước.");
                return;
            }

            if (_quantityUpDown.Value <= 0)
            {
                UiDialogs.ShowWarning("Số lượng phải lớn hơn 0.");
                return;
            }

            var requestedQuantity = Convert.ToInt32(_quantityUpDown.Value);
            var existingLine = _cartLines.FirstOrDefault(line => line.ProductId == product.ProductId);
            var newQuantity = requestedQuantity + (existingLine == null ? 0 : existingLine.Quantity);

            if (newQuantity > product.StockOnHand)
            {
                UiDialogs.ShowWarning($"Chỉ còn {product.StockOnHand} sản phẩm cho {product.DisplayText}.");
                _stockLabel.ForeColor = UITheme.ErrorColor;
                _stockLabel.Text = $"Kiểm tra tồn kho không đạt: yêu cầu {newQuantity}, hiện còn {product.StockOnHand}.";
                return;
            }

            if (existingLine == null)
            {
                var cartLine = new CartLine(product.ProductId, product.ProductCode, product.ProductName, product.StockOnHand, requestedQuantity, product.UnitPrice);
                cartLine.PropertyChanged += CartLine_PropertyChanged;
                _cartLines.Add(cartLine);
            }
            else
            {
                existingLine.StockOnHand = product.StockOnHand;
                existingLine.Quantity = newQuantity;
            }

            _quantityUpDown.Value = 1;
            _productSearchCombo.Focus();
            _productSearchCombo.SelectAll();
            _statusLabel.Text = $"Đã thêm {product.DisplayText}.";
            _statusLabel.ForeColor = UITheme.SuccessColor;
            _invoicePrintService.SetInvoice(null);
            _previewButton.Enabled = false;
            _printButton.Enabled = false;
            UpdateSummary();
        }

        private void CartLine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartLine.Quantity) || e.PropertyName == nameof(CartLine.LineTotal))
            {
                UpdateSummary();
            }
        }

        private void UpdateStockHint()
        {
            var product = ResolveSelectedProduct();
            if (product == null)
            {
                _stockLabel.Text = "Chọn một sản phẩm để xem thông tin tồn kho.";
                _stockLabel.ForeColor = UITheme.TextSecondaryColor;
                return;
            }

            _stockLabel.Text = $"Số lượng còn lại: {product.StockOnHand}";
            _stockLabel.ForeColor = product.StockOnHand <= 5 ? UITheme.WarningColor : UITheme.SuccessColor;
        }

        private ProductLookupItem ResolveSelectedProduct()
        {
            if (_productSearchCombo.SelectedItem is ProductLookupItem selected)
            {
                return selected;
            }

            var text = _productSearchCombo.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return _productLookupItems.FirstOrDefault(item =>
                item.DisplayText.Equals(text, StringComparison.OrdinalIgnoreCase) ||
                (item.ProductCode ?? string.Empty).Equals(text, StringComparison.OrdinalIgnoreCase) ||
                (item.ProductName ?? string.Empty).IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                item.DisplayText.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private CustomerLookupItem ResolveSelectedCustomer()
        {
            if (_customerSearchCombo.SelectedItem is CustomerLookupItem selected)
            {
                return selected;
            }

            var text = _customerSearchCombo.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return _customerLookupItems.FirstOrDefault(item =>
                item.DisplayText.Equals(text, StringComparison.OrdinalIgnoreCase) ||
                (item.CustomerCode ?? string.Empty).Equals(text, StringComparison.OrdinalIgnoreCase) ||
                (item.FullName ?? string.Empty).IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                (item.Phone ?? string.Empty).IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void CartGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _cartGrid.Columns[e.ColumnIndex].Name != "Remove")
            {
                return;
            }

            var line = _cartGrid.Rows[e.RowIndex].DataBoundItem as CartLine;
            if (line != null)
            {
                RemoveCartLine(line);
            }
        }

        private void CartGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _cartGrid.Columns[e.ColumnIndex].Name != nameof(CartLine.Quantity))
            {
                return;
            }

            var line = _cartGrid.Rows[e.RowIndex].DataBoundItem as CartLine;
            if (line == null)
            {
                return;
            }

            var cellValue = Convert.ToString(_cartGrid.Rows[e.RowIndex].Cells[nameof(CartLine.Quantity)].Value);
            if (!int.TryParse(cellValue, out var quantity) || quantity <= 0)
            {
                UiDialogs.ShowWarning("Số lượng phải là số nguyên dương.");
                line.Quantity = 1;
                _cartGrid.InvalidateRow(e.RowIndex);
                return;
            }

            if (quantity > line.StockOnHand)
            {
                UiDialogs.ShowWarning($"Chỉ còn {line.StockOnHand} sản phẩm cho {line.ProductName}.");
                line.Quantity = line.StockOnHand;
                _cartGrid.InvalidateRow(e.RowIndex);
                return;
            }

            line.Quantity = quantity;
            UpdateSummary();
        }

        private void RemoveCartLine(CartLine line)
        {
            if (line == null)
            {
                return;
            }

            line.PropertyChanged -= CartLine_PropertyChanged;
            _cartLines.Remove(line);
            UpdateSummary();
        }

        private void UpdateCartEmptyState()
        {
            var hasItems = _cartLines.Count > 0;
            _cartGrid.Visible = hasItems;
            _emptyState.Visible = !hasItems;
            _cartGrid.BringToFront();
            _emptyState.BringToFront();
        }

        private void UpdateButtonStates()
        {
            var hasItems = _cartLines.Count > 0;
            _previewButton.Enabled = hasItems && _invoicePrintService.HasInvoice;
            _printButton.Enabled = hasItems && _invoicePrintService.HasInvoice;
            _createInvoiceButton.Enabled = hasItems;
        }

        private void UpdateSummary()
        {
            var subtotal = _cartLines.Sum(line => line.LineTotal);
            var discount = _discountUpDown.Value;
            var vatPercent = _vatUpDown.Value;
            var taxableBase = Math.Max(0m, subtotal - discount);
            var vatAmount = taxableBase * vatPercent / 100m;
            var grandTotal = taxableBase + vatAmount;
            var paymentStatus = string.IsNullOrWhiteSpace(_paymentStatusCombo.Text) ? "Đã thanh toán" : _paymentStatusCombo.Text;

            _itemCountLabel.Text = $"Số mặt hàng: {_cartLines.Count}";
            _subtotalValueLabel.Text = $"Tạm tính: {subtotal:N0}";
            _discountValueLabel.Text = $"Giảm giá: {discount:N0}";
            _vatValueLabel.Text = $"VAT: {vatAmount:N0}";
            _totalValueLabel.Text = $"Tổng cộng: {grandTotal:N0}";
            _paymentStatusValueLabel.Text = $"Trạng thái thanh toán: {paymentStatus}";
            _statusLabel.Text = _cartLines.Count == 0 ? "Giỏ hàng trống." : "Hóa đơn nháp đã sẵn sàng.";
            _statusLabel.ForeColor = UITheme.TextSecondaryColor;

            UpdateCartEmptyState();
            UpdateButtonStates();
        }

        private void ClearCart()
        {
            foreach (var line in _cartLines.ToList())
            {
                line.PropertyChanged -= CartLine_PropertyChanged;
            }

            _cartLines.Clear();
            _invoicePrintService.SetInvoice(null);
            _previewButton.Enabled = false;
            _printButton.Enabled = false;
            _statusLabel.Text = "Đã xóa giỏ hàng.";
            _statusLabel.ForeColor = UITheme.WarningColor;
            UpdateSummary();
        }

        private bool ShowPaymentConfirmation(decimal grandTotal)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Xác nhận thanh toán";
                dialog.Width = 450;
                dialog.Height = 320;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                UIHelper.ApplyFormTheme(dialog);
                FontHelper.ApplyVietnameseFontToForm(dialog);

                var root = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 4,
                    Padding = new Padding(24),
                    BackColor = UITheme.SurfaceColor
                };
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));  // Title
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));  // Total Amount
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Message
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));  // Buttons

                var titleLabel = new Label
                {
                    Text = "Xác nhận thanh toán cho hóa đơn này?",
                    Font = UITheme.BodyBoldFont,
                    ForeColor = UITheme.TextPrimaryColor,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                var totalLabel = new Label
                {
                    Text = DataGridViewHelper.FormatCurrencyVN(grandTotal),
                    Font = UITheme.HeroFont,
                    ForeColor = UITheme.PrimaryColor,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var detailLabel = new Label
                {
                    Text = "Thao tác này sẽ ghi nhận hóa đơn vào hệ thống và làm mới giỏ hàng. Vui lòng xác nhận số tiền trước khi tiếp tục.",
                    Font = UITheme.CaptionFont,
                    ForeColor = UITheme.TextSecondaryColor,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.TopLeft
                };

                var buttonFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
                var btnCancel = UIHelper.CreateSecondaryButton("Quay lại");
                btnCancel.Width = 110;
                btnCancel.Click += (s, e) => dialog.DialogResult = DialogResult.Cancel;
                
                var btnOk = UIHelper.CreatePrimaryButton("Xác nhận");
                btnOk.Width = 130;
                btnOk.Click += (s, e) => dialog.DialogResult = DialogResult.OK;

                buttonFlow.Controls.Add(btnCancel);
                buttonFlow.Controls.Add(btnOk);

                root.Controls.Add(titleLabel, 0, 0);
                root.Controls.Add(totalLabel, 0, 1);
                root.Controls.Add(detailLabel, 0, 2);
                root.Controls.Add(buttonFlow, 0, 3);

                dialog.Controls.Add(root);
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;

                return dialog.ShowDialog(this) == DialogResult.OK;
            }
        }

        private void ShowPrintPreviewWithConfirmation()
        {
            if (_invoicePrintService.HasInvoice)
            {
                _invoicePrintService.ShowPrintPreview(this);
            }
        }

        private void PrintInvoiceWithConfirmation()
        {
            if (_invoicePrintService.HasInvoice)
            {
                if (UiDialogs.Confirm("In hóa đơn ngay bây giờ?", "Xác nhận in"))
                {
                    _invoicePrintService.PrintNow(this);
                }
            }
        }

        private void ClearCartWithConfirmation()
        {
            if (_cartLines.Count == 0)
            {
                return;
            }

            if (UiDialogs.Confirm("Bạn có chắc chắn muốn xóa giỏ hàng? Thao tác này không thể hoàn tác.", "Xóa giỏ hàng"))
            {
                ClearCart();
            }
        }

        private void SalesPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                ClearCartWithConfirmation();
            }
            else if (e.KeyCode == Keys.F2 && _cartGrid.SelectedRows.Count > 0)
            {
                e.SuppressKeyPress = true;
                var selectedRow = _cartGrid.SelectedRows[0];
                _cartGrid.BeginEdit(false);
                _cartGrid.CurrentCell = selectedRow.Cells[nameof(CartLine.Quantity)];
            }
        }

        private void CartGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _cartGrid.SelectedRows.Count > 0)
            {
                e.SuppressKeyPress = true;
                var line = _cartGrid.SelectedRows[0].DataBoundItem as CartLine;
                if (line != null)
                {
                    RemoveCartLine(line);
                    _statusLabel.Text = $"Đã xóa {line.ProductName}.";
                    _statusLabel.ForeColor = UITheme.WarningColor;
                }
            }
        }

        private void DiscountOrVatChanged(object sender, EventArgs e)
        {
            UpdateSummary();
        }

        private void CreateInvoice()
        {
            if (_cartLines.Count == 0)
            {
                UiDialogs.ShowWarning("Giỏ hàng đang trống.");
                return;
            }

            if (!ShowPaymentConfirmation(_cartLines.Sum(line => line.LineTotal)))
            {
                _statusLabel.Text = "Đã hủy tạo hóa đơn.";
                _statusLabel.ForeColor = UITheme.WarningColor;
                return;
            }

            var customer = ResolveSelectedCustomer();
            var currentItems = _cartLines.Select(line => new SalesCartItem
            {
                ProductId = line.ProductId,
                ProductCode = line.ProductCode,
                ProductName = line.ProductName,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                LineTotal = line.LineTotal
            }).ToList();

            int? customerId = customer?.CustomerId;

            try
            {
                var orderId = _salesService.CreateInvoice(customerId, _currentUser.EmployeeId, _discountUpDown.Value, _vatUpDown.Value, currentItems);
                var snapshot = BuildInvoiceSnapshot(orderId, customer, currentItems);
                _invoiceNumberLabel.Text = $"Hóa đơn: {snapshot.InvoiceCode}";
                _statusLabel.Text = $"Đã tạo hóa đơn {snapshot.InvoiceCode} thành công.";
                _statusLabel.ForeColor = UITheme.SuccessColor;
                ClearCart();
                // Gán snapshot SAU khi xóa giỏ để giữ lại thông tin in
                _invoicePrintService.SetInvoice(snapshot);
                _lastInvoiceSnapshot = snapshot;
                _previewButton.Enabled = true;
                _printButton.Enabled = true;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Invoice creation failed", ex);
                UiDialogs.ShowError("Không thể tạo hóa đơn. Vui lòng kiểm tra tồn kho rồi thử lại.");
            }
        }

        private InvoiceSnapshot BuildInvoiceSnapshot(int orderId, CustomerLookupItem customer, List<SalesCartItem> cartItems)
        {
            var headerTable = DbHelper.ExecuteQuery(
                @"SELECT s.SalesOrderCode, s.OrderDate, s.SubTotal, s.Discount, s.VAT, s.TotalAmount, e.FullName AS CashierName
                  FROM SalesOrders s
                  INNER JOIN Employees e ON e.EmployeeId = s.CreatedBy
                  WHERE s.SalesOrderId = @id",
                new SQLiteParameter("@id", orderId));

            if (headerTable.Rows.Count == 0)
            {
                throw new InvalidOperationException("Unable to load saved invoice data.");
            }

            var headerRow = headerTable.Rows[0];
            var snapshot = new InvoiceSnapshot
            {
                InvoiceCode = Convert.ToString(headerRow["SalesOrderCode"]),
                InvoiceDate = Convert.ToDateTime(headerRow["OrderDate"]),
                CustomerName = customer == null ? null : customer.DisplayText,
                CreatedBy = Convert.ToString(headerRow["CashierName"]),
                SubTotal = Convert.ToDecimal(headerRow["SubTotal"]),
                Discount = Convert.ToDecimal(headerRow["Discount"]),
                VatAmount = Convert.ToDecimal(headerRow["VAT"]),
                TotalAmount = Convert.ToDecimal(headerRow["TotalAmount"]),
                PaymentStatus = string.IsNullOrWhiteSpace(_paymentStatusCombo.Text) ? "Paid" : _paymentStatusCombo.Text
            };

            if (string.IsNullOrWhiteSpace(snapshot.CustomerName) && customer != null)
            {
                snapshot.CustomerName = customer.DisplayText;
            }

            foreach (var item in cartItems)
            {
                snapshot.Items.Add(new InvoiceSnapshotItem
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.LineTotal
                });
            }

            return snapshot;
        }

        private void ShowLoading(bool show, string message = null)
        {
            if (show)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    _loadingOverlay.SetMessage(message);
                }

                _loadingOverlay.ShowOverlay();
                _loadingOverlay.BringToFront();
            }
            else
            {
                _loadingOverlay.HideOverlay();
            }
        }

        private Control MakeMoneyField(string label, NumericUpDown control)
        {
            var wrapper = new Panel { Width = 220, Height = 54, Margin = new Padding(0, 0, 0, 10) };
            var title = new Label { Dock = DockStyle.Top, Height = 16, Text = label, Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor };
            control.Dock = DockStyle.Bottom;
            control.Maximum = 100000000;
            control.DecimalPlaces = 0;
            control.Minimum = 0;
            UIHelper.StyleTextBox(control);
            control.ValueChanged += (s, e) => UpdateSummary();
            wrapper.Controls.Add(control);
            wrapper.Controls.Add(title);
            return wrapper;
        }

        public sealed class ProductLookupItem
        {
            public ProductLookupItem(Product product)
            {
                ProductId = product.ProductId;
                ProductCode = product.ProductCode;
                ProductName = product.ProductName;
                StockOnHand = product.Quantity;
                UnitPrice = product.UnitPrice;
                DisplayText = $"{product.ProductCode} - {product.ProductName}";
            }

            public int ProductId { get; }
            public string ProductCode { get; }
            public string ProductName { get; }
            public int StockOnHand { get; }
            public decimal UnitPrice { get; }
            public string DisplayText { get; }
        }

        public sealed class CustomerLookupItem
        {
            public int CustomerId { get; set; }
            public string CustomerCode { get; set; }
            public string FullName { get; set; }
            public string Phone { get; set; }
            public string DisplayText => $"{CustomerCode} - {FullName}";
        }

        public sealed class CartLine : INotifyPropertyChanged
        {
            private int _quantity;
            private int _stockOnHand;

            public CartLine(int productId, string productCode, string productName, int stockOnHand, int quantity, decimal unitPrice)
            {
                ProductId = productId;
                ProductCode = productCode;
                ProductName = productName;
                _stockOnHand = stockOnHand;
                _quantity = quantity;
                UnitPrice = unitPrice;
            }

            public int ProductId { get; }
            public string ProductCode { get; }
            public string ProductName { get; }
            public int StockOnHand
            {
                get => _stockOnHand;
                set
                {
                    if (_stockOnHand != value)
                    {
                        _stockOnHand = value;
                        OnPropertyChanged(nameof(StockOnHand));
                    }
                }
            }

            public int Quantity
            {
                get => _quantity;
                set
                {
                    if (value < 1)
                    {
                        throw new ArgumentException("Quantity must be greater than zero.");
                    }

                    if (value > StockOnHand)
                    {
                        throw new ArgumentException($"Only {StockOnHand} units available for {ProductName}.");
                    }

                    if (_quantity != value)
                    {
                        _quantity = value;
                        OnPropertyChanged(nameof(Quantity));
                        OnPropertyChanged(nameof(LineTotal));
                    }
                }
            }

            public decimal UnitPrice { get; }

            public decimal LineTotal => Quantity * UnitPrice;

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
