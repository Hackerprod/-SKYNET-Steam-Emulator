using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SKYNET.GUI;
using TsudaKageyu;

namespace SKYNET
{
    public partial class frmGameManager : frmBase
    {
        private int boxHandle;
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

            LoadLogo(filePath);

            if (File.Exists(Path.Combine(PathDirectory, "steam_api.dll")))
            {
                TB_SteamApiPath.Text = Path.Combine(PathDirectory, "steam_api.dll");
            }
            else if (File.Exists(Path.Combine(PathDirectory, "steam_api64.dll")))
            {
                TB_SteamApiPath.Text = Path.Combine(PathDirectory, "steam_api64.dll");
            }
            else
            {
                var files = Directory.GetFiles(PathDirectory, "*.dll", SearchOption.AllDirectories).ToList();

                string x86 = files.Find(x => x.Contains("steam_api.dll"));
                if (!string.IsNullOrEmpty(x86))
                {
                    TB_SteamApiPath.Text = x86;
                }

                string x64 = files.Find(x => x.Contains("steam_api64.dll"));
                if (!string.IsNullOrEmpty(x64))
                {
                    TB_SteamApiPath.Text = x64;
                }
            }

            TB_AppId.Text = "0";
            if (File.Exists(Path.Combine(PathDirectory, "steam_appid.txt")))
            {
                TB_AppId.Text = File.ReadAllText(Path.Combine(PathDirectory, "steam_appid.txt"));
            }
        }

        public frmGameManager(GameBox game)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            SetMouseMove(PN_Top);

            var Game = game.GetGame();
            TB_Name.Text = Game.Name;
            LB_Name.Text = Game.Name;
            TB_ExecutablePath.Text = Game.ExecutablePath;
            TB_SteamApiPath.Text = Game.SteamApiPath;
            TB_Parameters.Text = Game.Parameters;
            TB_AppId.Text = Game.AppId.ToString();
            CHB_WithoutEmu.Checked = Game.LaunchWithoutEmu;
            LoadLogo(Game.ExecutablePath);
            BT_AddGame.Text = "Update";

            boxHandle = game.Handle.ToInt32();
        }


        private void LoadLogo(string filePath)
        {
            PB_Avatar.Image = (Bitmap)IconFromFile(filePath);
        }

        public static Image IconFromFile(string filePath)
        {
            Image image = null;

            try
            {
                var extractor = new IconExtractor(filePath);
                var icon = extractor.GetIcon(0);

                Icon[] splitIcons = IconUtil.Split(icon);

                Icon selectedIcon = null;

                foreach (var item in splitIcons)
                {
                    if (selectedIcon == null)
                    {
                        selectedIcon = item;
                    }
                    else
                    {
                        if (IconUtil.GetBitCount(item) > IconUtil.GetBitCount(selectedIcon))
                        {
                            selectedIcon = item;
                        }
                        else if (IconUtil.GetBitCount(item) == IconUtil.GetBitCount(selectedIcon) && item.Width > selectedIcon.Width)
                        {
                            selectedIcon = item;
                        }
                    }
                }
                return selectedIcon.ToBitmap();
            }
            catch (Exception)
            {

            }

            try
            {
                image = Icon.ExtractAssociatedIcon(filePath)?.ToBitmap();
            }
            catch
            {
                image = new Icon(SystemIcons.Application, 256, 256).ToBitmap();
            }

            return image;
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

            var game = new Types.Game()
            {
                Name = TB_Name.Text,
                ExecutablePath = TB_ExecutablePath.Text,
                SteamApiPath = TB_SteamApiPath.Text,
                Parameters = TB_Parameters.Text,
                AppId = _AppId,
                LaunchWithoutEmu = CHB_WithoutEmu.Checked
            };

            if (boxHandle != 0)
            {
                frmMain.frm.UpdateGame(boxHandle, game);
                Close();
            }
            else
            {
                frmMain.frm.AddGame(game);
                Close();
            }
        }
        
        private void TB_Name_KeyUp(object sender, KeyEventArgs e)
        {
            LB_Name.Text = TB_Name.Text;
        }
    }
}
