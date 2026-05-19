using System;
using DTO;

namespace GUI.WinForms
{
    /// <summary>
    /// Premium supplier editor with top-aligned labels, inline validation, and unsaved-changes guard.
    /// </summary>
    public class SupplierEditorForm : PremiumEditorForm
    {
        public Supplier Supplier { get; private set; }

        private System.Windows.Forms.TextBox _nameField, _contactField, _phoneField, _emailField, _addressField;

        public SupplierEditorForm(Supplier supplier = null)
        {
            Supplier = supplier ?? new Supplier();
            Text = Supplier.SupplierId == 0 ? "Thêm nhà cung cấp mới" : "Sửa nhà cung cấp";
            Width = 520;
            Height = 500;

            InitializePremiumLayout();
            _nameField = AddTextField("Tên nhà cung cấp", required: true, row: 0, column: 0);
            _contactField = AddTextField("Người liên hệ", row: 1, column: 0);
            _phoneField = AddTextField("Số điện thoại", row: 2, column: 0);
            _emailField = AddTextField("Email", row: 3, column: 0);
            _addressField = AddTextField("Địa chỉ", multiline: true, row: 4, column: 0);

            if (supplier != null)
            {
                _nameField.Text = supplier.SupplierName;
                _contactField.Text = supplier.ContactName;
                _phoneField.Text = supplier.Phone;
                _emailField.Text = supplier.Email;
                _addressField.Text = supplier.Address;
            }
        }

        protected override string GetFormIcon() => "🏭";
        protected override string GetPrimaryButtonText() => Supplier.SupplierId == 0 ? "Tạo mới" : "Cập nhật";

        protected override bool OnSave()
        {
            var name = ValidateRequired(_nameField, "Tên nhà cung cấp");
            if (name == null) return false;

            var email = _emailField.Text.Trim();
            if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
            {
                SetFieldError(_emailField, "Email không đúng định dạng.");
                _emailField.Focus();
                return false;
            }

            Supplier.SupplierName = name;
            Supplier.ContactName = _contactField.Text.Trim();
            Supplier.Phone = _phoneField.Text.Trim();
            Supplier.Email = email;
            Supplier.Address = _addressField.Text.Trim();
            return true;
        }
    }
}