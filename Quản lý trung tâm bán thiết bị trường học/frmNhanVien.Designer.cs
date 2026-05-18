namespace Quản_lý_trung_tâm_bán_thiết_bị_trường_học
{
    partial class frmNhanVien
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
            btnXoaNhanVien = new Button();
            btnSuaNhanVien = new Button();
            btnThemNhanVien = new Button();
            btnLamMoiNhanVien = new Button();
            btnTimNhanVien = new Button();
            txtTimNhanVien = new TextBox();
            pnlNhanVien = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnXoaNhanVien);
            panel1.Controls.Add(btnSuaNhanVien);
            panel1.Controls.Add(btnThemNhanVien);
            panel1.Controls.Add(btnLamMoiNhanVien);
            panel1.Controls.Add(btnTimNhanVien);
            panel1.Controls.Add(txtTimNhanVien);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(958, 34);
            panel1.TabIndex = 2;
            // 
            // btnXoaNhanVien
            // 
            btnXoaNhanVien.Location = new Point(721, 2);
            btnXoaNhanVien.Name = "btnXoaNhanVien";
            btnXoaNhanVien.Size = new Size(94, 29);
            btnXoaNhanVien.TabIndex = 1;
            btnXoaNhanVien.Text = "Xóa";
            btnXoaNhanVien.UseVisualStyleBackColor = true;
            // 
            // btnSuaNhanVien
            // 
            btnSuaNhanVien.Location = new Point(621, 2);
            btnSuaNhanVien.Name = "btnSuaNhanVien";
            btnSuaNhanVien.Size = new Size(94, 29);
            btnSuaNhanVien.TabIndex = 1;
            btnSuaNhanVien.Text = "Sửa";
            btnSuaNhanVien.UseVisualStyleBackColor = true;
            // 
            // btnThemNhanVien
            // 
            btnThemNhanVien.Location = new Point(521, 2);
            btnThemNhanVien.Name = "btnThemNhanVien";
            btnThemNhanVien.Size = new Size(94, 29);
            btnThemNhanVien.TabIndex = 1;
            btnThemNhanVien.Text = "Thêm";
            btnThemNhanVien.UseVisualStyleBackColor = true;
            // 
            // btnLamMoiNhanVien
            // 
            btnLamMoiNhanVien.Location = new Point(421, 2);
            btnLamMoiNhanVien.Name = "btnLamMoiNhanVien";
            btnLamMoiNhanVien.Size = new Size(94, 29);
            btnLamMoiNhanVien.TabIndex = 1;
            btnLamMoiNhanVien.Text = "Làm mới";
            btnLamMoiNhanVien.UseVisualStyleBackColor = true;
            // 
            // btnTimNhanVien
            // 
            btnTimNhanVien.Location = new Point(321, 2);
            btnTimNhanVien.Name = "btnTimNhanVien";
            btnTimNhanVien.Size = new Size(94, 29);
            btnTimNhanVien.TabIndex = 1;
            btnTimNhanVien.Text = "Tìm";
            btnTimNhanVien.UseVisualStyleBackColor = true;
            // 
            // txtTimNhanVien
            // 
            txtTimNhanVien.Location = new Point(3, 3);
            txtTimNhanVien.Name = "txtTimNhanVien";
            txtTimNhanVien.Size = new Size(312, 27);
            txtTimNhanVien.TabIndex = 0;
            // 
            // pnlNhanVien
            // 
            pnlNhanVien.Location = new Point(12, 52);
            pnlNhanVien.Name = "pnlNhanVien";
            pnlNhanVien.Size = new Size(958, 489);
            pnlNhanVien.TabIndex = 3;
            // 
            // frmNhanVien
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 553);
            Controls.Add(pnlNhanVien);
            Controls.Add(panel1);
            Name = "frmNhanVien";
            Text = "Quản lý nhân viên";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnXoaNhanVien;
        private Button btnSuaNhanVien;
        private Button btnThemNhanVien;
        private Button btnLamMoiNhanVien;
        private Button btnTimNhanVien;
        private TextBox txtTimNhanVien;
        private Panel pnlNhanVien;
    }
}