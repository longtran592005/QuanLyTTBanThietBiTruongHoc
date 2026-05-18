using System;
using System.Drawing;
using System.Windows.Forms;
using DTO;

namespace GUI.WinForms
{
    public class CategoryEditorForm : Form
    {
        public Category Category { get; private set; }

        private TextBox txtName;
        private TextBox txtDescription;

        public CategoryEditorForm(Category category = null)
        {
            Category = category ?? new Category();
            InitializeComponents();
            if (category != null)
            {
                txtName.Text = category.CategoryName;
                txtDescription.Text = category.Description;
            }
        }

        private void InitializeComponents()
        {
            this.Text = Category.CategoryId == 0 ? "Thêm danh mục" : "Sửa danh mục";
            this.Width = 460;
            this.Height = 280;
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
                RowCount = 3,
                Padding = new Padding(24),
                BackColor = UITheme.BackgroundColor
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

            txtName = new TextBox { Dock = DockStyle.Fill };
            txtDescription = new TextBox { Dock = DockStyle.Fill, Multiline = true };

            layout.Controls.Add(new Label { Text = "Tên danh mục", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            layout.Controls.Add(txtName, 1, 0);

            layout.Controls.Add(new Label { Text = "Mô tả", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            layout.Controls.Add(txtDescription, 1, 1);

            var buttonFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 10, 0, 0) };
            var btnCancel = UIHelper.CreateSecondaryButton("Hủy");
            btnCancel.Width = 100;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            
            var btnOk = UIHelper.CreatePrimaryButton("Lưu");
            btnOk.Width = 100;
            btnOk.Click += BtnOk_Click;

            buttonFlow.Controls.Add(btnCancel);
            buttonFlow.Controls.Add(btnOk);
            layout.Controls.Add(buttonFlow, 0, 2);
            layout.SetColumnSpan(buttonFlow, 2);

            this.Controls.Add(layout);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tên danh mục là bắt buộc.");
                return;
            }

            Category.CategoryName = txtName.Text.Trim();
            Category.Description = txtDescription.Text.Trim();
            DialogResult = DialogResult.OK;
        }
    }
}