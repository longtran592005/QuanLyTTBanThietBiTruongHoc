namespace Quản_lý_trung_tâm_bán_thiết_bị_trường_học
{
    partial class frmDanhMuc
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
            btnXoaSanPham = new Button();
            btnSuaDanhMuc = new Button();
            btnThemDanhMuc = new Button();
            btnLamMoiDanhMuc = new Button();
            btnTimDanhMuc = new Button();
            txtTimDanhMuc = new TextBox();
            pnlDanhMuc = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnXoaSanPham);
            panel1.Controls.Add(btnSuaDanhMuc);
            panel1.Controls.Add(btnThemDanhMuc);
            panel1.Controls.Add(btnLamMoiDanhMuc);
            panel1.Controls.Add(btnTimDanhMuc);
            panel1.Controls.Add(txtTimDanhMuc);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(958, 34);
            panel1.TabIndex = 1;
            // 
            // btnXoaSanPham
            // 
            btnXoaSanPham.Location = new Point(721, 2);
            btnXoaSanPham.Name = "btnXoaSanPham";
            btnXoaSanPham.Size = new Size(94, 29);
            btnXoaSanPham.TabIndex = 1;
            btnXoaSanPham.Text = "Xóa";
            btnXoaSanPham.UseVisualStyleBackColor = true;
            // 
            // btnSuaDanhMuc
            // 
            btnSuaDanhMuc.Location = new Point(621, 2);
            btnSuaDanhMuc.Name = "btnSuaDanhMuc";
            btnSuaDanhMuc.Size = new Size(94, 29);
            btnSuaDanhMuc.TabIndex = 1;
            btnSuaDanhMuc.Text = "Sửa";
            btnSuaDanhMuc.UseVisualStyleBackColor = true;
            // 
            // btnThemDanhMuc
            // 
            btnThemDanhMuc.Location = new Point(521, 2);
            btnThemDanhMuc.Name = "btnThemDanhMuc";
            btnThemDanhMuc.Size = new Size(94, 29);
            btnThemDanhMuc.TabIndex = 1;
            btnThemDanhMuc.Text = "Thêm";
            btnThemDanhMuc.UseVisualStyleBackColor = true;
            // 
            // btnLamMoiDanhMuc
            // 
            btnLamMoiDanhMuc.Location = new Point(421, 2);
            btnLamMoiDanhMuc.Name = "btnLamMoiDanhMuc";
            btnLamMoiDanhMuc.Size = new Size(94, 29);
            btnLamMoiDanhMuc.TabIndex = 1;
            btnLamMoiDanhMuc.Text = "Làm mới";
            btnLamMoiDanhMuc.UseVisualStyleBackColor = true;
            // 
            // btnTimDanhMuc
            // 
            btnTimDanhMuc.Location = new Point(321, 2);
            btnTimDanhMuc.Name = "btnTimDanhMuc";
            btnTimDanhMuc.Size = new Size(94, 29);
            btnTimDanhMuc.TabIndex = 1;
            btnTimDanhMuc.Text = "Tìm";
            btnTimDanhMuc.UseVisualStyleBackColor = true;
            // 
            // txtTimDanhMuc
            // 
            txtTimDanhMuc.Location = new Point(3, 3);
            txtTimDanhMuc.Name = "txtTimDanhMuc";
            txtTimDanhMuc.Size = new Size(312, 27);
            txtTimDanhMuc.TabIndex = 0;
            // 
            // pnlDanhMuc
            // 
            pnlDanhMuc.Location = new Point(12, 52);
            pnlDanhMuc.Name = "pnlDanhMuc";
            pnlDanhMuc.Size = new Size(958, 489);
            pnlDanhMuc.TabIndex = 2;
            // 
            // frmDanhMuc
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 553);
            Controls.Add(pnlDanhMuc);
            Controls.Add(panel1);
            Name = "frmDanhMuc";
            Text = "Quản lý danh mục";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnXoaSanPham;
        private Button btnSuaDanhMuc;
        private Button btnThemDanhMuc;
        private Button btnLamMoiDanhMuc;
        private Button btnTimDanhMuc;
        private TextBox txtTimDanhMuc;
        private Panel pnlDanhMuc;
    }
}