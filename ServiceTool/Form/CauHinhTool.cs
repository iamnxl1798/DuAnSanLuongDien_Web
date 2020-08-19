using ServiceTool.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServiceTool
{
    public partial class CauHinhTool : Form
    {
        ConfigClass conf;
        public CauHinhTool(ConfigClass cf) : base()
        {
            this.conf = cf;
            InitializeComponent();
            txtQuet.Text = conf.ThuMucQuet;
            txtChuyen.Text = conf.ThuMucChuyen;
            ckAuto.Checked = conf.AutoRun;
        }

        private void BtRun_Click(object sender, EventArgs e)
        {
            conf.AutoRun = ckAuto.Checked;
            conf.ThuMucChuyen = txtChuyen.Text;
            conf.ThuMucQuet = txtQuet.Text;
            ConfigClass.SetConfig(conf);
            this.Close();
        }

        private void btBrowserQuet_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog op = new FolderBrowserDialog();
            op.Description = "Chọn thư mục để quét !!!";

            // Do not allow the user to create new files via the FolderBrowserDialog.
            op.ShowNewFolderButton = false;

            op.RootFolder = Environment.SpecialFolder.Desktop;
            if (op.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                txtQuet.Text = op.SelectedPath;

            }
        }

        private void btBrowserChuyen_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog op = new FolderBrowserDialog();
            op.Description = "Chọn thư mục để chuyển !!!";

            // Do not allow the user to create new files via the FolderBrowserDialog.
            op.ShowNewFolderButton = false;

            op.RootFolder = Environment.SpecialFolder.Desktop;
            if (op.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                txtChuyen.Text = op.SelectedPath;

            }
        }
    }
}

