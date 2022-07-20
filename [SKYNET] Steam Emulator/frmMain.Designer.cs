using SKYNET.GUI.Controls;
using System.Windows.Forms;

namespace SKYNET.GUI
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.PN_Top = new System.Windows.Forms.Panel();
            this.BT_Minimize = new SKYNET.GUI.Controls.SKYNET_MinimizeBox();
            this.BT_Close = new SKYNET.GUI.Controls.SKYNET_CloseBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.LB_Browser = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.LB_Status = new System.Windows.Forms.Label();
            this.LB_Clear = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.LB_Console = new System.Windows.Forms.Label();
            this.LB_Profile = new System.Windows.Forms.Label();
            this.LB_Community = new System.Windows.Forms.Label();
            this.LB_Library = new System.Windows.Forms.Label();
            this.PN_LeftContainer = new System.Windows.Forms.Panel();
            this.PN_GameContainer = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.LB_Add = new System.Windows.Forms.Label();
            this.PB_Add = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TB_Search = new SKYNET.GUI.Controls.SKYNET_TextBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.BT_Profile = new System.Windows.Forms.PictureBox();
            this.PB_Avatar = new SKYNET.GUI.Controls.SKYNET_CircularPictureBox();
            this.LB_SteamID = new System.Windows.Forms.Label();
            this.LB_NickName = new System.Windows.Forms.Label();
            this.gradiantBox1 = new SKYNET.GUI.Controls.SKYNET_GradiantBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.TabControl1 = new SKYNET.Controls.SKYNET_TabControl();
            this.TP_Library = new System.Windows.Forms.TabPage();
            this.LB_ShortDescription = new System.Windows.Forms.Label();
            this.PB_GameInfo = new System.Windows.Forms.PictureBox();
            this.PN_RContainer = new System.Windows.Forms.Panel();
            this.PN_UserContainer = new System.Windows.Forms.Panel();
            this.LB_UsersOnline = new System.Windows.Forms.Label();
            this.PB_Banner = new System.Windows.Forms.PictureBox();
            this.PB_Logo = new System.Windows.Forms.PictureBox();
            this.LB_GameTittle = new SKYNET.GUI.Controls.SKYNET_Label();
            this.ShadowBox = new System.Windows.Forms.Panel();
            this.LB_PlayingNowInfo = new System.Windows.Forms.Label();
            this.LB_PlayingNow = new System.Windows.Forms.Label();
            this.LB_PlayedTime = new System.Windows.Forms.Label();
            this.LB_LastPlayedInfo = new System.Windows.Forms.Label();
            this.LB_LastPlayed = new System.Windows.Forms.Label();
            this.LB_PlayedTimeInfo = new System.Windows.Forms.Label();
            this.BT_GameAction = new SKYNET.GUI.Controls.SKYNET_Button();
            this.TP_Community = new System.Windows.Forms.TabPage();
            this.TP_Profile = new System.Windows.Forms.TabPage();
            this.TP_Console = new System.Windows.Forms.TabPage();
            this.WebLogger1 = new SKYNET.GUI.Controls.SKYNET_WebLogger();
            this.PN_BodyContainer = new System.Windows.Forms.Panel();
            this.CM_MenuGame = new SKYNET.GUI.Controls.SKYNET_ContextMenuStrip();
            this.OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenWithoutEmuMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFileLocationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToTopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToButtomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GameCacheMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConfigureMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PN_Top.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.PN_LeftContainer.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Add)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BT_Profile)).BeginInit();
            this.gradiantBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.TabControl1.SuspendLayout();
            this.TP_Library.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_GameInfo)).BeginInit();
            this.PN_RContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Banner)).BeginInit();
            this.PB_Banner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Logo)).BeginInit();
            this.ShadowBox.SuspendLayout();
            this.TP_Console.SuspendLayout();
            this.PN_BodyContainer.SuspendLayout();
            this.CM_MenuGame.SuspendLayout();
            this.SuspendLayout();
            // 
            // PN_Top
            // 
            this.PN_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.PN_Top.Controls.Add(this.BT_Minimize);
            this.PN_Top.Controls.Add(this.BT_Close);
            this.PN_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.PN_Top.Location = new System.Drawing.Point(249, 0);
            this.PN_Top.Name = "PN_Top";
            this.PN_Top.Size = new System.Drawing.Size(893, 26);
            this.PN_Top.TabIndex = 0;
            // 
            // BT_Minimize
            // 
            this.BT_Minimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.BT_Minimize.Color = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.BT_Minimize.Dock = System.Windows.Forms.DockStyle.Right;
            this.BT_Minimize.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(53)))));
            this.BT_Minimize.Location = new System.Drawing.Point(825, 0);
            this.BT_Minimize.MaximumSize = new System.Drawing.Size(34, 26);
            this.BT_Minimize.MinimumSize = new System.Drawing.Size(34, 26);
            this.BT_Minimize.Name = "BT_Minimize";
            this.BT_Minimize.Size = new System.Drawing.Size(34, 26);
            this.BT_Minimize.TabIndex = 1;
            this.BT_Minimize.Clicked += new System.EventHandler(this.Minimize_Clicked);
            // 
            // BT_Close
            // 
            this.BT_Close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.BT_Close.Color = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.BT_Close.Dock = System.Windows.Forms.DockStyle.Right;
            this.BT_Close.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(53)))));
            this.BT_Close.Location = new System.Drawing.Point(859, 0);
            this.BT_Close.MaximumSize = new System.Drawing.Size(34, 26);
            this.BT_Close.MinimumSize = new System.Drawing.Size(34, 26);
            this.BT_Close.Name = "BT_Close";
            this.BT_Close.Size = new System.Drawing.Size(34, 26);
            this.BT_Close.TabIndex = 0;
            this.BT_Close.Clicked += new System.EventHandler(this.Close_Clicked);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.panel3.Controls.Add(this.LB_Browser);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.LB_Status);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(249, 565);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(893, 44);
            this.panel3.TabIndex = 7;
            // 
            // LB_Browser
            // 
            this.LB_Browser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.LB_Browser.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_Browser.Font = new System.Drawing.Font("Segoe UI Emoji", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Browser.ForeColor = System.Drawing.Color.White;
            this.LB_Browser.Location = new System.Drawing.Point(823, 23);
            this.LB_Browser.Name = "LB_Browser";
            this.LB_Browser.Size = new System.Drawing.Size(60, 15);
            this.LB_Browser.TabIndex = 11;
            this.LB_Browser.Text = "BROWSER";
            this.LB_Browser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LB_Browser.Visible = false;
            this.LB_Browser.Click += new System.EventHandler(this.LB_Browser_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.label6.Font = new System.Drawing.Font("Segoe UI Emoji", 8F);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(3, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(812, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "Connected to 10.31.0.1";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LB_Status
            // 
            this.LB_Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.LB_Status.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Status.ForeColor = System.Drawing.Color.White;
            this.LB_Status.Location = new System.Drawing.Point(3, 5);
            this.LB_Status.Name = "LB_Status";
            this.LB_Status.Size = new System.Drawing.Size(812, 15);
            this.LB_Status.TabIndex = 10;
            this.LB_Status.Text = "ONLINE";
            this.LB_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LB_Clear
            // 
            this.LB_Clear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.LB_Clear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_Clear.Font = new System.Drawing.Font("Segoe UI Emoji", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Clear.ForeColor = System.Drawing.Color.White;
            this.LB_Clear.Location = new System.Drawing.Point(803, 11);
            this.LB_Clear.Name = "LB_Clear";
            this.LB_Clear.Size = new System.Drawing.Size(60, 19);
            this.LB_Clear.TabIndex = 13;
            this.LB_Clear.Text = "CLEAR";
            this.LB_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LB_Clear.Click += new System.EventHandler(this.LB_Clear_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.panel4.Controls.Add(this.LB_Console);
            this.panel4.Controls.Add(this.LB_Profile);
            this.panel4.Controls.Add(this.LB_Community);
            this.panel4.Controls.Add(this.LB_Library);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(249, 26);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(893, 34);
            this.panel4.TabIndex = 8;
            // 
            // LB_Console
            // 
            this.LB_Console.AutoSize = true;
            this.LB_Console.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_Console.Font = new System.Drawing.Font("Segoe UI Emoji", 12.25F, System.Drawing.FontStyle.Bold);
            this.LB_Console.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(208)))), ((int)(((byte)(218)))));
            this.LB_Console.Location = new System.Drawing.Point(399, 5);
            this.LB_Console.Name = "LB_Console";
            this.LB_Console.Size = new System.Drawing.Size(93, 22);
            this.LB_Console.TabIndex = 3;
            this.LB_Console.Text = "CONSOLE";
            this.LB_Console.Click += new System.EventHandler(this.LB_Console_Click);
            // 
            // LB_Profile
            // 
            this.LB_Profile.AutoSize = true;
            this.LB_Profile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_Profile.Font = new System.Drawing.Font("Segoe UI Emoji", 12.25F, System.Drawing.FontStyle.Bold);
            this.LB_Profile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(208)))), ((int)(((byte)(218)))));
            this.LB_Profile.Location = new System.Drawing.Point(253, 5);
            this.LB_Profile.Name = "LB_Profile";
            this.LB_Profile.Size = new System.Drawing.Size(128, 22);
            this.LB_Profile.TabIndex = 2;
            this.LB_Profile.Text = "HACKERPROD";
            this.LB_Profile.Click += new System.EventHandler(this.LB_Profile_Click);
            // 
            // LB_Community
            // 
            this.LB_Community.AutoSize = true;
            this.LB_Community.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_Community.Font = new System.Drawing.Font("Segoe UI Emoji", 12.25F, System.Drawing.FontStyle.Bold);
            this.LB_Community.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(208)))), ((int)(((byte)(218)))));
            this.LB_Community.Location = new System.Drawing.Point(111, 5);
            this.LB_Community.Name = "LB_Community";
            this.LB_Community.Size = new System.Drawing.Size(121, 22);
            this.LB_Community.TabIndex = 1;
            this.LB_Community.Text = "COMMUNITY";
            this.LB_Community.Click += new System.EventHandler(this.LB_Community_Click);
            // 
            // LB_Library
            // 
            this.LB_Library.AutoSize = true;
            this.LB_Library.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_Library.Font = new System.Drawing.Font("Segoe UI Emoji", 12.25F, System.Drawing.FontStyle.Bold);
            this.LB_Library.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(208)))), ((int)(((byte)(218)))));
            this.LB_Library.Location = new System.Drawing.Point(13, 5);
            this.LB_Library.Name = "LB_Library";
            this.LB_Library.Size = new System.Drawing.Size(80, 22);
            this.LB_Library.TabIndex = 0;
            this.LB_Library.Text = "LIBRARY";
            this.LB_Library.Click += new System.EventHandler(this.LB_Library_Click);
            // 
            // PN_LeftContainer
            // 
            this.PN_LeftContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(46)))), ((int)(((byte)(51)))));
            this.PN_LeftContainer.Controls.Add(this.PN_GameContainer);
            this.PN_LeftContainer.Controls.Add(this.panel8);
            this.PN_LeftContainer.Controls.Add(this.panel6);
            this.PN_LeftContainer.Controls.Add(this.panel1);
            this.PN_LeftContainer.Controls.Add(this.panel7);
            this.PN_LeftContainer.Controls.Add(this.gradiantBox1);
            this.PN_LeftContainer.Controls.Add(this.panel2);
            this.PN_LeftContainer.Controls.Add(this.panel5);
            this.PN_LeftContainer.Dock = System.Windows.Forms.DockStyle.Left;
            this.PN_LeftContainer.Location = new System.Drawing.Point(0, 0);
            this.PN_LeftContainer.Name = "PN_LeftContainer";
            this.PN_LeftContainer.Size = new System.Drawing.Size(249, 609);
            this.PN_LeftContainer.TabIndex = 6;
            // 
            // PN_GameContainer
            // 
            this.PN_GameContainer.AllowDrop = true;
            this.PN_GameContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.PN_GameContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PN_GameContainer.Location = new System.Drawing.Point(0, 149);
            this.PN_GameContainer.Name = "PN_GameContainer";
            this.PN_GameContainer.Size = new System.Drawing.Size(249, 416);
            this.PN_GameContainer.TabIndex = 19;
            this.PN_GameContainer.DragDrop += new System.Windows.Forms.DragEventHandler(this.PN_GameContainer_DragDrop);
            this.PN_GameContainer.DragEnter += new System.Windows.Forms.DragEventHandler(this.PN_GameContainer_DragEnter);
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 144);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(249, 5);
            this.panel8.TabIndex = 18;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.panel6.Controls.Add(this.LB_Add);
            this.panel6.Controls.Add(this.PB_Add);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 565);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(249, 44);
            this.panel6.TabIndex = 17;
            // 
            // LB_Add
            // 
            this.LB_Add.AutoSize = true;
            this.LB_Add.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F);
            this.LB_Add.ForeColor = System.Drawing.Color.White;
            this.LB_Add.Location = new System.Drawing.Point(50, 12);
            this.LB_Add.Name = "LB_Add";
            this.LB_Add.Size = new System.Drawing.Size(92, 15);
            this.LB_Add.TabIndex = 9;
            this.LB_Add.Text = "ADD NEW GAME";
            this.LB_Add.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AddGame_Clicked);
            this.LB_Add.MouseLeave += new System.EventHandler(this.Add_MouseLeave);
            this.LB_Add.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Add_MouseMove);
            // 
            // PB_Add
            // 
            this.PB_Add.Image = global::SKYNET.Properties.Resources.add;
            this.PB_Add.Location = new System.Drawing.Point(20, 5);
            this.PB_Add.Name = "PB_Add";
            this.PB_Add.Size = new System.Drawing.Size(28, 28);
            this.PB_Add.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Add.TabIndex = 9;
            this.PB_Add.TabStop = false;
            this.PB_Add.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AddGame_Clicked);
            this.PB_Add.MouseLeave += new System.EventHandler(this.Add_MouseLeave);
            this.PB_Add.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Add_MouseMove);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.panel1.Controls.Add(this.TB_Search);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 99);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(249, 45);
            this.panel1.TabIndex = 15;
            // 
            // TB_Search
            // 
            this.TB_Search.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.TB_Search.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.TB_Search.Color = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.TB_Search.ForeColor = System.Drawing.Color.White;
            this.TB_Search.IsPassword = false;
            this.TB_Search.Location = new System.Drawing.Point(13, 1);
            this.TB_Search.Logo = global::SKYNET.Properties.Resources.search;
            this.TB_Search.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_Search.Name = "TB_Search";
            this.TB_Search.OnlyNumbers = false;
            this.TB_Search.ShowLogo = true;
            this.TB_Search.Size = new System.Drawing.Size(225, 35);
            this.TB_Search.TabIndex = 9;
            this.TB_Search.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TB_Search.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TB_Search_KeyUp);
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.panel7.Controls.Add(this.BT_Profile);
            this.panel7.Controls.Add(this.PB_Avatar);
            this.panel7.Controls.Add(this.LB_SteamID);
            this.panel7.Controls.Add(this.LB_NickName);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(0, 40);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(249, 59);
            this.panel7.TabIndex = 14;
            // 
            // BT_Profile
            // 
            this.BT_Profile.BackColor = System.Drawing.Color.Transparent;
            this.BT_Profile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BT_Profile.Image = global::SKYNET.Properties.Resources.EditProfile;
            this.BT_Profile.Location = new System.Drawing.Point(218, 22);
            this.BT_Profile.Name = "BT_Profile";
            this.BT_Profile.Size = new System.Drawing.Size(22, 22);
            this.BT_Profile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.BT_Profile.TabIndex = 21;
            this.BT_Profile.TabStop = false;
            this.BT_Profile.Click += new System.EventHandler(this.BT_Profile_Click);
            // 
            // PB_Avatar
            // 
            this.PB_Avatar.Image = global::SKYNET.Properties.Resources.profile_picture;
            this.PB_Avatar.Location = new System.Drawing.Point(16, 3);
            this.PB_Avatar.Name = "PB_Avatar";
            this.PB_Avatar.Size = new System.Drawing.Size(50, 50);
            this.PB_Avatar.TabIndex = 19;
            // 
            // LB_SteamID
            // 
            this.LB_SteamID.AutoSize = true;
            this.LB_SteamID.Font = new System.Drawing.Font("Segoe UI Emoji", 8.5F);
            this.LB_SteamID.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.LB_SteamID.Location = new System.Drawing.Point(75, 25);
            this.LB_SteamID.Name = "LB_SteamID";
            this.LB_SteamID.Size = new System.Drawing.Size(110, 16);
            this.LB_SteamID.TabIndex = 16;
            this.LB_SteamID.Text = "76513692034573246";
            // 
            // LB_NickName
            // 
            this.LB_NickName.AutoSize = true;
            this.LB_NickName.Font = new System.Drawing.Font("Segoe UI Emoji", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_NickName.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.LB_NickName.Location = new System.Drawing.Point(73, 3);
            this.LB_NickName.Name = "LB_NickName";
            this.LB_NickName.Size = new System.Drawing.Size(101, 21);
            this.LB_NickName.TabIndex = 12;
            this.LB_NickName.Text = "Hackerprod";
            this.LB_NickName.Click += new System.EventHandler(this.LB_NickName_Click);
            // 
            // gradiantBox1
            // 
            this.gradiantBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.gradiantBox1.Controls.Add(this.label5);
            this.gradiantBox1.Controls.Add(this.label1);
            this.gradiantBox1.Controls.Add(this.pictureBox1);
            this.gradiantBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.gradiantBox1.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.gradiantBox1.Location = new System.Drawing.Point(0, 15);
            this.gradiantBox1.Mode = SKYNET.GUI.Controls.Mode.Vertical;
            this.gradiantBox1.Name = "gradiantBox1";
            this.gradiantBox1.RigthColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.gradiantBox1.Size = new System.Drawing.Size(249, 25);
            this.gradiantBox1.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(187, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "SKYNET";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(63, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 25);
            this.label1.TabIndex = 7;
            this.label1.Text = "STEAM EMU";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::SKYNET.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(27, 46);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 30);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(249, 10);
            this.panel2.TabIndex = 13;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(249, 5);
            this.panel5.TabIndex = 10;
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TP_Library);
            this.TabControl1.Controls.Add(this.TP_Community);
            this.TabControl1.Controls.Add(this.TP_Profile);
            this.TabControl1.Controls.Add(this.TP_Console);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.HideBorders = true;
            this.TabControl1.ItemSize = new System.Drawing.Size(43, 18);
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Multiline = true;
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(893, 505);
            this.TabControl1.TabIndex = 0;
            // 
            // TP_Library
            // 
            this.TP_Library.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(53)))));
            this.TP_Library.Controls.Add(this.LB_ShortDescription);
            this.TP_Library.Controls.Add(this.PB_GameInfo);
            this.TP_Library.Controls.Add(this.PN_RContainer);
            this.TP_Library.Controls.Add(this.PB_Banner);
            this.TP_Library.Location = new System.Drawing.Point(4, 22);
            this.TP_Library.Name = "TP_Library";
            this.TP_Library.Padding = new System.Windows.Forms.Padding(3);
            this.TP_Library.Size = new System.Drawing.Size(885, 479);
            this.TP_Library.TabIndex = 0;
            this.TP_Library.Text = "Library";
            // 
            // LB_ShortDescription
            // 
            this.LB_ShortDescription.Font = new System.Drawing.Font("Franklin Gothic Medium", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_ShortDescription.ForeColor = System.Drawing.Color.LightGray;
            this.LB_ShortDescription.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.LB_ShortDescription.Location = new System.Drawing.Point(132, 301);
            this.LB_ShortDescription.Name = "LB_ShortDescription";
            this.LB_ShortDescription.Size = new System.Drawing.Size(555, 80);
            this.LB_ShortDescription.TabIndex = 12;
            this.LB_ShortDescription.Visible = false;
            // 
            // PB_GameInfo
            // 
            this.PB_GameInfo.BackColor = System.Drawing.Color.Transparent;
            this.PB_GameInfo.Image = global::SKYNET.Properties.Resources.logo;
            this.PB_GameInfo.Location = new System.Drawing.Point(35, 299);
            this.PB_GameInfo.Name = "PB_GameInfo";
            this.PB_GameInfo.Size = new System.Drawing.Size(73, 67);
            this.PB_GameInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_GameInfo.TabIndex = 11;
            this.PB_GameInfo.TabStop = false;
            this.PB_GameInfo.Visible = false;
            // 
            // PN_RContainer
            // 
            this.PN_RContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(53)))));
            this.PN_RContainer.Controls.Add(this.PN_UserContainer);
            this.PN_RContainer.Controls.Add(this.LB_UsersOnline);
            this.PN_RContainer.Dock = System.Windows.Forms.DockStyle.Right;
            this.PN_RContainer.Location = new System.Drawing.Point(715, 272);
            this.PN_RContainer.Name = "PN_RContainer";
            this.PN_RContainer.Padding = new System.Windows.Forms.Padding(0, 5, 5, 0);
            this.PN_RContainer.Size = new System.Drawing.Size(167, 204);
            this.PN_RContainer.TabIndex = 10;
            // 
            // PN_UserContainer
            // 
            this.PN_UserContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PN_UserContainer.Location = new System.Drawing.Point(0, 24);
            this.PN_UserContainer.Name = "PN_UserContainer";
            this.PN_UserContainer.Size = new System.Drawing.Size(162, 180);
            this.PN_UserContainer.TabIndex = 11;
            // 
            // LB_UsersOnline
            // 
            this.LB_UsersOnline.Dock = System.Windows.Forms.DockStyle.Top;
            this.LB_UsersOnline.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_UsersOnline.ForeColor = System.Drawing.Color.White;
            this.LB_UsersOnline.Location = new System.Drawing.Point(0, 5);
            this.LB_UsersOnline.Name = "LB_UsersOnline";
            this.LB_UsersOnline.Size = new System.Drawing.Size(162, 19);
            this.LB_UsersOnline.TabIndex = 10;
            this.LB_UsersOnline.Text = "Users Online 0";
            this.LB_UsersOnline.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PB_Banner
            // 
            this.PB_Banner.Controls.Add(this.PB_Logo);
            this.PB_Banner.Controls.Add(this.LB_GameTittle);
            this.PB_Banner.Controls.Add(this.ShadowBox);
            this.PB_Banner.Dock = System.Windows.Forms.DockStyle.Top;
            this.PB_Banner.Image = global::SKYNET.Properties.Resources.Header_1;
            this.PB_Banner.Location = new System.Drawing.Point(3, 3);
            this.PB_Banner.Name = "PB_Banner";
            this.PB_Banner.Size = new System.Drawing.Size(879, 269);
            this.PB_Banner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PB_Banner.TabIndex = 9;
            this.PB_Banner.TabStop = false;
            // 
            // PB_Logo
            // 
            this.PB_Logo.BackColor = System.Drawing.Color.Transparent;
            this.PB_Logo.Image = global::SKYNET.Properties.Resources.logo;
            this.PB_Logo.Location = new System.Drawing.Point(83, 132);
            this.PB_Logo.Name = "PB_Logo";
            this.PB_Logo.Size = new System.Drawing.Size(73, 67);
            this.PB_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Logo.TabIndex = 10;
            this.PB_Logo.TabStop = false;
            // 
            // LB_GameTittle
            // 
            this.LB_GameTittle.BackColor = System.Drawing.Color.Transparent;
            this.LB_GameTittle.BorderColor = System.Drawing.Color.Black;
            this.LB_GameTittle.CreateBorder = true;
            this.LB_GameTittle.Font = new System.Drawing.Font("Sitka Text", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_GameTittle.ForeColor = System.Drawing.Color.White;
            this.LB_GameTittle.Location = new System.Drawing.Point(170, 142);
            this.LB_GameTittle.Name = "LB_GameTittle";
            this.LB_GameTittle.Size = new System.Drawing.Size(672, 75);
            this.LB_GameTittle.TabIndex = 11;
            this.LB_GameTittle.Text = "[SKYNET] Steam Emulator";
            // 
            // ShadowBox
            // 
            this.ShadowBox.BackColor = System.Drawing.Color.Transparent;
            this.ShadowBox.Controls.Add(this.LB_PlayingNowInfo);
            this.ShadowBox.Controls.Add(this.LB_PlayingNow);
            this.ShadowBox.Controls.Add(this.LB_PlayedTime);
            this.ShadowBox.Controls.Add(this.LB_LastPlayedInfo);
            this.ShadowBox.Controls.Add(this.LB_LastPlayed);
            this.ShadowBox.Controls.Add(this.LB_PlayedTimeInfo);
            this.ShadowBox.Controls.Add(this.BT_GameAction);
            this.ShadowBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ShadowBox.Location = new System.Drawing.Point(0, 219);
            this.ShadowBox.Name = "ShadowBox";
            this.ShadowBox.Size = new System.Drawing.Size(879, 50);
            this.ShadowBox.TabIndex = 10;
            // 
            // LB_PlayingNowInfo
            // 
            this.LB_PlayingNowInfo.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_PlayingNowInfo.ForeColor = System.Drawing.Color.White;
            this.LB_PlayingNowInfo.Location = new System.Drawing.Point(670, 12);
            this.LB_PlayingNowInfo.Name = "LB_PlayingNowInfo";
            this.LB_PlayingNowInfo.Size = new System.Drawing.Size(111, 13);
            this.LB_PlayingNowInfo.TabIndex = 6;
            this.LB_PlayingNowInfo.Text = "Playing now";
            this.LB_PlayingNowInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LB_PlayingNowInfo.Visible = false;
            // 
            // LB_PlayingNow
            // 
            this.LB_PlayingNow.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Bold);
            this.LB_PlayingNow.ForeColor = System.Drawing.Color.LightGray;
            this.LB_PlayingNow.Location = new System.Drawing.Point(670, 25);
            this.LB_PlayingNow.Name = "LB_PlayingNow";
            this.LB_PlayingNow.Size = new System.Drawing.Size(111, 17);
            this.LB_PlayingNow.TabIndex = 5;
            this.LB_PlayingNow.Text = "0 friends";
            this.LB_PlayingNow.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LB_PlayingNow.Visible = false;
            // 
            // LB_PlayedTime
            // 
            this.LB_PlayedTime.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Bold);
            this.LB_PlayedTime.ForeColor = System.Drawing.Color.LightGray;
            this.LB_PlayedTime.Location = new System.Drawing.Point(241, 25);
            this.LB_PlayedTime.Name = "LB_PlayedTime";
            this.LB_PlayedTime.Size = new System.Drawing.Size(111, 17);
            this.LB_PlayedTime.TabIndex = 4;
            this.LB_PlayedTime.Text = "3.8 Hours";
            this.LB_PlayedTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LB_PlayedTime.Visible = false;
            // 
            // LB_LastPlayedInfo
            // 
            this.LB_LastPlayedInfo.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_LastPlayedInfo.ForeColor = System.Drawing.Color.White;
            this.LB_LastPlayedInfo.Location = new System.Drawing.Point(457, 12);
            this.LB_LastPlayedInfo.Name = "LB_LastPlayedInfo";
            this.LB_LastPlayedInfo.Size = new System.Drawing.Size(111, 13);
            this.LB_LastPlayedInfo.TabIndex = 3;
            this.LB_LastPlayedInfo.Text = "Last played";
            this.LB_LastPlayedInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LB_LastPlayedInfo.Visible = false;
            // 
            // LB_LastPlayed
            // 
            this.LB_LastPlayed.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Bold);
            this.LB_LastPlayed.ForeColor = System.Drawing.Color.LightGray;
            this.LB_LastPlayed.Location = new System.Drawing.Point(457, 25);
            this.LB_LastPlayed.Name = "LB_LastPlayed";
            this.LB_LastPlayed.Size = new System.Drawing.Size(111, 17);
            this.LB_LastPlayed.TabIndex = 2;
            this.LB_LastPlayed.Text = "3 days ago";
            this.LB_LastPlayed.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LB_LastPlayed.Visible = false;
            // 
            // LB_PlayedTimeInfo
            // 
            this.LB_PlayedTimeInfo.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Bold);
            this.LB_PlayedTimeInfo.ForeColor = System.Drawing.Color.White;
            this.LB_PlayedTimeInfo.Location = new System.Drawing.Point(241, 12);
            this.LB_PlayedTimeInfo.Name = "LB_PlayedTimeInfo";
            this.LB_PlayedTimeInfo.Size = new System.Drawing.Size(111, 13);
            this.LB_PlayedTimeInfo.TabIndex = 1;
            this.LB_PlayedTimeInfo.Text = "Played time";
            this.LB_PlayedTimeInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LB_PlayedTimeInfo.Visible = false;
            // 
            // BT_GameAction
            // 
            this.BT_GameAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(186)))), ((int)(((byte)(65)))));
            this.BT_GameAction.BackColorMouseOver = System.Drawing.Color.Empty;
            this.BT_GameAction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BT_GameAction.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_GameAction.ForeColor = System.Drawing.Color.White;
            this.BT_GameAction.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.BT_GameAction.ImageAlignment = SKYNET.GUI.Controls.SKYNET_Button._ImgAlign.Left;
            this.BT_GameAction.ImageIcon = null;
            this.BT_GameAction.Location = new System.Drawing.Point(32, 11);
            this.BT_GameAction.MenuMode = false;
            this.BT_GameAction.Name = "BT_GameAction";
            this.BT_GameAction.Rounded = false;
            this.BT_GameAction.Size = new System.Drawing.Size(100, 32);
            this.BT_GameAction.Style = SKYNET.GUI.Controls.SKYNET_Button._Style.TextOnly;
            this.BT_GameAction.TabIndex = 0;
            this.BT_GameAction.Text = "PLAY";
            this.BT_GameAction.Visible = false;
            this.BT_GameAction.Click += new System.EventHandler(this.GameAction_Click);
            // 
            // TP_Community
            // 
            this.TP_Community.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(53)))));
            this.TP_Community.Location = new System.Drawing.Point(4, 22);
            this.TP_Community.Name = "TP_Community";
            this.TP_Community.Padding = new System.Windows.Forms.Padding(3);
            this.TP_Community.Size = new System.Drawing.Size(885, 479);
            this.TP_Community.TabIndex = 1;
            this.TP_Community.Text = "Community";
            // 
            // TP_Profile
            // 
            this.TP_Profile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(53)))));
            this.TP_Profile.Location = new System.Drawing.Point(4, 22);
            this.TP_Profile.Name = "TP_Profile";
            this.TP_Profile.Padding = new System.Windows.Forms.Padding(3);
            this.TP_Profile.Size = new System.Drawing.Size(885, 479);
            this.TP_Profile.TabIndex = 2;
            this.TP_Profile.Text = "Profile";
            // 
            // TP_Console
            // 
            this.TP_Console.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(53)))));
            this.TP_Console.Controls.Add(this.LB_Clear);
            this.TP_Console.Controls.Add(this.WebLogger1);
            this.TP_Console.Location = new System.Drawing.Point(4, 22);
            this.TP_Console.Name = "TP_Console";
            this.TP_Console.Padding = new System.Windows.Forms.Padding(3);
            this.TP_Console.Size = new System.Drawing.Size(885, 479);
            this.TP_Console.TabIndex = 3;
            this.TP_Console.Text = "Console";
            // 
            // WebLogger1
            // 
            this.WebLogger1.AutoScrollLines = true;
            this.WebLogger1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(53)))));
            this.WebLogger1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebLogger1.Location = new System.Drawing.Point(3, 3);
            this.WebLogger1.LoggerBackColor = System.Drawing.Color.Empty;
            this.WebLogger1.Name = "WebLogger1";
            this.WebLogger1.ScrollColors = System.Drawing.Color.Empty;
            this.WebLogger1.Size = new System.Drawing.Size(879, 473);
            this.WebLogger1.TabIndex = 2;
            // 
            // PN_BodyContainer
            // 
            this.PN_BodyContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(50)))), ((int)(((byte)(57)))));
            this.PN_BodyContainer.Controls.Add(this.TabControl1);
            this.PN_BodyContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PN_BodyContainer.Location = new System.Drawing.Point(249, 60);
            this.PN_BodyContainer.Name = "PN_BodyContainer";
            this.PN_BodyContainer.Size = new System.Drawing.Size(893, 505);
            this.PN_BodyContainer.TabIndex = 10;
            // 
            // CM_MenuGame
            // 
            this.CM_MenuGame.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.CM_MenuGame.ForeColor = System.Drawing.Color.White;
            this.CM_MenuGame.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenMenuItem,
            this.OpenWithoutEmuMenuItem,
            this.OpenFileLocationMenuItem,
            this.RemoveMenuItem,
            this.ToTopMenuItem,
            this.ToButtomMenuItem,
            this.GameCacheMenuItem,
            this.ConfigureMenuItem});
            this.CM_MenuGame.Name = "CM_MenuGame";
            this.CM_MenuGame.ShowImageMargin = false;
            this.CM_MenuGame.Size = new System.Drawing.Size(193, 180);
            // 
            // OpenMenuItem
            // 
            this.OpenMenuItem.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenMenuItem.Name = "OpenMenuItem";
            this.OpenMenuItem.Size = new System.Drawing.Size(192, 22);
            this.OpenMenuItem.Text = "Open";
            this.OpenMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // OpenWithoutEmuMenuItem
            // 
            this.OpenWithoutEmuMenuItem.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.OpenWithoutEmuMenuItem.Name = "OpenWithoutEmuMenuItem";
            this.OpenWithoutEmuMenuItem.Size = new System.Drawing.Size(192, 22);
            this.OpenWithoutEmuMenuItem.Text = "Open Without Emulation";
            this.OpenWithoutEmuMenuItem.Click += new System.EventHandler(this.OpenWithoutEmuMenuItem_Click);
            // 
            // OpenFileLocationMenuItem
            // 
            this.OpenFileLocationMenuItem.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.OpenFileLocationMenuItem.Name = "OpenFileLocationMenuItem";
            this.OpenFileLocationMenuItem.Size = new System.Drawing.Size(192, 22);
            this.OpenFileLocationMenuItem.Text = "Open File Location";
            this.OpenFileLocationMenuItem.Click += new System.EventHandler(this.OpenFileLocationMenuItem_Click);
            // 
            // RemoveMenuItem
            // 
            this.RemoveMenuItem.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.RemoveMenuItem.Name = "RemoveMenuItem";
            this.RemoveMenuItem.Size = new System.Drawing.Size(192, 22);
            this.RemoveMenuItem.Text = "Delete";
            this.RemoveMenuItem.Click += new System.EventHandler(this.RemoveMenuItem_Click);
            // 
            // ToTopMenuItem
            // 
            this.ToTopMenuItem.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.ToTopMenuItem.Name = "ToTopMenuItem";
            this.ToTopMenuItem.Size = new System.Drawing.Size(192, 22);
            this.ToTopMenuItem.Text = "Move to Top List";
            this.ToTopMenuItem.Click += new System.EventHandler(this.ToTopMenuItem_Click);
            // 
            // ToButtomMenuItem
            // 
            this.ToButtomMenuItem.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.ToButtomMenuItem.Name = "ToButtomMenuItem";
            this.ToButtomMenuItem.Size = new System.Drawing.Size(192, 22);
            this.ToButtomMenuItem.Text = "Move to Bottom List";
            this.ToButtomMenuItem.Click += new System.EventHandler(this.ToButtomMenuItem_Click);
            // 
            // GameCacheMenuItem
            // 
            this.GameCacheMenuItem.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.GameCacheMenuItem.Name = "GameCacheMenuItem";
            this.GameCacheMenuItem.Size = new System.Drawing.Size(192, 22);
            this.GameCacheMenuItem.Text = "Download Game Cache";
            this.GameCacheMenuItem.Click += new System.EventHandler(this.GameCacheMenuItem_Click);
            // 
            // ConfigureMenuItem
            // 
            this.ConfigureMenuItem.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.ConfigureMenuItem.Name = "ConfigureMenuItem";
            this.ConfigureMenuItem.Size = new System.Drawing.Size(192, 22);
            this.ConfigureMenuItem.Text = "Properties";
            this.ConfigureMenuItem.Click += new System.EventHandler(this.ConfigureMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(1142, 609);
            this.Controls.Add(this.PN_BodyContainer);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.PN_Top);
            this.Controls.Add(this.PN_LeftContainer);
            this.EnableShadows = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1366, 728);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.PN_Top.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.PN_LeftContainer.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Add)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BT_Profile)).EndInit();
            this.gradiantBox1.ResumeLayout(false);
            this.gradiantBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.TabControl1.ResumeLayout(false);
            this.TP_Library.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_GameInfo)).EndInit();
            this.PN_RContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Banner)).EndInit();
            this.PB_Banner.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Logo)).EndInit();
            this.ShadowBox.ResumeLayout(false);
            this.TP_Console.ResumeLayout(false);
            this.PN_BodyContainer.ResumeLayout(false);
            this.CM_MenuGame.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel PN_Top;
        private SKYNET_MinimizeBox BT_Minimize;
        private SKYNET_CloseBox BT_Close;
        private Panel panel3;
        private Panel panel4;
        private Label LB_Library;
        private Label LB_Community;
        private Label LB_Profile;
        private Panel PN_LeftContainer;
        private Panel panel5;
        private SKYNET_TextBox TB_Search;
        private PictureBox PB_Add;
        private Label LB_Add;
        private PictureBox PB_Banner;
        private PictureBox PB_Logo;
        private Label LB_Status;
        private Panel ShadowBox;
        private SKYNET_Button BT_GameAction;
        private Label label6;
        private Panel PN_BodyContainer;
        private SKYNET_Label LB_GameTittle;
        private Panel panel7;
        private SKYNET_GradiantBox gradiantBox1;
        private Label label5;
        private Label label1;
        private PictureBox pictureBox1;
        private Panel panel2;
        private Panel panel1;
        private Panel panel6;
        private Label LB_NickName;
        private Label LB_SteamID;
        private Panel PN_GameContainer;
        private Panel panel8;
        private SKYNET_ContextMenuStrip CM_MenuGame;
        private ToolStripMenuItem ToTopMenuItem;
        private ToolStripMenuItem OpenMenuItem;
        private ToolStripMenuItem ConfigureMenuItem;
        private ToolStripMenuItem RemoveMenuItem;
        private ToolStripMenuItem ToButtomMenuItem;
        private ToolStripMenuItem OpenWithoutEmuMenuItem;
        private ToolStripMenuItem OpenFileLocationMenuItem;
        private SKYNET_CircularPictureBox PB_Avatar;
        private ToolStripMenuItem GameCacheMenuItem;
        private SKYNET_WebLogger WebLogger1;
        private Label LB_Browser;
        private Label LB_Clear;
        private PictureBox BT_Profile;
        private Label LB_PlayedTimeInfo;
        private Label LB_LastPlayed;
        private Label LB_LastPlayedInfo;
        private Label LB_PlayedTime;
        private Label LB_PlayingNowInfo;
        private Label LB_PlayingNow;
        private Label LB_Console;
        private SKYNET.Controls.SKYNET_TabControl TabControl1;
        private TabPage TP_Library;
        private TabPage TP_Community;
        private TabPage TP_Profile;
        private TabPage TP_Console;
        private Panel PN_RContainer;
        private Label LB_UsersOnline;
        private PictureBox PB_GameInfo;
        private Panel PN_UserContainer;
        private Label LB_ShortDescription;
    }
}

