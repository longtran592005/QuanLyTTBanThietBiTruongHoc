using System;
using System.Drawing;
using System.Windows.Forms;
using BLL;

namespace GUI.WinForms
{
    public class BackupRestorePage : UserControl
    {
        private readonly BackupService _service = new BackupService();
        private readonly TextBox _backupPathBox = new TextBox();
        private readonly TextBox _restorePathBox = new TextBox();

        public BackupRestorePage()
        {
            Dock = DockStyle.Fill;
            BackColor = UITheme.BackgroundColor;
            BuildLayout();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = UITheme.BackgroundColor };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            root.Controls.Add(BuildBackupCard(), 0, 0);
            root.Controls.Add(BuildRestoreCard(), 1, 0);
            Controls.Add(root);
        }

        private Control BuildBackupCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(card);
            var title = new Label { Dock = DockStyle.Top, Height = 28, Text = "Sao lưu cơ sở dữ liệu", Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            var hint = new Label { Dock = DockStyle.Top, Height = 44, Text = "Tạo một bản sao an toàn của tệp cơ sở dữ liệu SQLite.", ForeColor = UITheme.TextSecondaryColor };
            var layout = new TableLayoutPanel { Dock = DockStyle.Top, RowCount = 3, ColumnCount = 1, Height = 160 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));

            _backupPathBox.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(_backupPathBox);
            var browse = UIHelper.CreateSecondaryButton("Duyệt...");
            browse.Click += (s, e) => BrowseBackupPath();
            var backupButton = UIHelper.CreatePrimaryButton("Thực hiện sao lưu");
            backupButton.Click += (s, e) => RunBackup();

            layout.Controls.Add(_backupPathBox, 0, 0);
            layout.Controls.Add(browse, 0, 1);
            layout.Controls.Add(backupButton, 0, 2);
            card.Controls.Add(layout);
            card.Controls.Add(hint);
            card.Controls.Add(title);
            return card;
        }

        private Control BuildRestoreCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.SurfaceColor, Padding = new Padding(16) };
            UIHelper.StyleCard(card);
            var title = new Label { Dock = DockStyle.Top, Height = 28, Text = "Khôi phục cơ sở dữ liệu", Font = UITheme.SectionTitleFont, ForeColor = UITheme.TextPrimaryColor };
            var hint = new Label { Dock = DockStyle.Top, Height = 44, Text = "Khôi phục từ tệp .bak. Tệp cơ sở dữ liệu hiện tại sẽ bị ghi đè.", ForeColor = UITheme.TextSecondaryColor };
            var layout = new TableLayoutPanel { Dock = DockStyle.Top, RowCount = 3, ColumnCount = 1, Height = 160 };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));

            _restorePathBox.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(_restorePathBox);
            var browse = UIHelper.CreateSecondaryButton("Duyệt...");
            browse.Click += (s, e) => BrowseRestorePath();
            var restoreButton = UIHelper.CreateDangerButton("Thực hiện khôi phục");
            restoreButton.Click += (s, e) => RunRestore();

            layout.Controls.Add(_restorePathBox, 0, 0);
            layout.Controls.Add(browse, 0, 1);
            layout.Controls.Add(restoreButton, 0, 2);
            card.Controls.Add(layout);
            card.Controls.Add(hint);
            card.Controls.Add(title);
            return card;
        }

        private void BrowseBackupPath()
        {
            using (var dialog = new SaveFileDialog { Filter = "Backup File (*.bak)|*.bak", FileName = "SchoolDeviceStore.bak" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _backupPathBox.Text = dialog.FileName;
                }
            }
        }

        private void BrowseRestorePath()
        {
            using (var dialog = new OpenFileDialog { Filter = "Backup File (*.bak)|*.bak" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _restorePathBox.Text = dialog.FileName;
                }
            }
        }

        private void RunBackup()
        {
            if (string.IsNullOrWhiteSpace(_backupPathBox.Text))
            {
                MessageBox.Show("Hãy chọn nơi lưu bản sao lưu trước.");
                return;
            }

            _service.BackupDatabase(_backupPathBox.Text.Trim());
            MessageBox.Show("Sao lưu hoàn tất.");
        }

        private void RunRestore()
        {
            if (string.IsNullOrWhiteSpace(_restorePathBox.Text))
            {
                MessageBox.Show("Hãy chọn tệp sao lưu trước.");
                return;
            }

            if (MessageBox.Show("Khôi phục sẽ thay thế cơ sở dữ liệu hiện tại. Tiếp tục?", "Xác nhận khôi phục", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _service.RestoreDatabase(_restorePathBox.Text.Trim());
            MessageBox.Show("Khôi phục hoàn tất.");
        }
    }
}
