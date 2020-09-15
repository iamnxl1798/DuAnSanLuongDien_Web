namespace ServiceTool
{
    partial class ServiceToolForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceToolForm));
            this.btRun = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.btConfig = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openFullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btRun
            // 
            this.btRun.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRun.Location = new System.Drawing.Point(29, 27);
            this.btRun.Name = "btRun";
            this.btRun.Size = new System.Drawing.Size(153, 62);
            this.btRun.TabIndex = 0;
            this.btRun.Text = "Chạy";
            this.btRun.UseVisualStyleBackColor = true;
            this.btRun.Click += new System.EventHandler(this.BtRun_Click);
            // 
            // btStop
            // 
            this.btStop.Enabled = false;
            this.btStop.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btStop.Location = new System.Drawing.Point(236, 27);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(153, 62);
            this.btStop.TabIndex = 1;
            this.btStop.Text = "Dừng";
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // btConfig
            // 
            this.btConfig.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btConfig.Location = new System.Drawing.Point(29, 123);
            this.btConfig.Name = "btConfig";
            this.btConfig.Size = new System.Drawing.Size(153, 62);
            this.btConfig.TabIndex = 2;
            this.btConfig.Text = "Cấu hình";
            this.btConfig.UseVisualStyleBackColor = true;
            this.btConfig.Click += new System.EventHandler(this.BtCauHinh_Click);
            // 
            // btCancel
            // 
            this.btCancel.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCancel.Location = new System.Drawing.Point(236, 123);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(153, 62);
            this.btCancel.TabIndex = 3;
            this.btCancel.Text = "Thoát";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.BtCancel_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "Is running !!!";
            this.notifyIcon.BalloonTipTitle = "Giam Sat";
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Giam Sat SL";
            this.notifyIcon.DoubleClick += new System.EventHandler(this.OpenFullScreenToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFullScreenToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.resumeToolStripMenuItem,
            this.exitApplicationToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(164, 92);
            // 
            // openFullScreenToolStripMenuItem
            // 
            this.openFullScreenToolStripMenuItem.Name = "openFullScreenToolStripMenuItem";
            this.openFullScreenToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.openFullScreenToolStripMenuItem.Text = "Open Full Screen";
            this.openFullScreenToolStripMenuItem.Click += new System.EventHandler(this.OpenFullScreenToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.btStop_Click);
            // 
            // resumeToolStripMenuItem
            // 
            this.resumeToolStripMenuItem.Name = "resumeToolStripMenuItem";
            this.resumeToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.resumeToolStripMenuItem.Text = "Resume";
            this.resumeToolStripMenuItem.Click += new System.EventHandler(this.BtRun_Click);
            // 
            // exitApplicationToolStripMenuItem
            // 
            this.exitApplicationToolStripMenuItem.Name = "exitApplicationToolStripMenuItem";
            this.exitApplicationToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exitApplicationToolStripMenuItem.Text = "Exit Application";
            this.exitApplicationToolStripMenuItem.Click += new System.EventHandler(this.BtCancel_Click);
            // 
            // ServiceToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 221);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btConfig);
            this.Controls.Add(this.btRun);
            this.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServiceToolForm";
            this.Text = "Giám Sát Dữ Liệu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServiceToolForm_FormClosing);
            this.Load += new System.EventHandler(this.ServiceToolForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btRun;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Button btConfig;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openFullScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitApplicationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resumeToolStripMenuItem;
    }
}

