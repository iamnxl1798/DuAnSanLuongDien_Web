using DocDuLieuCongTo.Model;
using ServiceTool.Model;
using ServiceTool.Model.CustomModel;
using ServiceTool.Model.DbModel;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static DocDuLieuCongTo.Model.DAO;

namespace ServiceTool
{
    public partial class ServiceToolForm : Form
    {
        ConfigClass conf;
        SanLuongManage slm;
        ChiSoChotManage cscm;
        ThongSoVanHanhManage tsvhm;
        static FileSystemWatcher fileWatcher;
        public ServiceToolForm()
        {
            InitializeComponent();
        }

        private void BtCauHinh_Click(object sender, EventArgs e)
        {
            CauHinhTool cht = new CauHinhTool(conf);
            cht.ShowDialog();

            conf = cht.updateConfigClass();
        }
        bool CheckExit = false;
        private void BtCancel_Click(object sender, EventArgs e)
        {
            CheckExit = true;
            DialogResult rs = MessageBox.Show("Turn Off Service ?", "Notification !!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rs == DialogResult.No)
            {
                CheckExit = false;
                return;
            }
            else
            {
                Application.Exit();
            }
            notifyIcon.Dispose();

        }

        private void BtRun_Click(object sender, EventArgs e)
        {
            ServiceRun();
        }

        // thu nho 
        private void SmallScreen()
        {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            notifyIcon.Visible = true;
        }
        // phong to
        private void FullScreen()
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            notifyIcon.Visible = true;
        }

        private void OpenFullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullScreen();
        }
        private void ServiceRun()
        {
            try
            {
                // Declares the FileSystemWatcher object San Luong
                fileWatcher = new FileSystemWatcher
                {
                    EnableRaisingEvents = false,
                    // We have to specify the path which has to monitor

                    Path = conf.ThuMucQuet,

                    // This property specifies which are the events to be monitored
                    NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                    Filter = "*.csv*" // Only watch csv files.
                };

                // Add event handlers for specific change events...

                //watcher.Changed += new FileSystemEventHandler(OnChanged);
                fileWatcher.Created += new FileSystemEventHandler(OnCreated);
                fileWatcher.Deleted += new FileSystemEventHandler(OnDelete);
                fileWatcher.Renamed += new RenamedEventHandler(OnRenamed);
                // Begin watching.
                fileWatcher.EnableRaisingEvents = true;
                slm = new SanLuongManage(notifyIcon, conf);
                cscm = new ChiSoChotManage(notifyIcon, conf);
                tsvhm = new ThongSoVanHanhManage(notifyIcon, conf);

                btRun.Enabled = false;
                btStop.Enabled = true;
                contextMenuStrip1.Items[1].Visible = true;
                contextMenuStrip1.Items[2].Visible = false;
                btConfig.Enabled = false;
                btCancel.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đường dẫn không tìm thấy !! ");
            }
        }
        /*private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            string checkfile = checkFileName(e.Name.Split('.')[0]);
            switch (checkfile)
            {
                case "sl":
                    {
                        slm.OnChangedSanLuong(source, e);
                        break;
                    }
                case "csc":
                    {
                        cscm.OnChangedCSC(source, e);
                        break;
                    }
                case "tsvh":
                    {
                        tsvhm.OnChangedTSVH(source, e);
                        break;
                    }
                default:
                    {
                        notifyIcon.ShowBalloonTip(100, "Lỗi file", checkfile, ToolTipIcon.Warning);
                        break;
                    }
            }
        }*/

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            string checkfile = checkFileName(e.Name.Split('.')[0]);
            switch (checkfile)
            {
                case "sl":
                    {
                        slm.OnRenamedSanLuong(source, e);
                        break;
                    }
                case "csc":
                    {
                        cscm.OnRenamedCSC(source, e);
                        break;
                    }
                case "tsvh":
                    {
                        tsvhm.OnRenamedTSVH(source, e);
                        break;
                    }
                default:
                    {
                        notifyIcon.ShowBalloonTip(100, "Lỗi file", checkfile, ToolTipIcon.Warning);
                        break;
                    }
            }
        }

        private void OnDelete(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            string checkfile = checkFileName(e.Name.Split('.')[0]);
            switch (checkfile)
            {
                case "sl":
                    {
                        slm.OnDeleteSanLuong(source, e);
                        break;
                    }
                case "csc":
                    {
                        cscm.OnDeleteCSC(source, e);
                        break;
                    }
                case "tsvh":
                    {
                        tsvhm.OnDeletedTSVH(source, e);
                        break;
                    }
                default:
                    {
                        notifyIcon.ShowBalloonTip(100, "Lỗi file", checkfile, ToolTipIcon.Warning);
                        break;
                    }
            }
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            string checkfile = checkFileName(e.Name.Split('.')[0]);
            switch (checkfile)
            {
                case "sl":
                    {
                        slm.OnCreatedSanLuong(source, e);
                        break;
                    }
                case "csc":
                    {
                        cscm.OnCreatedCSC(source, e);
                        break;
                    }
                case "tsvh":
                    {
                        tsvhm.OnCreatedTSVH(source, e);
                        break;
                    }
                default:
                    {
                        notifyIcon.ShowBalloonTip(100, "Lỗi file", checkfile, ToolTipIcon.Warning);
                        break;
                    }
            }
        }

        // stop service
        private void ServiceStop()
        {
            btRun.Enabled = true;
            btStop.Enabled = false;
            fileWatcher.EnableRaisingEvents = false;
            contextMenuStrip1.Items[1].Visible = false;
            contextMenuStrip1.Items[2].Visible = true;
            notifyIcon.ShowBalloonTip(100, "Giam Sat Thu Muc", "Is Stopped !!!! ", ToolTipIcon.Warning);
            btConfig.Enabled = true;
            btCancel.Enabled = true;
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            ServiceStop();
        }

        private void ServiceToolForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckExit)
            {
                e.Cancel = true;
                SmallScreen();
            }
        }

        private string checkFileName(string fileName)
        {
            String rx_sl = "[^0-9]";
            String rx_csc = "[0-9]+(_ReadHistoricalRegister_)[0-9]{8}_[0-9]{6}";
            String rx_tsvh = "[0-9]+(_ReadCurrentRegisterValues_)[0-9]{8}_[0-9]{6}";
            if (!Regex.IsMatch(fileName, rx_sl))
            {
                return "sl";
            }
            if (Regex.IsMatch(fileName, rx_csc))
            {
                return "csc";
            }
            /*if (fileName.Contains("ReadHistoricalRegister"))
            {
                string[] split = fileName.Split('_');
                if(!Regex.IsMatch(split[0], rx_sl) && Regex.IsMatch(split[2], "[0-9]{8}") && Regex.IsMatch(split[3], "[0-9]{6}"))
                {
                    return "csc";
                }
            }*/
            if (Regex.IsMatch(fileName, rx_tsvh))
            {
                return "tsvh";
            }
            /*if (fileName.Contains("ReadCurrentRegisterValues"))
            {
                string[] split = fileName.Split('_');
                if (!Regex.IsMatch(split[0], rx_sl) && Regex.IsMatch(split[2], "[0-9]{8}") && Regex.IsMatch(split[3], "[0-9]{6}"))
                {
                    return "tsvh";
                }
            }*/
            return "File không đúng định dạng";
        }

        private void ServiceToolForm_Load(object sender, EventArgs e)
        {
            try
            {
                conf = ConfigClass.GetConfig();
                if (conf.AutoRun)
                {
                    ServiceRun();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
