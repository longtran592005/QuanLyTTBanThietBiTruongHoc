using System;
using System.Drawing;
using System.Windows.Forms;
using BLL;

namespace GUI.WinForms
{
    public class BackupRestoreForm : Form
    {
        private readonly BackupService _service = new BackupService();
        private TextBox txtPath;

        public BackupRestoreForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Sao lưu & Phục hồi";
            this.Width = 550;
            this.Height = 250;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            UIHelper.ApplyFormTheme(this);
            FontHelper.ApplyVietnameseFontToForm(this);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                Padding = new Padding(20),
                BackColor = UITheme.BackgroundColor
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            txtPath = new TextBox { Dock = DockStyle.Fill };
            var btnBrowse = UIHelper.CreateSecondaryButton("Chọn file");
            btnBrowse.Width = 90;
            btnBrowse.Click += BtnBrowse_Click;

            layout.Controls.Add(new Label { Text = "Đường dẫn", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            layout.Controls.Add(txtPath, 1, 0);
            layout.Controls.Add(btnBrowse, 2, 0);

            var buttonFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(0, 10, 0, 0) };
            var btnBackup = UIHelper.CreatePrimaryButton("Sao lưu");
            btnBackup.Width = 140;
            btnBackup.Click += BtnBackup_Click;
            
            var btnRestore = UIHelper.CreateSecondaryButton("Phục hồi");
            btnRestore.Width = 140;
            btnRestore.Click += BtnRestore_Click;

            buttonFlow.Controls.Add(btnBackup);
            buttonFlow.Controls.Add(btnRestore);

            layout.Controls.Add(buttonFlow, 1, 1);
            layout.SetColumnSpan(buttonFlow, 2);

            this.Controls.Add(layout);
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "SQL Backup|*.bak";
                dialog.Title = "Choose backup file";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = dialog.FileName;
                }
            }
        }

        private void BtnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                _service.BackupDatabase(txtPath.Text.Trim());
                MessageBox.Show("Backup completed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Backup failed: " + ex.Message);
            }
        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                _service.RestoreDatabase(txtPath.Text.Trim());
                MessageBox.Show("Restore completed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Restore failed: " + ex.Message);
            }
        }
    }
}