namespace Quản_lý_trung_tâm_bán_thiết_bị_trường_học
{
    partial class frmKhachHang
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
            panel1 = new Panel();
            btnXoaKhachHang = new Button();
            btnSuaKhachHang = new Button();
            btnThemKhachHang = new Button();
            btnLamMoiKhachHang = new Button();
            btnTimKhachHang = new Button();
            txtTimKhachHang = new TextBox();
            pnlKhachHang = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnXoaKhachHang);
            panel1.Controls.Add(btnSuaKhachHang);
            panel1.Controls.Add(btnThemKhachHang);
            panel1.Controls.Add(btnLamMoiKhachHang);
            panel1.Controls.Add(btnTimKhachHang);
            panel1.Controls.Add(txtTimKhachHang);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(958, 34);
            panel1.TabIndex = 2;
            // 
            // btnXoaKhachHang
            // 
            btnXoaKhachHang.Location = new Point(721, 2);
            btnXoaKhachHang.Name = "btnXoaKhachHang";
            btnXoaKhachHang.Size = new Size(94, 29);
            btnXoaKhachHang.TabIndex = 1;
            btnXoaKhachHang.Text = "Xóa";
            btnXoaKhachHang.UseVisualStyleBackColor = true;
            // 
            // btnSuaKhachHang
            // 
            btnSuaKhachHang.Location = new Point(621, 2);
            btnSuaKhachHang.Name = "btnSuaKhachHang";
            btnSuaKhachHang.Size = new Size(94, 29);
            btnSuaKhachHang.TabIndex = 1;
            btnSuaKhachHang.Text = "Sửa";
            btnSuaKhachHang.UseVisualStyleBackColor = true;
            // 
            // btnThemKhachHang
            // 
            btnThemKhachHang.Location = new Point(521, 2);
            btnThemKhachHang.Name = "btnThemKhachHang";
            btnThemKhachHang.Size = new Size(94, 29);
            btnThemKhachHang.TabIndex = 1;
            btnThemKhachHang.Text = "Thêm";
            btnThemKhachHang.UseVisualStyleBackColor = true;
            // 
            // btnLamMoiKhachHang
            // 
            btnLamMoiKhachHang.Location = new Point(421, 2);
            btnLamMoiKhachHang.Name = "btnLamMoiKhachHang";
            btnLamMoiKhachHang.Size = new Size(94, 29);
            btnLamMoiKhachHang.TabIndex = 1;
            btnLamMoiKhachHang.Text = "Làm mới";
            btnLamMoiKhachHang.UseVisualStyleBackColor = true;
            // 
            // btnTimKhachHang
            // 
            btnTimKhachHang.Location = new Point(321, 2);
            btnTimKhachHang.Name = "btnTimKhachHang";
            btnTimKhachHang.Size = new Size(94, 29);
            btnTimKhachHang.TabIndex = 1;
            btnTimKhachHang.Text = "Tìm";
            btnTimKhachHang.UseVisualStyleBackColor = true;
            // 
            // txtTimKhachHang
            // 
            txtTimKhachHang.Location = new Point(3, 3);
            txtTimKhachHang.Name = "txtTimKhachHang";
            txtTimKhachHang.Size = new Size(312, 27);
            txtTimKhachHang.TabIndex = 0;
            // 
            // pnlKhachHang
            // 
            pnlKhachHang.Location = new Point(12, 52);
            pnlKhachHang.Name = "pnlKhachHang";
            pnlKhachHang.Size = new Size(958, 489);
            pnlKhachHang.TabIndex = 3;
            // 
            // frmKhachHang
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 553);
            Controls.Add(pnlKhachHang);
            Controls.Add(panel1);
            Name = "frmKhachHang";
            Text = "Quản lý Khách Hàng";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnXoaKhachHang;
        private Button btnSuaKhachHang;
        private Button btnThemKhachHang;
        private Button btnLamMoiKhachHang;
        private Button btnTimKhachHang;
        private TextBox txtTimKhachHang;
        private Panel pnlKhachHang;
    }
}