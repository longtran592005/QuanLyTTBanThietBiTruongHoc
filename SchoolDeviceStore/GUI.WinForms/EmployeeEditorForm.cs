using System;
using System.Windows.Forms;
using BLL;
using DTO;

namespace GUI.WinForms
{
    /// <summary>
    /// Premium employee editor with top-aligned labels, inline validation, and dynamic role mapping.
    /// </summary>
    public class EmployeeEditorForm : PremiumEditorForm
    {
        private readonly EmployeeService _service = new EmployeeService();
        private readonly int? _employeeId;
        private Employee _existingEmployee;

        private TextBox _usernameField;
        private TextBox _fullNameField;
        private TextBox _emailField;
        private TextBox _phoneField;
        private TextBox _passwordField;
        private ComboBox _roleField;
        private CheckBox _isActiveCheck;

        public EmployeeEditorForm(int? employeeId)
        {
            _employeeId = employeeId;
            Text = _employeeId.HasValue ? "Sửa thông tin nhân viên" : "Thêm nhân viên mới";
            Width = 520;
            Height = 560;

            InitializePremiumLayout();
            BuildFields();
            LoadData();
        }

        protected override string GetFormIcon() => "👥";
        protected override string GetPrimaryButtonText() => _employeeId.HasValue ? "Cập nhật" : "Tạo mới";

        private void BuildFields()
        {
            _usernameField = AddTextField("Tên đăng nhập", required: true, row: 0, column: 0);
            if (_employeeId.HasValue)
            {
                _usernameField.ReadOnly = true;
                _usernameField.BackColor = UITheme.SubtleSurfaceColor;
            }

            _fullNameField = AddTextField("Họ và tên", required: true, row: 1, column: 0);
            _emailField = AddTextField("Email", row: 2, column: 0);
            _phoneField = AddTextField("Số điện thoại", row: 3, column: 0);

            if (!_employeeId.HasValue)
            {
                _passwordField = AddPasswordField("Mật khẩu tài khoản", required: true, row: 4, column: 0);
            }
            else
            {
                // Simple read-only indicator
                var warnPanel = new Panel { Dock = DockStyle.Fill, Height = 64, Margin = new Padding(0, 0, 8, 4) };
                var lbl = new Label { Dock = DockStyle.Top, Height = 18, Text = "Mật khẩu", Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor };
                var hint = new TextBox { Dock = DockStyle.Top, Text = "••••••••", ReadOnly = true, BackColor = UITheme.SubtleSurfaceColor, Font = UITheme.BaseFont };
                var errorLbl = new Label { Dock = DockStyle.Bottom, Height = 14, Font = UITheme.CaptionFont, ForeColor = UITheme.TextSecondaryColor, Text = "(Sử dụng nút Đặt lại mật khẩu từ danh sách)" };
                
                warnPanel.Controls.Add(hint);
                warnPanel.Controls.Add(errorLbl);
                warnPanel.Controls.Add(lbl);

                ContentLayout.RowCount++;
                ContentLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
                ContentLayout.Controls.Add(warnPanel, 0, 4);
            }

            _roleField = AddDropdownField("Vai trò phân quyền", required: true, row: 5, column: 0);
            _isActiveCheck = AddCheckboxField("Tài khoản đang hoạt động", row: 6, column: 0);
            _isActiveCheck.Checked = true;
        }

        private void LoadData()
        {
            // Bind role dropdown
            var roles = _service.GetRoles();
            _roleField.DataSource = roles;
            _roleField.DisplayMember = "RoleName";
            _roleField.ValueMember = "RoleId";

            if (_employeeId.HasValue)
            {
                _existingEmployee = _service.GetById(_employeeId.Value);
                if (_existingEmployee != null)
                {
                    _usernameField.Text = _existingEmployee.Username;
                    _fullNameField.Text = _existingEmployee.FullName;
                    _emailField.Text = _existingEmployee.Email;
                    _phoneField.Text = _existingEmployee.Phone;
                    _roleField.SelectedValue = _existingEmployee.RoleId;
                    _isActiveCheck.Checked = _existingEmployee.IsActive;
                }
            }
        }

        protected override bool OnSave()
        {
            var username = ValidateRequired(_usernameField, "Tên đăng nhập");
            var fullName = ValidateRequired(_fullNameField, "Họ và tên");
            
            if (username == null || fullName == null) return false;
            
            if (!_employeeId.HasValue)
            {
                var password = ValidateRequired(_passwordField, "Mật khẩu");
                if (password == null) return false;
                if (password.Length < 6)
                {
                    SetFieldError(_passwordField, "Mật khẩu phải từ 6 ký tự trở lên.");
                    return false;
                }
            }

            var email = _emailField.Text.Trim();
            if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
            {
                SetFieldError(_emailField, "Email không đúng định dạng.");
                return false;
            }

            if (_employeeId.HasValue && _existingEmployee != null)
            {
                _existingEmployee.FullName = fullName;
                _existingEmployee.Email = email;
                _existingEmployee.Phone = _phoneField.Text.Trim();
                _existingEmployee.RoleId = Convert.ToInt32(_roleField.SelectedValue);
                _existingEmployee.IsActive = _isActiveCheck.Checked;

                _service.UpdateEmployee(_existingEmployee);
            }
            else
            {
                var newEmp = new Employee
                {
                    Username = username,
                    FullName = fullName,
                    Email = email,
                    Phone = _phoneField.Text.Trim(),
                    RoleId = Convert.ToInt32(_roleField.SelectedValue),
                    IsActive = _isActiveCheck.Checked
                };

                try
                {
                    _service.CreateEmployee(newEmp, _passwordField.Text);
                }
                catch (Exception ex)
                {
                    SetFieldError(_usernameField, ex.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
