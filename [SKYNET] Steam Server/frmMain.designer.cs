

using SKYNET.GUI.Controls;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.PN_Top = new System.Windows.Forms.Panel();
            this.skyneT_MinimizeBox1 = new SKYNET.GUI.Controls.SKYNET_MinimizeBox();
            this.skyneT_CloseBox1 = new SKYNET.GUI.Controls.SKYNET_CloseBox();
            this.LB_Tittle = new System.Windows.Forms.Label();
            this.acceptBtn = new System.Windows.Forms.Button();
            this.Browser = new System.Windows.Forms.WebBrowser();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.WebBrowserpnl = new System.Windows.Forms.Panel();
            this.Logger = new SKYNET.GUI.Controls.SKYNET_WebLogger();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.Check = new System.Windows.Forms.Timer(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.Users_playing = new System.Windows.Forms.Label();
            this.Accounts_Created = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.GC_Version = new System.Windows.Forms.Label();
            this.ConnectedClients = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TimeOnline = new System.Windows.Forms.Label();
            this.Logo = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bodyContainer = new System.Windows.Forms.Panel();
            this.pn_Progress = new System.Windows.Forms.Panel();
            this.pb_Progress = new SKYNET.GUI.Controls.SKYNET_ProgressBar();
            this.lb_Progress = new System.Windows.Forms.Label();
            this.Settings = new SKYNET.GUI.Controls.SKYNET_Button();
            this.btn_ContextMenu = new SKYNET.GUI.Controls.SKYNET_Button();
            this.shadow = new SKYNET.GUI.Controls.SKYNET_ShadowBox();
            this.ContextMenu = new SKYNET.GUI.Controls.SKYNET_ContextMenuStrip();
            this.CM_UsersManager = new System.Windows.Forms.ToolStripMenuItem();
            this.GlobalNotificationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CM_ReloadItemsImage = new System.Windows.Forms.ToolStripMenuItem();
            this.CM_ReloadItemsDB = new System.Windows.Forms.ToolStripMenuItem();
            this.CM_Export = new System.Windows.Forms.ToolStripMenuItem();
            this.registerTINserverCallbacks = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.PN_Top.SuspendLayout();
            this.WebBrowserpnl.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            this.panel2.SuspendLayout();
            this.bodyContainer.SuspendLayout();
            this.pn_Progress.SuspendLayout();
            this.ContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // PN_Top
            // 
            this.PN_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.PN_Top.Controls.Add(this.skyneT_MinimizeBox1);
            this.PN_Top.Controls.Add(this.skyneT_CloseBox1);
            this.PN_Top.Controls.Add(this.LB_Tittle);
            this.PN_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.PN_Top.ForeColor = System.Drawing.Color.White;
            this.PN_Top.Location = new System.Drawing.Point(0, 0);
            this.PN_Top.Name = "PN_Top";
            this.PN_Top.Size = new System.Drawing.Size(917, 26);
            this.PN_Top.TabIndex = 5;
            // 
            // skyneT_MinimizeBox1
            // 
            this.skyneT_MinimizeBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.skyneT_MinimizeBox1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.skyneT_MinimizeBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.skyneT_MinimizeBox1.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(64)))), ((int)(((byte)(78)))));
            this.skyneT_MinimizeBox1.Location = new System.Drawing.Point(849, 0);
            this.skyneT_MinimizeBox1.MaximumSize = new System.Drawing.Size(34, 26);
            this.skyneT_MinimizeBox1.MinimumSize = new System.Drawing.Size(34, 26);
            this.skyneT_MinimizeBox1.Name = "skyneT_MinimizeBox1";
            this.skyneT_MinimizeBox1.Size = new System.Drawing.Size(34, 26);
            this.skyneT_MinimizeBox1.TabIndex = 9;
            this.skyneT_MinimizeBox1.Click += new System.EventHandler(this.Minimize_click);
            // 
            // skyneT_CloseBox1
            // 
            this.skyneT_CloseBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.skyneT_CloseBox1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.skyneT_CloseBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.skyneT_CloseBox1.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(64)))), ((int)(((byte)(78)))));
            this.skyneT_CloseBox1.Location = new System.Drawing.Point(883, 0);
            this.skyneT_CloseBox1.MaximumSize = new System.Drawing.Size(34, 26);
            this.skyneT_CloseBox1.MinimumSize = new System.Drawing.Size(34, 26);
            this.skyneT_CloseBox1.Name = "skyneT_CloseBox1";
            this.skyneT_CloseBox1.Size = new System.Drawing.Size(34, 26);
            this.skyneT_CloseBox1.TabIndex = 8;
            this.skyneT_CloseBox1.Clicked += new System.EventHandler(this.closeBox_Click);
            this.skyneT_CloseBox1.Click += new System.EventHandler(this.closeBox_Click);
            // 
            // LB_Tittle
            // 
            this.LB_Tittle.AutoSize = true;
            this.LB_Tittle.Font = new System.Drawing.Font("Segoe UI Emoji", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Tittle.Location = new System.Drawing.Point(8, 4);
            this.LB_Tittle.Name = "LB_Tittle";
            this.LB_Tittle.Size = new System.Drawing.Size(150, 16);
            this.LB_Tittle.TabIndex = 7;
            this.LB_Tittle.Text = "[SKYNET] Steam Server";
            // 
            // acceptBtn
            // 
            this.acceptBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.acceptBtn.Location = new System.Drawing.Point(819, 372);
            this.acceptBtn.Name = "acceptBtn";
            this.acceptBtn.Size = new System.Drawing.Size(75, 23);
            this.acceptBtn.TabIndex = 16;
            this.acceptBtn.Text = "button1";
            this.acceptBtn.UseVisualStyleBackColor = true;
            // 
            // Browser
            // 
            this.Browser.Location = new System.Drawing.Point(-21, -2);
            this.Browser.Name = "Browser";
            this.Browser.Size = new System.Drawing.Size(16, 20);
            this.Browser.TabIndex = 18;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "P0.jpg");
            this.imageList1.Images.SetKeyName(1, "P1.jpg");
            this.imageList1.Images.SetKeyName(2, "P2.jpg");
            this.imageList1.Images.SetKeyName(3, "P3.jpg");
            this.imageList1.Images.SetKeyName(4, "P4.jpg");
            // 
            // WebBrowserpnl
            // 
            this.WebBrowserpnl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.WebBrowserpnl.Controls.Add(this.Logger);
            this.WebBrowserpnl.Dock = System.Windows.Forms.DockStyle.Top;
            this.WebBrowserpnl.Location = new System.Drawing.Point(0, 64);
            this.WebBrowserpnl.Name = "WebBrowserpnl";
            this.WebBrowserpnl.Padding = new System.Windows.Forms.Padding(8);
            this.WebBrowserpnl.Size = new System.Drawing.Size(917, 400);
            this.WebBrowserpnl.TabIndex = 34;
            // 
            // Logger
            // 
            this.Logger.AutoScrollLines = true;
            this.Logger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Logger.Location = new System.Drawing.Point(8, 8);
            this.Logger.LoggerBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.Logger.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Logger.Name = "Logger";
            this.Logger.ScrollColors = System.Drawing.Color.Empty;
            this.Logger.Size = new System.Drawing.Size(901, 384);
            this.Logger.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panel3.Location = new System.Drawing.Point(848, 36);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(1);
            this.panel3.Size = new System.Drawing.Size(61, 20);
            this.panel3.TabIndex = 39;
            this.panel3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ClearScreen_MouseClick);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(1, 1);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(59, 18);
            this.panel4.TabIndex = 28;
            this.panel4.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ClearScreen_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Font = new System.Drawing.Font("Segoe UI Emoji", 7F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(210)))), ((int)(((byte)(217)))));
            this.label2.Location = new System.Drawing.Point(1, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 14);
            this.label2.TabIndex = 27;
            this.label2.Text = "Clear screen";
            this.label2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ClearScreen_MouseClick);
            // 
            // Check
            // 
            this.Check.Enabled = true;
            this.Check.Interval = 1000;
            this.Check.Tick += new System.EventHandler(this.Check_Tick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(230)))), ((int)(((byte)(237)))));
            this.label5.Location = new System.Drawing.Point(83, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 15);
            this.label5.TabIndex = 42;
            this.label5.Text = "Users playing";
            // 
            // Users_playing
            // 
            this.Users_playing.AutoSize = true;
            this.Users_playing.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Users_playing.Location = new System.Drawing.Point(191, 41);
            this.Users_playing.Name = "Users_playing";
            this.Users_playing.Size = new System.Drawing.Size(13, 15);
            this.Users_playing.TabIndex = 43;
            this.Users_playing.Text = "0";
            // 
            // Accounts_Created
            // 
            this.Accounts_Created.AutoSize = true;
            this.Accounts_Created.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Accounts_Created.Location = new System.Drawing.Point(191, 5);
            this.Accounts_Created.Name = "Accounts_Created";
            this.Accounts_Created.Size = new System.Drawing.Size(13, 15);
            this.Accounts_Created.TabIndex = 40;
            this.Accounts_Created.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(230)))), ((int)(((byte)(237)))));
            this.label11.Location = new System.Drawing.Point(281, 5);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(116, 15);
            this.label11.TabIndex = 44;
            this.label11.Text = "Steam Server version";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(230)))), ((int)(((byte)(237)))));
            this.label3.Location = new System.Drawing.Point(83, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 15);
            this.label3.TabIndex = 39;
            this.label3.Text = "Accounts created";
            // 
            // GC_Version
            // 
            this.GC_Version.AutoSize = true;
            this.GC_Version.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.GC_Version.Location = new System.Drawing.Point(389, 5);
            this.GC_Version.Name = "GC_Version";
            this.GC_Version.Size = new System.Drawing.Size(0, 15);
            this.GC_Version.TabIndex = 45;
            // 
            // ConnectedClients
            // 
            this.ConnectedClients.AutoSize = true;
            this.ConnectedClients.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ConnectedClients.Location = new System.Drawing.Point(191, 23);
            this.ConnectedClients.Name = "ConnectedClients";
            this.ConnectedClients.Size = new System.Drawing.Size(13, 15);
            this.ConnectedClients.TabIndex = 37;
            this.ConnectedClients.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(230)))), ((int)(((byte)(237)))));
            this.label9.Location = new System.Drawing.Point(281, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(104, 15);
            this.label9.TabIndex = 46;
            this.label9.Text = "Server Online time";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(230)))), ((int)(((byte)(237)))));
            this.label1.Location = new System.Drawing.Point(83, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 15);
            this.label1.TabIndex = 36;
            this.label1.Text = "Connected Clients";
            // 
            // TimeOnline
            // 
            this.TimeOnline.AutoSize = true;
            this.TimeOnline.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.TimeOnline.Location = new System.Drawing.Point(410, 23);
            this.TimeOnline.Name = "TimeOnline";
            this.TimeOnline.Size = new System.Drawing.Size(61, 15);
            this.TimeOnline.TabIndex = 47;
            this.TimeOnline.Text = "00 : 00 : 00";
            // 
            // Logo
            // 
            this.Logo.Image = global::SKYNET.Properties.Resources.logo;
            this.Logo.Location = new System.Drawing.Point(8, 6);
            this.Logo.Name = "Logo";
            this.Logo.Size = new System.Drawing.Size(57, 52);
            this.Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Logo.TabIndex = 1;
            this.Logo.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.Logo);
            this.panel2.Controls.Add(this.TimeOnline);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.ConnectedClients);
            this.panel2.Controls.Add(this.GC_Version);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.Accounts_Created);
            this.panel2.Controls.Add(this.Users_playing);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.ForeColor = System.Drawing.Color.White;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(917, 64);
            this.panel2.TabIndex = 33;
            // 
            // bodyContainer
            // 
            this.bodyContainer.Controls.Add(this.pn_Progress);
            this.bodyContainer.Controls.Add(this.WebBrowserpnl);
            this.bodyContainer.Controls.Add(this.Settings);
            this.bodyContainer.Controls.Add(this.panel2);
            this.bodyContainer.Controls.Add(this.btn_ContextMenu);
            this.bodyContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyContainer.Location = new System.Drawing.Point(0, 26);
            this.bodyContainer.Name = "bodyContainer";
            this.bodyContainer.Size = new System.Drawing.Size(917, 507);
            this.bodyContainer.TabIndex = 41;
            // 
            // pn_Progress
            // 
            this.pn_Progress.Controls.Add(this.pb_Progress);
            this.pn_Progress.Controls.Add(this.lb_Progress);
            this.pn_Progress.Location = new System.Drawing.Point(202, 464);
            this.pn_Progress.Name = "pn_Progress";
            this.pn_Progress.Size = new System.Drawing.Size(707, 33);
            this.pn_Progress.TabIndex = 41;
            this.pn_Progress.Visible = false;
            // 
            // pb_Progress
            // 
            this.pb_Progress.BackColor = System.Drawing.Color.Transparent;
            this.pb_Progress.DrawHatch = true;
            this.pb_Progress.Location = new System.Drawing.Point(6, 17);
            this.pb_Progress.Maximum = 100;
            this.pb_Progress.Minimum = 0;
            this.pb_Progress.Name = "pb_Progress";
            this.pb_Progress.ProgressColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.pb_Progress.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(200)))));
            this.pb_Progress.ShowPercentage = false;
            this.pb_Progress.Size = new System.Drawing.Size(697, 11);
            this.pb_Progress.TabIndex = 1;
            this.pb_Progress.Text = "hackProgressBar1";
            this.pb_Progress.Value = 0;
            this.pb_Progress.ValueAlignment = SKYNET.GUI.Controls.SKYNET_ProgressBar.Alignment.Right;
            // 
            // lb_Progress
            // 
            this.lb_Progress.AutoSize = true;
            this.lb_Progress.Font = new System.Drawing.Font("Segoe UI Emoji", 7.5F);
            this.lb_Progress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.lb_Progress.Location = new System.Drawing.Point(3, 1);
            this.lb_Progress.Name = "lb_Progress";
            this.lb_Progress.Size = new System.Drawing.Size(136, 14);
            this.lb_Progress.TabIndex = 0;
            this.lb_Progress.Text = "Exporting Items to mongodb";
            // 
            // Settings
            // 
            this.Settings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.Settings.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.Settings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Settings.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Settings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.Settings.ForeColorMouseOver = System.Drawing.Color.White;
            this.Settings.ImageAlignment = SKYNET.GUI.Controls.SKYNET_Button._ImgAlign.Left;
            this.Settings.ImageIcon = null;
            this.Settings.Location = new System.Drawing.Point(107, 468);
            this.Settings.MenuMode = false;
            this.Settings.Name = "Settings";
            this.Settings.Rounded = true;
            this.Settings.Size = new System.Drawing.Size(89, 25);
            this.Settings.Style = SKYNET.GUI.Controls.SKYNET_Button._Style.TextOnly;
            this.Settings.TabIndex = 40;
            this.Settings.Text = "Settings";
            this.Settings.Click += new System.EventHandler(this.Settings_Click);
            // 
            // btn_ContextMenu
            // 
            this.btn_ContextMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.btn_ContextMenu.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.btn_ContextMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_ContextMenu.Enabled = false;
            this.btn_ContextMenu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btn_ContextMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.btn_ContextMenu.ForeColorMouseOver = System.Drawing.Color.White;
            this.btn_ContextMenu.ImageAlignment = SKYNET.GUI.Controls.SKYNET_Button._ImgAlign.Left;
            this.btn_ContextMenu.ImageIcon = null;
            this.btn_ContextMenu.Location = new System.Drawing.Point(12, 468);
            this.btn_ContextMenu.MenuMode = false;
            this.btn_ContextMenu.Name = "btn_ContextMenu";
            this.btn_ContextMenu.Rounded = true;
            this.btn_ContextMenu.Size = new System.Drawing.Size(89, 25);
            this.btn_ContextMenu.Style = SKYNET.GUI.Controls.SKYNET_Button._Style.TextOnly;
            this.btn_ContextMenu.TabIndex = 39;
            this.btn_ContextMenu.Text = "Tools";
            this.btn_ContextMenu.Click += new System.EventHandler(this.Btn_ContextMenu_Click);
            // 
            // shadow
            // 
            this.shadow.BackColor = System.Drawing.Color.Transparent;
            this.shadow.Color = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(26)))), ((int)(((byte)(37)))));
            this.shadow.Location = new System.Drawing.Point(0, 0);
            this.shadow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.shadow.Name = "shadow";
            this.shadow.Opacity = 70;
            this.shadow.Size = new System.Drawing.Size(0, 0);
            this.shadow.TabIndex = 1;
            // 
            // ContextMenu
            // 
            this.ContextMenu.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.ContextMenu.ForeColor = System.Drawing.Color.White;
            this.ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CM_UsersManager,
            this.GlobalNotificationMenuItem,
            this.CM_ReloadItemsImage,
            this.CM_ReloadItemsDB,
            this.CM_Export,
            this.registerTINserverCallbacks});
            this.ContextMenu.Name = "ContextMenu";
            this.ContextMenu.ShowImageMargin = false;
            this.ContextMenu.Size = new System.Drawing.Size(190, 136);
            // 
            // CM_UsersManager
            // 
            this.CM_UsersManager.Name = "CM_UsersManager";
            this.CM_UsersManager.Size = new System.Drawing.Size(189, 22);
            this.CM_UsersManager.Text = "Users manager";
            this.CM_UsersManager.Click += new System.EventHandler(this.CM_UsersManager_Click);
            // 
            // GlobalNotificationMenuItem
            // 
            this.GlobalNotificationMenuItem.Name = "GlobalNotificationMenuItem";
            this.GlobalNotificationMenuItem.Size = new System.Drawing.Size(189, 22);
            this.GlobalNotificationMenuItem.Text = "Send global notification";
            // 
            // CM_ReloadItemsImage
            // 
            this.CM_ReloadItemsImage.Name = "CM_ReloadItemsImage";
            this.CM_ReloadItemsImage.Size = new System.Drawing.Size(189, 22);
            this.CM_ReloadItemsImage.Text = "Reload Items images";
            // 
            // CM_ReloadItemsDB
            // 
            this.CM_ReloadItemsDB.Name = "CM_ReloadItemsDB";
            this.CM_ReloadItemsDB.Size = new System.Drawing.Size(189, 22);
            this.CM_ReloadItemsDB.Text = "Reload Items in DB";
            // 
            // CM_Export
            // 
            this.CM_Export.Name = "CM_Export";
            this.CM_Export.Size = new System.Drawing.Size(189, 22);
            this.CM_Export.Text = "Import/Export DB";
            // 
            // registerTINserverCallbacks
            // 
            this.registerTINserverCallbacks.Name = "registerTINserverCallbacks";
            this.registerTINserverCallbacks.Size = new System.Drawing.Size(189, 22);
            this.registerTINserverCallbacks.Text = "Register TINserver callbacks";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label4.Location = new System.Drawing.Point(410, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 15);
            this.label4.TabIndex = 48;
            this.label4.Text = "1.00";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(917, 533);
            this.Controls.Add(this.shadow);
            this.Controls.Add(this.bodyContainer);
            this.Controls.Add(this.Browser);
            this.Controls.Add(this.acceptBtn);
            this.Controls.Add(this.PN_Top);
            this.Font = new System.Drawing.Font("Segoe UI Emoji", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(1360, 728);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "[SKYNET] Dota2 GCS";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.PN_Top.ResumeLayout(false);
            this.PN_Top.PerformLayout();
            this.WebBrowserpnl.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.bodyContainer.ResumeLayout(false);
            this.pn_Progress.ResumeLayout(false);
            this.pn_Progress.PerformLayout();
            this.ContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel PN_Top;
        private System.Windows.Forms.Button acceptBtn;
        private System.Windows.Forms.WebBrowser Browser;
        private System.Windows.Forms.ImageList imageList1;
        private SKYNET_ContextMenuStrip ContextMenu;
        private System.Windows.Forms.ToolStripMenuItem CM_UsersManager;
        private System.Windows.Forms.ToolStripMenuItem CM_ReloadItemsDB;
        private System.Windows.Forms.ToolStripMenuItem CM_ReloadItemsImage;
        public System.Windows.Forms.Label LB_Tittle;
        private System.Windows.Forms.Panel WebBrowserpnl;
        private System.Windows.Forms.Timer Check;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label Users_playing;
        public System.Windows.Forms.Label Accounts_Created;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label GC_Version;
        public System.Windows.Forms.Label ConnectedClients;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label TimeOnline;
        private System.Windows.Forms.PictureBox Logo;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem CM_Export;
        public SKYNET_Button btn_ContextMenu;
        public SKYNET_Button Settings;
        public SKYNET_WebLogger Logger;
        private SKYNET_ShadowBox shadow;
        private System.Windows.Forms.Panel bodyContainer;
        private System.Windows.Forms.Panel pn_Progress;
        public System.Windows.Forms.Label lb_Progress;
        private SKYNET_ProgressBar pb_Progress;
        private System.Windows.Forms.ToolStripMenuItem GlobalNotificationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registerTINserverCallbacks;
        private SKYNET_MinimizeBox skyneT_MinimizeBox1;
        private SKYNET_CloseBox skyneT_CloseBox1;
        public System.Windows.Forms.Label label4;
    }
}