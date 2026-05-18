namespace Quản_lý_trung_tâm_bán_thiết_bị_trường_học
{
    partial class frmTrangChu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            tsmiTrangChu = new ToolStripMenuItem();
            tsmiSanPham = new ToolStripMenuItem();
            tsmiDanhMuc = new ToolStripMenuItem();
            tsmiBanHang = new ToolStripMenuItem();
            tsmiNhaCungCap = new ToolStripMenuItem();
            tsmiNhanVien = new ToolStripMenuItem();
            tsmiKhachHang = new ToolStripMenuItem();
            tsmiBackUpOrRestore = new ToolStripMenuItem();
            tsmiThongKe = new ToolStripMenuItem();
            tsmiDangXuat = new ToolStripMenuItem();
            pnlChinh = new Panel();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { tsmiTrangChu, tsmiSanPham, tsmiDanhMuc, tsmiBanHang, tsmiNhaCungCap, tsmiNhanVien, tsmiKhachHang, tsmiBackUpOrRestore, tsmiThongKe, tsmiDangXuat });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1076, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "mnuTrangChu";
            // 
            // tsmiTrangChu
            // 
            tsmiTrangChu.Name = "tsmiTrangChu";
            tsmiTrangChu.Size = new Size(87, 24);
            tsmiTrangChu.Text = "Trang chủ";
            // 
            // tsmiSanPham
            // 
            tsmiSanPham.Name = "tsmiSanPham";
            tsmiSanPham.Size = new Size(89, 24);
            tsmiSanPham.Text = "Sản phẩm";
            // 
            // tsmiDanhMuc
            // 
            tsmiDanhMuc.Name = "tsmiDanhMuc";
            tsmiDanhMuc.Size = new Size(90, 24);
            tsmiDanhMuc.Text = "Danh mục";
            // 
            // tsmiBanHang
            // 
            tsmiBanHang.Name = "tsmiBanHang";
            tsmiBanHang.Size = new Size(85, 24);
            tsmiBanHang.Text = "Bán hàng";
            // 
            // tsmiNhaCungCap
            // 
            tsmiNhaCungCap.Name = "tsmiNhaCungCap";
            tsmiNhaCungCap.Size = new Size(114, 24);
            tsmiNhaCungCap.Text = "Nhà cung cấp";
            // 
            // tsmiNhanVien
            // 
            tsmiNhanVien.Name = "tsmiNhanVien";
            tsmiNhanVien.Size = new Size(89, 24);
            tsmiNhanVien.Text = "Nhân viên";
            // 
            // tsmiKhachHang
            // 
            tsmiKhachHang.Name = "tsmiKhachHang";
            tsmiKhachHang.Size = new Size(100, 24);
            tsmiKhachHang.Text = "Khách hàng";
            // 
            // tsmiBackUpOrRestore
            // 
            tsmiBackUpOrRestore.Name = "tsmiBackUpOrRestore";
            tsmiBackUpOrRestore.Size = new Size(127, 24);
            tsmiBackUpOrRestore.Text = "Backup/Restore";
            // 
            // tsmiThongKe
            // 
            tsmiThongKe.Name = "tsmiThongKe";
            tsmiThongKe.Size = new Size(84, 24);
            tsmiThongKe.Text = "Thống kê";
            // 
            // tsmiDangXuat
            // 
            tsmiDangXuat.Name = "tsmiDangXuat";
            tsmiDangXuat.Size = new Size(91, 24);
            tsmiDangXuat.Text = "Đăng xuất";
            // 
            // pnlChinh
            // 
            pnlChinh.Dock = DockStyle.Fill;
            pnlChinh.Location = new Point(0, 28);
            pnlChinh.Name = "pnlChinh";
            pnlChinh.Size = new Size(1076, 622);
            pnlChinh.TabIndex = 1;
            // 
            // frmTrangChu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1076, 650);
            Controls.Add(pnlChinh);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "frmTrangChu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Trang chủ";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem tsmiTrangChu;
        private ToolStripMenuItem tsmiSanPham;
        private ToolStripMenuItem tsmiDanhMuc;
        private ToolStripMenuItem tsmiNhanVien;
        private ToolStripMenuItem tsmiNhaCungCap;
        private ToolStripMenuItem tsmiBanHang;
        private ToolStripMenuItem tsmiBackUpOrRestore;
        private ToolStripMenuItem tsmiThongKe;
        private ToolStripMenuItem tsmiDangXuat;
        private ToolStripMenuItem tsmiKhachHang;
        private Panel pnlChinh;
    }
}