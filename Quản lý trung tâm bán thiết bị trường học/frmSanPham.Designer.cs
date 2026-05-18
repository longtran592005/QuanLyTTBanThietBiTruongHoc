namespace Quản_lý_trung_tâm_bán_thiết_bị_trường_học
{
    partial class frmSanPham
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
            btnSuaSanPham = new Button();
            btnThemSanPham = new Button();
            btnLamMoiSanPham = new Button();
            btnTimSanPham = new Button();
            txtTimSanPham = new TextBox();
            pnlSanPham = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnXoaSanPham);
            panel1.Controls.Add(btnSuaSanPham);
            panel1.Controls.Add(btnThemSanPham);
            panel1.Controls.Add(btnLamMoiSanPham);
            panel1.Controls.Add(btnTimSanPham);
            panel1.Controls.Add(txtTimSanPham);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(958, 34);
            panel1.TabIndex = 0;
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
            // btnSuaSanPham
            // 
            btnSuaSanPham.Location = new Point(621, 2);
            btnSuaSanPham.Name = "btnSuaSanPham";
            btnSuaSanPham.Size = new Size(94, 29);
            btnSuaSanPham.TabIndex = 1;
            btnSuaSanPham.Text = "Sửa";
            btnSuaSanPham.UseVisualStyleBackColor = true;
            // 
            // btnThemSanPham
            // 
            btnThemSanPham.Location = new Point(521, 2);
            btnThemSanPham.Name = "btnThemSanPham";
            btnThemSanPham.Size = new Size(94, 29);
            btnThemSanPham.TabIndex = 1;
            btnThemSanPham.Text = "Thêm";
            btnThemSanPham.UseVisualStyleBackColor = true;
            // 
            // btnLamMoiSanPham
            // 
            btnLamMoiSanPham.Location = new Point(421, 2);
            btnLamMoiSanPham.Name = "btnLamMoiSanPham";
            btnLamMoiSanPham.Size = new Size(94, 29);
            btnLamMoiSanPham.TabIndex = 1;
            btnLamMoiSanPham.Text = "Làm mới";
            btnLamMoiSanPham.UseVisualStyleBackColor = true;
            // 
            // btnTimSanPham
            // 
            btnTimSanPham.Location = new Point(321, 2);
            btnTimSanPham.Name = "btnTimSanPham";
            btnTimSanPham.Size = new Size(94, 29);
            btnTimSanPham.TabIndex = 1;
            btnTimSanPham.Text = "Tìm";
            btnTimSanPham.UseVisualStyleBackColor = true;
            // 
            // txtTimSanPham
            // 
            txtTimSanPham.Location = new Point(3, 3);
            txtTimSanPham.Name = "txtTimSanPham";
            txtTimSanPham.Size = new Size(312, 27);
            txtTimSanPham.TabIndex = 0;
            // 
            // pnlSanPham
            // 
            pnlSanPham.Location = new Point(12, 52);
            pnlSanPham.Name = "pnlSanPham";
            pnlSanPham.Size = new Size(958, 489);
            pnlSanPham.TabIndex = 1;
            // 
            // frmSanPham
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 553);
            Controls.Add(pnlSanPham);
            Controls.Add(panel1);
            Name = "frmSanPham";
            Text = "Quản lý sản phẩm";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnThemSanPham;
        private Button btnLamMoiSanPham;
        private Button btnTimSanPham;
        private TextBox txtTimSanPham;
        private Button btnXoaSanPham;
        private Button btnSuaSanPham;
        private Panel pnlSanPham;
    }
}