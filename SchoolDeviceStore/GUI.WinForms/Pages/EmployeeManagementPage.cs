using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BLL;

namespace GUI.WinForms
{
    /// <summary>
    /// Employee management page (Admin only).
    /// Supports viewing, adding, editing, deactivating/activating employees and resetting passwords.
    /// </summary>
    public class EmployeeManagementPage : UserControl
    {
        private readonly EmployeeService _service = new EmployeeService();
        private readonly DataGridView _grid = new DataGridView();
        private readonly ComboBox _roleFilter = new ComboBox();
        private readonly ComboBox _statusFilter = new ComboBox();

        public EmployeeManagementPage()
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;
            BuildLayout();
            Load += (s, e) => RefreshGrid();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = UITheme.BackgroundColor
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.Controls.Add(BuildToolbar(), 0, 0);
            root.Controls.Add(BuildGridCard(), 0, 1);
            Controls.Add(root);
        }

        private Control BuildToolbar()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16, 8, 16, 8) };
            UIHelper.StyleCard(panel);

            var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };

            var addBtn = UIHelper.CreatePrimaryButton("Thêm nhân viên");
            addBtn.Click += (s, e) => ShowEmployeeEditor(null);
            layout.Controls.Add(addBtn);

            var editBtn = UIHelper.CreateSecondaryButton("Sửa");
            editBtn.Click += (s, e) => EditSelectedEmployee();
            layout.Controls.Add(editBtn);

            var deactivateBtn = UIHelper.CreateOutlineDangerButton("Khóa tài khoản");
            deactivateBtn.Click += (s, e) => ToggleSelectedEmployeeStatus();
            layout.Controls.Add(deactivateBtn);

            var resetPwdBtn = UIHelper.CreateSecondaryButton("Đặt lại mật khẩu");
            resetPwdBtn.Click += (s, e) => ResetSelectedPassword();
            layout.Controls.Add(resetPwdBtn);

            // Role filter
            var roleLbl = new Label { Text = "  Vai trò:", AutoSize = true, TextAlign = ContentAlignment.MiddleCenter, Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor, Margin = new Padding(12, 10, 4, 0) };
            layout.Controls.Add(roleLbl);
            _roleFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _roleFilter.Width = 140;
            _roleFilter.Items.Add("Tất cả");
            _roleFilter.Items.Add("Admin");
            _roleFilter.Items.Add("Manager");
            _roleFilter.Items.Add("Salesperson");
            _roleFilter.Items.Add("Warehouse");
            _roleFilter.Items.Add("Accountant");
            _roleFilter.SelectedIndex = 0;
            _roleFilter.SelectedIndexChanged += (s, e) => RefreshGrid();
            layout.Controls.Add(_roleFilter);

            // Status filter
            var statusLbl = new Label { Text = "  Trạng thái:", AutoSize = true, TextAlign = ContentAlignment.MiddleCenter, Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor, Margin = new Padding(8, 10, 4, 0) };
            layout.Controls.Add(statusLbl);
            _statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _statusFilter.Width = 120;
            _statusFilter.Items.Add("Tất cả");
            _statusFilter.Items.Add("Hoạt động");
            _statusFilter.Items.Add("Đã khóa");
            _statusFilter.SelectedIndex = 0;
            _statusFilter.SelectedIndexChanged += (s, e) => RefreshGrid();
            layout.Controls.Add(_statusFilter);

            panel.Controls.Add(layout);
            return panel;
        }

        private Control BuildGridCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16), Margin = new Padding(0, 12, 0, 0) };
            UIHelper.StyleCard(card);
            UIHelper.StyleDataGridView(_grid);
            _grid.Dock = DockStyle.Fill;
            _grid.ReadOnly = true;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AllowUserToAddRows = false;
            _grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) EditSelectedEmployee(); };
            card.Controls.Add(_grid);
            return card;
        }

        private void RefreshGrid()
        {
            try
            {
                var dt = _service.GetAllAsDataTable();
                var view = new DataView(dt);

                // Apply filters
                var filters = new System.Collections.Generic.List<string>();
                if (_roleFilter.SelectedIndex > 0)
                {
                    var roleName = _roleFilter.SelectedItem.ToString();
                    filters.Add($"[Vai trò] = '{roleName}'");
                }
                if (_statusFilter.SelectedIndex > 0)
                {
                    var status = _statusFilter.SelectedItem.ToString();
                    filters.Add($"[Trạng thái] = '{status}'");
                }

                if (filters.Count > 0)
                    view.RowFilter = string.Join(" AND ", filters);

                _grid.DataSource = view;

                // Format columns
                if (_grid.Columns.Count > 0)
                {
                    foreach (DataGridViewColumn col in _grid.Columns)
                    {
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    if (_grid.Columns.Contains("Họ tên"))
                        _grid.Columns["Họ tên"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Employee grid refresh failed", ex);
                UiDialogs.ShowError("Không thể tải danh sách nhân viên.");
            }
        }

        private void ShowEmployeeEditor(int? employeeId)
        {
            using (var dialog = new EmployeeEditorForm(employeeId))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshGrid();
                }
            }
        }

        private void EditSelectedEmployee()
        {
            if (_grid.SelectedRows.Count == 0)
            {
                UiDialogs.ShowWarning("Vui lòng chọn nhân viên cần sửa.");
                return;
            }

            var row = _grid.SelectedRows[0];
            var employeeId = Convert.ToInt32(row.Cells["Mã NV"].Value);
            ShowEmployeeEditor(employeeId);
        }

        private void ToggleSelectedEmployeeStatus()
        {
            if (_grid.SelectedRows.Count == 0)
            {
                UiDialogs.ShowWarning("Vui lòng chọn nhân viên.");
                return;
            }

            var row = _grid.SelectedRows[0];
            var employeeId = Convert.ToInt32(row.Cells["Mã NV"].Value);
            var currentStatus = row.Cells["Trạng thái"].Value.ToString();
            var name = row.Cells["Họ tên"].Value.ToString();

            try
            {
                if (currentStatus == "Hoạt động")
                {
                    if (employeeId == 1)
                    {
                        UiDialogs.ShowWarning("Không thể khóa tài khoản quản trị viên chính.");
                        return;
                    }
                    if (UiDialogs.Confirm($"Khóa tài khoản của nhân viên \"{name}\"?\n\nNhân viên sẽ không thể đăng nhập.", "Xác nhận khóa"))
                    {
                        _service.DeactivateEmployee(employeeId);
                        UiDialogs.ShowInfo($"Đã khóa tài khoản của {name}.");
                    }
                }
                else
                {
                    if (UiDialogs.Confirm($"Kích hoạt lại tài khoản của nhân viên \"{name}\"?", "Xác nhận kích hoạt"))
                    {
                        _service.ActivateEmployee(employeeId);
                        UiDialogs.ShowInfo($"Đã kích hoạt lại tài khoản của {name}.");
                    }
                }

                RefreshGrid();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Toggle employee status failed", ex);
                UiDialogs.ShowError("Thao tác thất bại.");
            }
        }

        private void ResetSelectedPassword()
        {
            if (_grid.SelectedRows.Count == 0)
            {
                UiDialogs.ShowWarning("Vui lòng chọn nhân viên.");
                return;
            }

            var row = _grid.SelectedRows[0];
            var employeeId = Convert.ToInt32(row.Cells["Mã NV"].Value);
            var name = row.Cells["Họ tên"].Value.ToString();

            using (var dialog = new Form())
            {
                dialog.Text = $"Đặt lại mật khẩu - {name}";
                dialog.Width = 400;
                dialog.Height = 220;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                UIHelper.ApplyFormTheme(dialog);
                FontHelper.ApplyVietnameseFontToForm(dialog);

                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(24) };
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

                var label = new Label { Text = "Nhập mật khẩu mới:", Dock = DockStyle.Fill, Font = UITheme.BodyBoldFont, ForeColor = UITheme.TextPrimaryColor };
                var textBox = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true, Font = UITheme.BaseFont };
                var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
                var btnCancel = UIHelper.CreateSecondaryButton("Hủy");
                btnCancel.Click += (s2, e2) => dialog.DialogResult = DialogResult.Cancel;
                var btnOk = UIHelper.CreatePrimaryButton("Đặt lại");
                btnOk.Click += (s2, e2) =>
                {
                    try
                    {
                        _service.ResetPassword(employeeId, textBox.Text);
                        UiDialogs.ShowInfo($"Đã đặt lại mật khẩu cho {name}.");
                        dialog.DialogResult = DialogResult.OK;
                    }
                    catch (Exception ex)
                    {
                        UiDialogs.ShowError(ex.Message);
                    }
                };

                btnPanel.Controls.Add(btnCancel);
                btnPanel.Controls.Add(btnOk);
                layout.Controls.Add(label, 0, 0);
                layout.Controls.Add(textBox, 0, 1);
                layout.Controls.Add(btnPanel, 0, 2);
                dialog.Controls.Add(layout);
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;
                dialog.ShowDialog(this);
            }
        }
    }
}
