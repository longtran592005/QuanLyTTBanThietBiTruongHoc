using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BLL;
using DTO;

namespace GUI.WinForms
{
    public class MainForm : Form
    {
        private readonly Employee _currentUser;
        private readonly Dictionary<string, Button> _navButtons = new Dictionary<string, Button>();
        private readonly Dictionary<string, UserControl> _pageCache = new Dictionary<string, UserControl>();

        private Panel _contentHost;
        private Label _pageTitleLabel;
        private Label _userInfoLabel;
        private Label _connectionLabel;
        private Label _roleLabel;
        private Label _clockLabel;
        private Timer _clockTimer;

        public MainForm(Employee user)
        {
            _currentUser = user;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "Quản lý thiết bị trường học";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1280, 800);
            BackColor = UITheme.BackgroundColor;
            Font = UITheme.BaseFont;
            DoubleBuffered = true;
            ControlBox = false; // Disable title bar buttons (close, min, max) as requested

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = UITheme.BackgroundColor
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, UITheme.SidebarWidth));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            root.Controls.Add(BuildSidebar(), 0, 0);
            root.Controls.Add(BuildShell(), 1, 0);
            Controls.Add(root);

            LoadPage(GetDashboardPage(), "Trang chủ", "dashboard");

            _clockTimer = new Timer { Interval = 1000 };
            _clockTimer.Tick += (s, e) => UpdateClock();
            _clockTimer.Start();
            UpdateClock();
        }

        private Control BuildSidebar()
        {
            var sidebar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.SidebarColor,
                Padding = new Padding(16, 18, 16, 16)
            };

            int roleId = _currentUser != null ? _currentUser.RoleId : 0;

            // Count visible nav items to determine row count
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 13,
                BackColor = Color.Transparent
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));  // Brand
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));  // Separator
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // Dashboard
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // Sales
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // Products
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // Categories
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // Suppliers
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // Promotions
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // Reports
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // Employees
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));  // Backup
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Spacer
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));  // Logout

            var brand = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            var brandTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                ForeColor = UITheme.TextPrimaryColor,
                Font = UITheme.TitleFont,
                Text = "Quản lý thiết bị trường học"
            };
            var brandSubtitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = UITheme.SidebarMutedTextColor,
                Font = UITheme.CaptionFont,
                Text = "Bộ quản lý nghiệp vụ"
            };
            brand.Controls.Add(brandSubtitle);
            brand.Controls.Add(brandTitle);
            layout.Controls.Add(brand, 0, 0);

            // Navigation buttons with permission-based visibility
            AddSidebarButton(layout, 2, "dashboard", "  Trang chủ", "\uE10F", () => LoadPage(GetDashboardPage(), "Trang chủ", "dashboard"));
            AddSidebarButton(layout, 3, "sales", "  Bán hàng", "\uE14D", () => LoadPage(GetSalesPage(), "Bán hàng", "sales"));
            AddSidebarButton(layout, 4, "products", "  Sản phẩm", "\uE14C", () => LoadPage(GetProductsPage(), "Sản phẩm", "products"));
            AddSidebarButton(layout, 5, "categories", "  Danh mục", "\uE1D3", () => LoadPage(GetCategoriesPage(), "Danh mục", "categories"));
            AddSidebarButton(layout, 6, "suppliers", "  Nhà cung cấp", "\uE13F", () => LoadPage(GetSuppliersPage(), "Nhà cung cấp", "suppliers"));
            AddSidebarButton(layout, 7, "promotions", "  Khuyến mãi", "\uE248", () => LoadPage(GetPromotionsPage(), "Khuyến mãi", "promotions"));
            AddSidebarButton(layout, 8, "reports", "  Báo cáo", "\uE1D5", () => LoadPage(GetReportsPage(), "Báo cáo", "reports"));
            AddSidebarButton(layout, 9, "employees", "  Nhân viên", "\uE13D", () => LoadPage(GetEmployeesPage(), "Quản lý nhân viên", "employees"));

            var backupButton = UIHelper.CreateSidebarButton("  Sao lưu / Khôi phục", "\uE114");
            backupButton.Name = "backup";
            backupButton.Tag = false;
            backupButton.Click += (s, e) => LoadPage(GetBackupPage(), "Sao lưu / Khôi phục", "backup");
            layout.Controls.Add(backupButton, 0, 10);
            _navButtons["backup"] = backupButton;

            var logoutButton = UIHelper.CreateSidebarDangerButton("  Thoát hệ thống", "\uE106");
            logoutButton.Name = "logout";
            logoutButton.Dock = DockStyle.Fill;
            logoutButton.Click += (s, e) =>
            {
                if (UiDialogs.Confirm("Bạn có chắc chắn muốn đăng xuất?\n\nMọi dữ liệu chưa lưu sẽ bị mất.", "Xác nhận đăng xuất"))
                    Close();
            };
            layout.Controls.Add(logoutButton, 0, 12);

            // Apply role-based visibility using centralized permission check
            ApplyRolePermissions(roleId);

            sidebar.Controls.Add(layout);
            return sidebar;
        }

        /// <summary>
        /// Apply visibility to sidebar buttons based on the user's role permissions.
        /// </summary>
        private void ApplyRolePermissions(int roleId)
        {
            SetNavVisibility("sales", EmployeeService.HasPermission(roleId, "sales"));
            SetNavVisibility("categories", EmployeeService.HasPermission(roleId, "categories"));
            SetNavVisibility("suppliers", EmployeeService.HasPermission(roleId, "suppliers"));
            SetNavVisibility("promotions", EmployeeService.HasPermission(roleId, "promotions"));
            SetNavVisibility("reports", EmployeeService.HasPermission(roleId, "reports"));
            SetNavVisibility("employees", EmployeeService.HasPermission(roleId, "employees"));
            SetNavVisibility("backup", EmployeeService.HasPermission(roleId, "backup"));
            // Products and Dashboard are visible to all roles
        }

        private void SetNavVisibility(string key, bool visible)
        {
            if (_navButtons.ContainsKey(key))
            {
                _navButtons[key].Visible = visible;
            }
        }

        private void AddSidebarButton(TableLayoutPanel layout, int row, string key, string text, string iconChar, Action action)
        {
            var button = UIHelper.CreateSidebarButton(text, iconChar);
            button.Name = key;
            button.Tag = false;
            button.Click += (s, e) => action();
            layout.Controls.Add(button, 0, row);
            _navButtons[key] = button;
        }

        private Control BuildShell()
        {
            var shell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = UITheme.BackgroundColor,
                Padding = new Padding(24, 24, 24, 24)
            };
            shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            shell.Controls.Add(BuildTopBar(), 0, 0);

            _contentHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.BackgroundColor,
                Padding = new Padding(0)
            };
            shell.Controls.Add(_contentHost, 0, 1);

            shell.Controls.Add(BuildStatusBar(), 0, 2);
            return shell;
        }

        private Control BuildTopBar()
        {
            var topBar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 16)
            };
            // Remove StyleCard from TopBar to make it blend with background

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            var titlePanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            _pageTitleLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 44, // Increased height
                Padding = new Padding(0, 0, 0, 8), // Breathing room
                Font = UITheme.TitleFont,
                ForeColor = UITheme.TextPrimaryColor,
                Text = "Tổng quan"
            };
            var pageHint = new Label
            {
                Dock = DockStyle.Top,
                Height = 36,
                Font = UITheme.CaptionFont,
                ForeColor = UITheme.TextSecondaryColor,
                Text = "Không gian làm việc doanh nghiệp thống nhất"
            };
            titlePanel.Controls.Add(pageHint);
            titlePanel.Controls.Add(_pageTitleLabel);

            // Removed shortcutHint label as requested

            var userPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            
            // Avatar Profile Bubble
            var avatar = new PictureBox
            {
                Width = 36, Height = 36,
                Dock = DockStyle.Right,
                Margin = new Padding(0, 0, 12, 0)
            };
            avatar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.FromArgb(235, 240, 255)))
                {
                    e.Graphics.FillEllipse(brush, 0, 0, 35, 35);
                }
                using (var font = new Font(UITheme.BodyBoldFont.FontFamily, 12F))
                using (var textBrush = new SolidBrush(UITheme.PrimaryColor))
                {
                    var initial = _currentUser != null ? _currentUser.FullName.Substring(0, 1).ToUpper() : "U";
                    var size = e.Graphics.MeasureString(initial, font);
                    e.Graphics.DrawString(initial, font, textBrush, (36 - size.Width) / 2 + 1, (36 - size.Height) / 2 + 1);
                }
            };

            var textPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 8, 0) };
            _userInfoLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 22,
                Font = UITheme.BodyBoldFont,
                ForeColor = UITheme.TextPrimaryColor,
                Text = _currentUser == null ? "System Administrator" : _currentUser.FullName,
                TextAlign = ContentAlignment.BottomRight
            };
            var userRole = new Label
            {
                Dock = DockStyle.Top,
                Height = 18,
                Font = new Font(UITheme.CaptionFont.FontFamily, 8F),
                ForeColor = UITheme.TextSecondaryColor,
                Text = _currentUser == null ? "Quản trị viên" : EmployeeService.GetRoleDisplayName(_currentUser.RoleId),
                TextAlign = ContentAlignment.TopRight
            };
            textPanel.Controls.Add(userRole);
            textPanel.Controls.Add(_userInfoLabel);
            
            userPanel.Controls.Add(textPanel);
            userPanel.Controls.Add(avatar);

            layout.Controls.Add(titlePanel, 0, 0);
            layout.SetColumnSpan(titlePanel, 2);
            layout.Controls.Add(userPanel, 2, 0);
            topBar.Controls.Add(layout);
            return topBar;
        }

        private Control BuildStatusBar()
        {
            var statusBar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.SubtleSurfaceColor, // Light gray
                Padding = new Padding(16, 0, 16, 0)
            };
            statusBar.Paint += (s, e) =>
            {
                // Top subtle border
                using (var pen = new Pen(UITheme.BorderColor))
                {
                    e.Graphics.DrawLine(pen, 0, 0, statusBar.Width, 0);
                }
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

            _connectionLabel = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UITheme.SuccessColor,
                Font = UITheme.CaptionFont,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Cơ sở dữ liệu: Đã kết nối"
            };
            _roleLabel = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UITheme.TextSecondaryColor,
                Font = UITheme.CaptionFont,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = _currentUser == null ? "Vai trò: Khách" : $"Vai trò: {EmployeeService.GetRoleDisplayName(_currentUser.RoleId)}"
            };
            _clockLabel = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UITheme.TextSecondaryColor,
                Font = UITheme.CaptionFont,
                TextAlign = ContentAlignment.MiddleRight,
                Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            };

            layout.Controls.Add(_connectionLabel, 0, 0);
            layout.Controls.Add(_roleLabel, 1, 0);
            layout.Controls.Add(_clockLabel, 2, 0);
            statusBar.Controls.Add(layout);
            return statusBar;
        }

        private void UpdateClock()
        {
            if (_clockLabel != null)
            {
                _clockLabel.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        private void LoadPage(UserControl page, string title, string navKey)
        {
            if (page == null)
                return;

            _contentHost.SuspendLayout();
            _contentHost.Controls.Clear();
            page.Dock = DockStyle.Fill;
            _contentHost.Controls.Add(page);
            _contentHost.ResumeLayout();

            _pageTitleLabel.Text = title;
            SetActiveNav(navKey);
        }

        private void SetActiveNav(string navKey)
        {
            foreach (var pair in _navButtons)
            {
                var isActive = pair.Key == navKey;
                UIHelper.SetSidebarButtonState(pair.Value, isActive);
            }
        }

        private DashboardPage GetDashboardPage()
        {
            return GetOrCreatePage("dashboard", () => new DashboardPage(_currentUser, NavigateTo));
        }

        private ProductManagementPage GetProductsPage()
        {
            return GetOrCreatePage("products", () => new ProductManagementPage());
        }

        private CategoryManagementPage GetCategoriesPage()
        {
            return GetOrCreatePage("categories", () => new CategoryManagementPage());
        }

        private SupplierManagementPage GetSuppliersPage()
        {
            return GetOrCreatePage("suppliers", () => new SupplierManagementPage());
        }

        private SalesPage GetSalesPage()
        {
            return GetOrCreatePage("sales", () => new SalesPage(_currentUser));
        }

        private ReportsPage GetReportsPage()
        {
            return GetOrCreatePage("reports", () => new ReportsPage());
        }

        private BackupRestorePage GetBackupPage()
        {
            return GetOrCreatePage("backup", () => new BackupRestorePage());
        }

        private EmployeeManagementPage GetEmployeesPage()
        {
            return GetOrCreatePage("employees", () => new EmployeeManagementPage());
        }

        private PromotionManagementPage GetPromotionsPage()
        {
            return GetOrCreatePage("promotions", () => new PromotionManagementPage());
        }

        private TPage GetOrCreatePage<TPage>(string key, Func<TPage> factory) where TPage : UserControl
        {
            if (_pageCache.ContainsKey(key))
            {
                return (TPage)_pageCache[key];
            }

            var page = factory();
            _pageCache[key] = page;
            return page;
        }

        private void NavigateTo(string key)
        {
            switch (key)
            {
                case "dashboard":
                    LoadPage(GetDashboardPage(), "Trang chủ", "dashboard");
                    break;
                case "sales":
                    LoadPage(GetSalesPage(), "Bán hàng", "sales");
                    break;
                case "products":
                    LoadPage(GetProductsPage(), "Sản phẩm", "products");
                    break;
                case "categories":
                    LoadPage(GetCategoriesPage(), "Danh mục", "categories");
                    break;
                case "suppliers":
                    LoadPage(GetSuppliersPage(), "Nhà cung cấp", "suppliers");
                    break;
                case "promotions":
                    LoadPage(GetPromotionsPage(), "Khuyến mãi", "promotions");
                    break;
                case "reports":
                    LoadPage(GetReportsPage(), "Báo cáo", "reports");
                    break;
                case "employees":
                    LoadPage(GetEmployeesPage(), "Quản lý nhân viên", "employees");
                    break;
                case "backup":
                    LoadPage(GetBackupPage(), "Sao lưu / Khôi phục", "backup");
                    break;
            }
        }
    }
}
