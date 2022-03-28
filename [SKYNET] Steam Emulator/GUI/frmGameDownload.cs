using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET.GUI
{
    public partial class frmGameDownload : frmBase
    {
        uint AppId;
        WebClient WebClient;
        string CurrentFile;

        public frmGameDownload(GameBox Box)
        {
            InitializeComponent();

            WebClient = new WebClient();
            PB_Avatar.Image = Box.Image;
            AppId = Box.AppId;
            LB_Name.Text = Box.GetGame().Name;

            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Images", "Banner"));

            StartDownloading();
        }

        private async void StartDownloading()
        {
            string BannerPath = Path.Combine(modCommon.GetPath(), "Data", "Images", "Banner");

            WebClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;
            WebClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;

            try
            {
                string Url = $"https://steamcdn-a.akamaihd.net/steam/apps/{AppId}/library_hero.jpg";
                LB_Info.Text = $"Downloading Library_Hero file for AppId {AppId}";
                var Data = await WebClient.DownloadDataTaskAsync(Url);

                File.WriteAllBytes(Path.Combine(BannerPath, $"{AppId}_library_hero.jpg"), Data);
            }
            catch (Exception ex)
            {
                modCommon.Show("Error downloading file. " + ex.Message);
            }

            try
            {
                string Url = $"https://steamcdn-a.akamaihd.net/steam/apps/{AppId}/header.jpg";
                LB_Info.Text = $"Downloading Header file for AppId {AppId}";
                var Data = await WebClient.DownloadDataTaskAsync(Url);

                File.WriteAllBytes(Path.Combine(BannerPath, $"{AppId}_header.jpg"), Data);
                Close();
            }
            catch (Exception ex)
            {
                modCommon.Show("Error downloading file. " + ex.Message);
            }

        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            PB_Progress.Value = e.ProgressPercentage;
            if (e.ProgressPercentage == 100)
            {
                PB_Progress.Value = 0;
            }
        }

        private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            //e.Result
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            WebClient.CancelAsync();
            Close();
        }
    }
}
