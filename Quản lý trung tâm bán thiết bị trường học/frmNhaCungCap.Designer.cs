namespace Quản_lý_trung_tâm_bán_thiết_bị_trường_học
{
    partial class frmNhaCungCap
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
            btnXoaNhaCungCap = new Button();
            btnSuaNhaCungCap = new Button();
            btnThemNhaCungCap = new Button();
            btnLamMoiNhaCungCap = new Button();
            btnTimNhaCungCap = new Button();
            txtTimNhaCungCap = new TextBox();
            pnlNhaCungCap = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnXoaNhaCungCap);
            panel1.Controls.Add(btnSuaNhaCungCap);
            panel1.Controls.Add(btnThemNhaCungCap);
            panel1.Controls.Add(btnLamMoiNhaCungCap);
            panel1.Controls.Add(btnTimNhaCungCap);
            panel1.Controls.Add(txtTimNhaCungCap);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(958, 34);
            panel1.TabIndex = 2;
            // 
            // btnXoaNhaCungCap
            // 
            btnXoaNhaCungCap.Location = new Point(721, 2);
            btnXoaNhaCungCap.Name = "btnXoaNhaCungCap";
            btnXoaNhaCungCap.Size = new Size(94, 29);
            btnXoaNhaCungCap.TabIndex = 1;
            btnXoaNhaCungCap.Text = "Xóa";
            btnXoaNhaCungCap.UseVisualStyleBackColor = true;
            // 
            // btnSuaNhaCungCap
            // 
            btnSuaNhaCungCap.Location = new Point(621, 2);
            btnSuaNhaCungCap.Name = "btnSuaNhaCungCap";
            btnSuaNhaCungCap.Size = new Size(94, 29);
            btnSuaNhaCungCap.TabIndex = 1;
            btnSuaNhaCungCap.Text = "Sửa";
            btnSuaNhaCungCap.UseVisualStyleBackColor = true;
            // 
            // btnThemNhaCungCap
            // 
            btnThemNhaCungCap.Location = new Point(521, 2);
            btnThemNhaCungCap.Name = "btnThemNhaCungCap";
            btnThemNhaCungCap.Size = new Size(94, 29);
            btnThemNhaCungCap.TabIndex = 1;
            btnThemNhaCungCap.Text = "Thêm";
            btnThemNhaCungCap.UseVisualStyleBackColor = true;
            // 
            // btnLamMoiNhaCungCap
            // 
            btnLamMoiNhaCungCap.Location = new Point(421, 2);
            btnLamMoiNhaCungCap.Name = "btnLamMoiNhaCungCap";
            btnLamMoiNhaCungCap.Size = new Size(94, 29);
            btnLamMoiNhaCungCap.TabIndex = 1;
            btnLamMoiNhaCungCap.Text = "Làm mới";
            btnLamMoiNhaCungCap.UseVisualStyleBackColor = true;
            // 
            // btnTimNhaCungCap
            // 
            btnTimNhaCungCap.Location = new Point(321, 2);
            btnTimNhaCungCap.Name = "btnTimNhaCungCap";
            btnTimNhaCungCap.Size = new Size(94, 29);
            btnTimNhaCungCap.TabIndex = 1;
            btnTimNhaCungCap.Text = "Tìm";
            btnTimNhaCungCap.UseVisualStyleBackColor = true;
            // 
            // txtTimNhaCungCap
            // 
            txtTimNhaCungCap.Location = new Point(3, 3);
            txtTimNhaCungCap.Name = "txtTimNhaCungCap";
            txtTimNhaCungCap.Size = new Size(312, 27);
            txtTimNhaCungCap.TabIndex = 0;
            // 
            // pnlNhaCungCap
            // 
            pnlNhaCungCap.Location = new Point(12, 52);
            pnlNhaCungCap.Name = "pnlNhaCungCap";
            pnlNhaCungCap.Size = new Size(958, 489);
            pnlNhaCungCap.TabIndex = 3;
            // 
            // frmNhaCungCap
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 553);
            Controls.Add(pnlNhaCungCap);
            Controls.Add(panel1);
            Name = "frmNhaCungCap";
            Text = "Quản lý nhà cung cấp";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnXoaNhaCungCap;
        private Button btnSuaNhaCungCap;
        private Button btnThemNhaCungCap;
        private Button btnLamMoiNhaCungCap;
        private Button btnTimNhaCungCap;
        private TextBox txtTimNhaCungCap;
        private Panel pnlNhaCungCap;
    }
}