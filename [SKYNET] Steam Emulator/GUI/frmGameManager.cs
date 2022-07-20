using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Types;

namespace SKYNET.GUI
{
    public partial class frmGameManager : frmBase
    {
        private Game Game;
        public frmGameManager(string filePath)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            foreach (Control control in Controls)
            {
                base.SetMouseMove(control);
            }

            string FileName = Path.GetFileNameWithoutExtension(filePath);
            var PathDirectory = Directory.GetParent(filePath).ToString();

            TB_Name.Text = FileName;
            LB_Name.Text = FileName;
            TB_ExecutablePath.Text = filePath;

            var bitmap = (Bitmap)ImageHelper.IconFromFile(filePath);
            PB_Avatar.Image = bitmap;

            TB_AppId.Text = "0";
            if (File.Exists(Path.Combine(PathDirectory, "steam_appid.txt")))
            {
                TB_AppId.Text = File.ReadAllText(Path.Combine(PathDirectory, "steam_appid.txt"));
            }

            // Dota2 fix and customization
            if (FileName.ToLower() == "dota2")
            {
                TB_Name.Text = "Dota 2";
                LB_Name.Text = "Dota 2";
                TB_Parameters.Text = "-console -novid";
                CH_RunCallbacks.Checked = false;
                CH_ISteamHTTP.Checked = false;
                BT_AddGame.Focus();
            }
        }

        public frmGameManager(Game game)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            SetMouseMove(PN_Top);

            Game = game;
            TB_Name.Text = Game.Name;
            LB_Name.Text = Game.Name;
            TB_ExecutablePath.Text = Game.ExecutablePath;
            TB_Parameters.Text = Game.Parameters;
            TB_AppId.Text = Game.AppID.ToString();
            CH_WithoutEmu.Checked = Game.LaunchWithoutEmu;
            CH_LogToFile.Checked = Game.LogToFile;
            CH_LogToConsole.Checked = Game.LogToConsole;
            CH_RunCallbacks.Checked = Game.RunCallbacks;
            CH_ISteamHTTP.Checked = Game.ISteamHTTP;
            CH_CSteamworks.Checked = Game.CSteamworks;
            BT_AddGame.Text = "Update";

            try
            {
                var imageBytes = Convert.FromBase64String(Game.AvatarHex);
                Bitmap Avatar = (Bitmap)ImageHelper.ImageFromBytes(imageBytes);
                PB_Avatar.Image = Avatar; 
            }
            catch (Exception)
            {
                if (File.Exists(Game.ExecutablePath))
                {
                    var bitmap = (Bitmap)ImageHelper.IconFromFile(Game.ExecutablePath);
                    var AvatarHex = ImageHelper.GetImageBase64(bitmap);
                    Game.AvatarHex = AvatarHex;
                    PB_Avatar.Image = bitmap;
                }
            }
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            int attrValue = 2;
            DwmApi.DwmSetWindowAttribute(base.Handle, 2, ref attrValue, 16);
            DwmApi.MARGINS mARGINS = default(DwmApi.MARGINS);
            mARGINS.cyBottomHeight = 1;
            mARGINS.cxLeftWidth = 0;
            mARGINS.cxRightWidth = 0;
            mARGINS.cyTopHeight = 0;
            DwmApi.MARGINS marInset = mARGINS;
            DwmApi.DwmExtendFrameIntoClientArea(base.Handle, ref marInset);
        }

        private void AddGame_Click(object sender, EventArgs e)
        {
            if (!uint.TryParse(TB_AppId.Text, out uint _AppId))
            {
                modCommon.Show("Please set a valid AppId to continue");
                return;
            }

            if (Game == null)
            {
                string AvatarHex = "";
                try
                {
                    var bitmap = (Bitmap)ImageHelper.IconFromFile(TB_ExecutablePath.Text);
                    AvatarHex = ImageHelper.GetImageBase64(bitmap);
                }
                catch  { }

                Game = new Game()
                {
                    Guid = Guid.NewGuid().ToString(),
                    Name = TB_Name.Text,
                    ExecutablePath = TB_ExecutablePath.Text,
                    Parameters = TB_Parameters.Text,
                    AppID = _AppId,
                    LaunchWithoutEmu = CH_WithoutEmu.Checked,
                    GameOverlay = CH_GameOverlay.Checked,
                    LogToFile = CH_LogToFile.Checked,
                    LogToConsole = CH_LogToConsole.Checked,
                    RunCallbacks = CH_RunCallbacks.Checked,
                    ISteamHTTP = CH_ISteamHTTP.Checked,
                    CSteamworks = CH_CSteamworks.Checked,
                    AvatarHex = AvatarHex
                };
                try
                {
                    string appid_Path = Path.Combine(Path.GetDirectoryName(TB_ExecutablePath.Text), "steam_appid.txt");
                    File.WriteAllText(appid_Path, TB_AppId.Text);
                }
                catch { }
                GameManager.AddGame(Game);
            }
            else
            {
                Game.Name = TB_Name.Text;
                Game.ExecutablePath = TB_ExecutablePath.Text;
                Game.Parameters = TB_Parameters.Text;
                Game.AppID = _AppId;
                Game.LaunchWithoutEmu = CH_WithoutEmu.Checked;
                Game.GameOverlay = CH_GameOverlay.Checked;
                Game.LogToFile = CH_LogToFile.Checked;
                Game.LogToConsole = CH_LogToConsole.Checked;
                Game.RunCallbacks = CH_RunCallbacks.Checked;
                Game.ISteamHTTP = CH_ISteamHTTP.Checked;
                Game.CSteamworks = CH_CSteamworks.Checked;

                try
                {
                    string appid_Path = Path.Combine(Path.GetDirectoryName(TB_ExecutablePath.Text), "steam_appid.txt");
                    File.WriteAllText(appid_Path, TB_AppId.Text);
                }
                catch { }
                GameManager.Update(Game);
            }
            Close();
        }

        private void TB_Name_KeyUp(object sender, KeyEventArgs e)
        {
            LB_Name.Text = TB_Name.Text;
        }

        private void PB_Avatar_Click(object sender, EventArgs e)
        {
            if (File.Exists(Game.ExecutablePath))
            {
                var bitmap = (Bitmap)ImageHelper.IconFromFile(Game.ExecutablePath);
                var AvatarHex = ImageHelper.GetImageBase64(bitmap);
                Game.AvatarHex = AvatarHex;
                PB_Avatar.Image = bitmap;
            }
        }
    }
}
