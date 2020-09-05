namespace ServiceTool
{
    partial class CauHinhTool
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
            this.txtQuet = new System.Windows.Forms.TextBox();
            this.txtChuyen = new System.Windows.Forms.TextBox();
            this.ckAuto = new System.Windows.Forms.CheckBox();
            this.btSubmit = new System.Windows.Forms.Button();
            this.btBrowserChuyen = new System.Windows.Forms.Button();
            this.btBrowserQuet = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtQuet
            // 
            this.txtQuet.Location = new System.Drawing.Point(14, 64);
            this.txtQuet.Name = "txtQuet";
            this.txtQuet.Size = new System.Drawing.Size(431, 22);
            this.txtQuet.TabIndex = 5;
            // 
            // txtChuyen
            // 
            this.txtChuyen.Location = new System.Drawing.Point(14, 132);
            this.txtChuyen.Name = "txtChuyen";
            this.txtChuyen.Size = new System.Drawing.Size(431, 22);
            this.txtChuyen.TabIndex = 5;
            // 
            // ckAuto
            // 
            this.ckAuto.AutoSize = true;
            this.ckAuto.Location = new System.Drawing.Point(14, 184);
            this.ckAuto.Name = "ckAuto";
            this.ckAuto.Size = new System.Drawing.Size(249, 19);
            this.ckAuto.TabIndex = 3;
            this.ckAuto.Text = "Tự động chạy khi khởi động chương trình";
            this.ckAuto.UseVisualStyleBackColor = true;
            // 
            // btSubmit
            // 
            this.btSubmit.Location = new System.Drawing.Point(358, 184);
            this.btSubmit.Name = "btSubmit";
            this.btSubmit.Size = new System.Drawing.Size(87, 25);
            this.btSubmit.TabIndex = 4;
            this.btSubmit.Text = "Lưu";
            this.btSubmit.UseVisualStyleBackColor = true;
            this.btSubmit.Click += new System.EventHandler(this.BtRun_Click);
            // 
            // btBrowserChuyen
            // 
            this.btBrowserChuyen.Location = new System.Drawing.Point(477, 131);
            this.btBrowserChuyen.Name = "btBrowserChuyen";
            this.btBrowserChuyen.Size = new System.Drawing.Size(87, 25);
            this.btBrowserChuyen.TabIndex = 2;
            this.btBrowserChuyen.Text = "Chọn";
            this.btBrowserChuyen.UseVisualStyleBackColor = true;
            this.btBrowserChuyen.Click += new System.EventHandler(this.btBrowserChuyen_Click);
            // 
            // btBrowserQuet
            // 
            this.btBrowserQuet.Location = new System.Drawing.Point(477, 64);
            this.btBrowserQuet.Name = "btBrowserQuet";
            this.btBrowserQuet.Size = new System.Drawing.Size(87, 25);
            this.btBrowserQuet.TabIndex = 1;
            this.btBrowserQuet.Text = "Chọn";
            this.btBrowserQuet.UseVisualStyleBackColor = true;
            this.btBrowserQuet.Click += new System.EventHandler(this.btBrowserQuet_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Thư mục quét : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Thư mục chuyển : ";
            // 
            // CauHinhTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 249);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btBrowserQuet);
            this.Controls.Add(this.btBrowserChuyen);
            this.Controls.Add(this.btSubmit);
            this.Controls.Add(this.ckAuto);
            this.Controls.Add(this.txtChuyen);
            this.Controls.Add(this.txtQuet);
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CauHinhTool";
            this.Text = "Cấu Hình";
            this.Load += new System.EventHandler(this.CauHinhTool_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtQuet;
        private System.Windows.Forms.TextBox txtChuyen;
        private System.Windows.Forms.CheckBox ckAuto;
        private System.Windows.Forms.Button btSubmit;
        private System.Windows.Forms.Button btBrowserChuyen;
        private System.Windows.Forms.Button btBrowserQuet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}