using System;
using System.Drawing;
using System.Windows.Forms;
using DTO;

namespace GUI.WinForms
{
    public class SupplierEditorForm : Form
    {
        public Supplier Supplier { get; private set; }

        private TextBox txtName;
        private TextBox txtContact;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtAddress;

        public SupplierEditorForm(Supplier supplier = null)
        {
            Supplier = supplier ?? new Supplier();
            InitializeComponents();
            if (supplier != null)
            {
                txtName.Text = supplier.SupplierName;
                txtContact.Text = supplier.ContactName;
                txtPhone.Text = supplier.Phone;
                txtEmail.Text = supplier.Email;
                txtAddress.Text = supplier.Address;
            }
        }

        private void InitializeComponents()
        {
            this.Text = Supplier.SupplierId == 0 ? "Thêm nhà cung cấp" : "Sửa nhà cung cấp";
            this.Width = 500;
            this.Height = 400;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            UIHelper.ApplyFormTheme(this);
            FontHelper.ApplyVietnameseFontToForm(this);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(24),
                BackColor = UITheme.BackgroundColor
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            
            for (int i = 0; i < 5; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            txtName = new TextBox { Dock = DockStyle.Fill };
            txtContact = new TextBox { Dock = DockStyle.Fill };
            txtPhone = new TextBox { Dock = DockStyle.Fill };
            txtEmail = new TextBox { Dock = DockStyle.Fill };
            txtAddress = new TextBox { Dock = DockStyle.Fill, Multiline = true };

            layout.Controls.Add(new Label { Text = "Tên NCC", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            layout.Controls.Add(txtName, 1, 0);

            layout.Controls.Add(new Label { Text = "Người liên hệ", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            layout.Controls.Add(txtContact, 1, 1);

            layout.Controls.Add(new Label { Text = "Điện thoại", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
            layout.Controls.Add(txtPhone, 1, 2);

            layout.Controls.Add(new Label { Text = "Email", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
            layout.Controls.Add(txtEmail, 1, 3);

            layout.Controls.Add(new Label { Text = "Địa chỉ", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 4);
            layout.Controls.Add(txtAddress, 1, 4);

            var buttonFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 10, 0, 0) };
            var btnCancel = UIHelper.CreateSecondaryButton("Hủy");
            btnCancel.Width = 100;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            
            var btnOk = UIHelper.CreatePrimaryButton("Lưu");
            btnOk.Width = 100;
            btnOk.Click += BtnOk_Click;

            buttonFlow.Controls.Add(btnCancel);
            buttonFlow.Controls.Add(btnOk);
            layout.Controls.Add(buttonFlow, 0, 5);
            layout.SetColumnSpan(buttonFlow, 2);

            this.Controls.Add(layout);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên nhà cung cấp là bắt buộc.");
                return;
            }

            Supplier.SupplierName = txtName.Text.Trim();
            Supplier.ContactName = txtContact.Text.Trim();
            Supplier.Phone = txtPhone.Text.Trim();
            Supplier.Email = txtEmail.Text.Trim();
            Supplier.Address = txtAddress.Text.Trim();
            DialogResult = DialogResult.OK;
        }
    }
}