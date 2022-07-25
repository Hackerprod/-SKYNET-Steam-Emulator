using SKYNET.GUI.Controls;
using SKYNET.Helpers;
using SKYNET.Managers;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace SKYNET.GUI
{
    public partial class frmGameDownload : frmBase
    {
        private uint AppId;
        private WebClient WebClient;
        private int CurrentDownloadID;

        public frmGameDownload(GameBox Box)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            CurrentDownloadID = Common.GetRandom();

            foreach (Control control in Controls)
            {
                base.SetMouseMove(control);
            }

            WebClient = new WebClient();
            AppId = Box.Game.AppID;
            LB_Name.Text = Box.Game.Name;

            try
            {
                Bitmap Avatar = (Bitmap)ImageHelper.ImageFromBase64(Box.Game.AvatarHex);
                PB_Avatar.Image = Avatar;
            }
            catch
            {
            }
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Images", "AppCache"));

            Thread DownloadThread = new Thread(StartDownloading);
            DownloadThread.IsBackground = true;
            DownloadThread.Start();
        }

        private async void StartDownloading()
        {
            string errorTask = "";
            string appCachePath = Path.Combine(Common.GetPath(), "Data", "Images", "AppCache", AppId.ToString());
            Common.EnsureDirectoryExists(appCachePath);

            try
            {
                string Url = $"https://steamcdn-a.akamaihd.net/steam/apps/{AppId}/library_hero.jpg";
                SetProgress(0, $"Downloading Library_Hero file for AppId {AppId}");
                var Data = await WebClient.DownloadDataTaskAsync(Url);

                File.WriteAllBytes(Path.Combine(appCachePath, $"{AppId}_library_hero.jpg"), Data);
            }
            catch (Exception ex)
            {
                errorTask = "Error downloading file. " + ex.Message;
            }

            try
            {
                string Url = $"https://steamcdn-a.akamaihd.net/steam/apps/{AppId}/header.jpg";
                SetProgress(25, $"Downloading Header file for AppId {AppId}");
                var Data = await WebClient.DownloadDataTaskAsync(Url);

                File.WriteAllBytes(Path.Combine(appCachePath, $"{AppId}_header.jpg"), Data);
            }
            catch (Exception ex)
            {
                errorTask = "Error downloading file. " + ex.Message;
            }

            try
            {
                SetProgress(50, $"Downloading Achievements for AppId {AppId}");
                await StatsManager.GenerateAchievements(AppId);
            }
            catch (Exception ex)
            {
                errorTask = "Error downloading file. " + ex.Message;
            }

            try
            {
                SetProgress(75, $"Downloading AppDetails for AppId {AppId}");
                await StatsManager.GenerateAppDetails(AppId);
            }
            catch (Exception ex)
            {
                errorTask = "Error downloading file. " + ex.Message;
            }

            try
            {
                SetProgress(100, $"Downloading Items for AppId {AppId}"); 
                await StatsManager.GenerateItems(AppId);
            }
            catch (Exception ex)
            {
                errorTask = "Error downloading file. " + ex.Message;
            }

            WebManager.SendDownloadProcessCompleted(CurrentDownloadID);
            Close();

            if (!string.IsNullOrEmpty(errorTask))
            {
                LB_Info.Text = $"Finished with errors.";
                Common.Show(errorTask);
            }
        }

        private void SetProgress(int value, string info)
        {
            WebManager.SendDownloadProcess(CurrentDownloadID, value, info);

            Common.InvokeAction(LB_Info, delegate
            {
                LB_Info.Text = info;
            });

            Common.InvokeAction(PB_Progress, delegate
            {
                PB_Progress.Value = value;
            });
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            WebClient.CancelAsync();
            Close();
        }
    }
}
