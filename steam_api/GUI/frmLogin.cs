using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET.GUI
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();

            TB_UserName.Text = Environment.UserName;
            TB_SteamId.Text = modCommon.GenerateSteamID();
            TB_Languaje.Text = "English";
        }

        private void CloseBox_MouseClick(object sender, MouseEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void BT_Launch_Click(object sender, EventArgs e)
        {
            string _file = Path.Combine(modCommon.GetPath(), "[SKYNET] steam_api.ini");

            StringBuilder config = new StringBuilder();

            // User Configuration

            config.AppendLine("[STEAM USER]");
            config.AppendLine($"Nickname = {TB_UserName.Text}");
            config.AppendLine($"SteamID = {TB_SteamId.Text}");
            config.AppendLine($"Languaje = {TB_Languaje.Text}");
            config.AppendLine();

            // Network Configuration

            config.AppendLine("[NETWORK]");
            config.AppendLine("# When the emulator is in LAN mode (without dedicated server) it sends and receives data through broadcast ");
            config.AppendLine("ServerIP = 127.0.0.1");
            config.AppendLine("BroadCastPort = 28025");
            config.AppendLine();

            // Log Configuration

            config.AppendLine("[LOG]");
            config.AppendLine("Console = false");
            config.AppendLine("File = false");
            config.AppendLine();

            File.WriteAllText(_file, config.ToString());
            Close();
        }
    }
}
