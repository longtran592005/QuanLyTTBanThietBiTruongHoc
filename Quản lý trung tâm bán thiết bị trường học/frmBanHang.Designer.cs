namespace Quản_lý_trung_tâm_bán_thiết_bị_trường_học
{
    partial class frmBanHang
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
            btnXoaBanHang = new Button();
            btnSuaBanHang = new Button();
            btnThemBanHang = new Button();
            btnLamMoiBanHang = new Button();
            btnTimBanHang = new Button();
            txtTimBanHang = new TextBox();
            pnlBanHang = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnXoaBanHang);
            panel1.Controls.Add(btnSuaBanHang);
            panel1.Controls.Add(btnThemBanHang);
            panel1.Controls.Add(btnLamMoiBanHang);
            panel1.Controls.Add(btnTimBanHang);
            panel1.Controls.Add(txtTimBanHang);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(958, 34);
            panel1.TabIndex = 2;
            // 
            // btnXoaBanHang
            // 
            btnXoaBanHang.Location = new Point(721, 2);
            btnXoaBanHang.Name = "btnXoaBanHang";
            btnXoaBanHang.Size = new Size(94, 29);
            btnXoaBanHang.TabIndex = 1;
            btnXoaBanHang.Text = "Xóa";
            btnXoaBanHang.UseVisualStyleBackColor = true;
            // 
            // btnSuaBanHang
            // 
            btnSuaBanHang.Location = new Point(621, 2);
            btnSuaBanHang.Name = "btnSuaBanHang";
            btnSuaBanHang.Size = new Size(94, 29);
            btnSuaBanHang.TabIndex = 1;
            btnSuaBanHang.Text = "Sửa";
            btnSuaBanHang.UseVisualStyleBackColor = true;
            // 
            // btnThemBanHang
            // 
            btnThemBanHang.Location = new Point(521, 2);
            btnThemBanHang.Name = "btnThemBanHang";
            btnThemBanHang.Size = new Size(94, 29);
            btnThemBanHang.TabIndex = 1;
            btnThemBanHang.Text = "Thêm";
            btnThemBanHang.UseVisualStyleBackColor = true;
            // 
            // btnLamMoiBanHang
            // 
            btnLamMoiBanHang.Location = new Point(421, 2);
            btnLamMoiBanHang.Name = "btnLamMoiBanHang";
            btnLamMoiBanHang.Size = new Size(94, 29);
            btnLamMoiBanHang.TabIndex = 1;
            btnLamMoiBanHang.Text = "Làm mới";
            btnLamMoiBanHang.UseVisualStyleBackColor = true;
            // 
            // btnTimBanHang
            // 
            btnTimBanHang.Location = new Point(321, 2);
            btnTimBanHang.Name = "btnTimBanHang";
            btnTimBanHang.Size = new Size(94, 29);
            btnTimBanHang.TabIndex = 1;
            btnTimBanHang.Text = "Tìm";
            btnTimBanHang.UseVisualStyleBackColor = true;
            // 
            // txtTimBanHang
            // 
            txtTimBanHang.Location = new Point(3, 3);
            txtTimBanHang.Name = "txtTimBanHang";
            txtTimBanHang.Size = new Size(312, 27);
            txtTimBanHang.TabIndex = 0;
            // 
            // pnlBanHang
            // 
            pnlBanHang.Location = new Point(12, 52);
            pnlBanHang.Name = "pnlBanHang";
            pnlBanHang.Size = new Size(958, 489);
            pnlBanHang.TabIndex = 3;
            // 
            // frmBanHang
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 553);
            Controls.Add(pnlBanHang);
            Controls.Add(panel1);
            Name = "frmBanHang";
            Text = "Bán hàng/Hóa đơn";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnXoaBanHang;
        private Button btnSuaBanHang;
        private Button btnThemBanHang;
        private Button btnLamMoiBanHang;
        private Button btnTimBanHang;
        private TextBox txtTimBanHang;
        private Panel pnlBanHang;
    }
}