using System.Windows.Forms;

namespace SKYNET
{
    partial class frmGameManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGameManager));
            this.PN_Top = new System.Windows.Forms.Panel();
            this.BT_Close = new SKYNET.Controls.SKYNET_CloseBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.BT_Cancel = new SKYNET_Button();
            this.BT_AddGame = new SKYNET_Button();
            this.PN_LeftContainer = new System.Windows.Forms.Panel();
            this.PN_BodyContainer = new System.Windows.Forms.Panel();
            this.CHB_WithoutEmu = new SKYNET.Controls.SKYNET_Check();
            this.label6 = new System.Windows.Forms.Label();
            this.LB_Name = new System.Windows.Forms.Label();
            this.PB_Avatar = new System.Windows.Forms.PictureBox();
            this.TB_AppId = new SKYNET.Controls.SKYNET_TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TB_Parameters = new SKYNET.Controls.SKYNET_TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TB_SteamApiPath = new SKYNET.Controls.SKYNET_TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TB_ExecutablePath = new SKYNET.Controls.SKYNET_TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_Name = new SKYNET.Controls.SKYNET_TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PN_Top.SuspendLayout();
            this.panel3.SuspendLayout();
            this.PN_BodyContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).BeginInit();
            this.SuspendLayout();
            // 
            // PN_Top
            // 
            this.PN_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.PN_Top.Controls.Add(this.BT_Close);
            this.PN_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.PN_Top.Location = new System.Drawing.Point(0, 0);
            this.PN_Top.Name = "PN_Top";
            this.PN_Top.Size = new System.Drawing.Size(741, 26);
            this.PN_Top.TabIndex = 0;
            // 
            // BT_Close
            // 
            this.BT_Close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.BT_Close.Color = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.BT_Close.Dock = System.Windows.Forms.DockStyle.Right;
            this.BT_Close.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(43)))), ((int)(((byte)(45)))));
            this.BT_Close.Location = new System.Drawing.Point(707, 0);
            this.BT_Close.MaximumSize = new System.Drawing.Size(34, 26);
            this.BT_Close.MinimumSize = new System.Drawing.Size(34, 26);
            this.BT_Close.Name = "BT_Close";
            this.BT_Close.Size = new System.Drawing.Size(34, 26);
            this.BT_Close.TabIndex = 0;
            this.BT_Close.Clicked += new System.EventHandler(this.Close_Clicked);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(46)))), ((int)(((byte)(51)))));
            this.panel3.Controls.Add(this.BT_Cancel);
            this.panel3.Controls.Add(this.BT_AddGame);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 509);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(741, 69);
            this.panel3.TabIndex = 7;
            // 
            // BT_Cancel
            // 
            this.BT_Cancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.BT_Cancel.BackColorMouseOver = System.Drawing.Color.Empty;
            this.BT_Cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BT_Cancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BT_Cancel.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BT_Cancel.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.BT_Cancel.ImageAlignment = SKYNET_Button._ImgAlign.Left;
            this.BT_Cancel.ImageIcon = null;
            this.BT_Cancel.Location = new System.Drawing.Point(468, 15);
            this.BT_Cancel.MenuMode = false;
            this.BT_Cancel.Name = "BT_Cancel";
            this.BT_Cancel.Rounded = false;
            this.BT_Cancel.Size = new System.Drawing.Size(100, 32);
            this.BT_Cancel.Style = SKYNET_Button._Style.TextOnly;
            this.BT_Cancel.TabIndex = 12;
            this.BT_Cancel.Text = "Cancel";
            this.BT_Cancel.Click += new System.EventHandler(this.Close_Clicked);
            // 
            // BT_AddGame
            // 
            this.BT_AddGame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.BT_AddGame.BackColorMouseOver = System.Drawing.Color.Empty;
            this.BT_AddGame.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BT_AddGame.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BT_AddGame.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BT_AddGame.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.BT_AddGame.ImageAlignment = SKYNET_Button._ImgAlign.Left;
            this.BT_AddGame.ImageIcon = null;
            this.BT_AddGame.Location = new System.Drawing.Point(574, 15);
            this.BT_AddGame.MenuMode = false;
            this.BT_AddGame.Name = "BT_AddGame";
            this.BT_AddGame.Rounded = false;
            this.BT_AddGame.Size = new System.Drawing.Size(100, 32);
            this.BT_AddGame.Style = SKYNET_Button._Style.TextOnly;
            this.BT_AddGame.TabIndex = 11;
            this.BT_AddGame.Text = "Add Game";
            this.BT_AddGame.Click += new System.EventHandler(this.AddGame_Click);
            // 
            // PN_LeftContainer
            // 
            this.PN_LeftContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(46)))), ((int)(((byte)(51)))));
            this.PN_LeftContainer.Dock = System.Windows.Forms.DockStyle.Left;
            this.PN_LeftContainer.Location = new System.Drawing.Point(0, 0);
            this.PN_LeftContainer.Name = "PN_LeftContainer";
            this.PN_LeftContainer.Size = new System.Drawing.Size(0, 578);
            this.PN_LeftContainer.TabIndex = 6;
            // 
            // PN_BodyContainer
            // 
            this.PN_BodyContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(50)))), ((int)(((byte)(57)))));
            this.PN_BodyContainer.Controls.Add(this.CHB_WithoutEmu);
            this.PN_BodyContainer.Controls.Add(this.label6);
            this.PN_BodyContainer.Controls.Add(this.LB_Name);
            this.PN_BodyContainer.Controls.Add(this.PB_Avatar);
            this.PN_BodyContainer.Controls.Add(this.TB_AppId);
            this.PN_BodyContainer.Controls.Add(this.label5);
            this.PN_BodyContainer.Controls.Add(this.TB_Parameters);
            this.PN_BodyContainer.Controls.Add(this.label3);
            this.PN_BodyContainer.Controls.Add(this.TB_SteamApiPath);
            this.PN_BodyContainer.Controls.Add(this.label4);
            this.PN_BodyContainer.Controls.Add(this.TB_ExecutablePath);
            this.PN_BodyContainer.Controls.Add(this.label2);
            this.PN_BodyContainer.Controls.Add(this.TB_Name);
            this.PN_BodyContainer.Controls.Add(this.label1);
            this.PN_BodyContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PN_BodyContainer.Location = new System.Drawing.Point(0, 26);
            this.PN_BodyContainer.Name = "PN_BodyContainer";
            this.PN_BodyContainer.Size = new System.Drawing.Size(741, 483);
            this.PN_BodyContainer.TabIndex = 10;
            // 
            // CHB_WithoutEmu
            // 
            this.CHB_WithoutEmu.BackColor = System.Drawing.Color.Transparent;
            this.CHB_WithoutEmu.Checked = false;
            this.CHB_WithoutEmu.Location = new System.Drawing.Point(640, 438);
            this.CHB_WithoutEmu.Name = "CHB_WithoutEmu";
            this.CHB_WithoutEmu.Size = new System.Drawing.Size(34, 25);
            this.CHB_WithoutEmu.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label6.Location = new System.Drawing.Point(63, 441);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(196, 17);
            this.label6.TabIndex = 13;
            this.label6.Text = "Launch game without emulation ";
            // 
            // LB_Name
            // 
            this.LB_Name.AutoSize = true;
            this.LB_Name.Font = new System.Drawing.Font("Segoe UI Emoji", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Name.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.LB_Name.Location = new System.Drawing.Point(145, 31);
            this.LB_Name.Name = "LB_Name";
            this.LB_Name.Size = new System.Drawing.Size(83, 32);
            this.LB_Name.TabIndex = 12;
            this.LB_Name.Text = "Name";
            // 
            // PB_Avatar
            // 
            this.PB_Avatar.Location = new System.Drawing.Point(66, 17);
            this.PB_Avatar.Name = "PB_Avatar";
            this.PB_Avatar.Size = new System.Drawing.Size(60, 60);
            this.PB_Avatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Avatar.TabIndex = 11;
            this.PB_Avatar.TabStop = false;
            // 
            // TB_AppId
            // 
            this.TB_AppId.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_AppId.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_AppId.Color = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_AppId.IsPassword = false;
            this.TB_AppId.Location = new System.Drawing.Point(66, 389);
            this.TB_AppId.Logo = null;
            this.TB_AppId.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_AppId.Name = "TB_AppId";
            this.TB_AppId.OnlyNumbers = false;
            this.TB_AppId.ShowLogo = false;
            this.TB_AppId.Size = new System.Drawing.Size(608, 35);
            this.TB_AppId.TabIndex = 10;
            this.TB_AppId.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label5.Location = new System.Drawing.Point(63, 367);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "App ID";
            // 
            // TB_Parameters
            // 
            this.TB_Parameters.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_Parameters.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_Parameters.Color = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_Parameters.IsPassword = false;
            this.TB_Parameters.Location = new System.Drawing.Point(66, 321);
            this.TB_Parameters.Logo = null;
            this.TB_Parameters.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_Parameters.Name = "TB_Parameters";
            this.TB_Parameters.OnlyNumbers = false;
            this.TB_Parameters.ShowLogo = false;
            this.TB_Parameters.Size = new System.Drawing.Size(608, 35);
            this.TB_Parameters.TabIndex = 8;
            this.TB_Parameters.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.Location = new System.Drawing.Point(63, 299);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Game Parameters";
            // 
            // TB_SteamApiPath
            // 
            this.TB_SteamApiPath.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_SteamApiPath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_SteamApiPath.Color = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_SteamApiPath.IsPassword = false;
            this.TB_SteamApiPath.Location = new System.Drawing.Point(66, 254);
            this.TB_SteamApiPath.Logo = null;
            this.TB_SteamApiPath.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_SteamApiPath.Name = "TB_SteamApiPath";
            this.TB_SteamApiPath.OnlyNumbers = false;
            this.TB_SteamApiPath.ShowLogo = false;
            this.TB_SteamApiPath.Size = new System.Drawing.Size(608, 35);
            this.TB_SteamApiPath.TabIndex = 6;
            this.TB_SteamApiPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label4.Location = new System.Drawing.Point(63, 232);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 17);
            this.label4.TabIndex = 5;
            this.label4.Text = "SteamApi Path";
            // 
            // TB_ExecutablePath
            // 
            this.TB_ExecutablePath.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_ExecutablePath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_ExecutablePath.Color = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_ExecutablePath.IsPassword = false;
            this.TB_ExecutablePath.Location = new System.Drawing.Point(66, 186);
            this.TB_ExecutablePath.Logo = null;
            this.TB_ExecutablePath.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_ExecutablePath.Name = "TB_ExecutablePath";
            this.TB_ExecutablePath.OnlyNumbers = false;
            this.TB_ExecutablePath.ShowLogo = false;
            this.TB_ExecutablePath.Size = new System.Drawing.Size(608, 35);
            this.TB_ExecutablePath.TabIndex = 4;
            this.TB_ExecutablePath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(63, 164);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Executable Path";
            // 
            // TB_Name
            // 
            this.TB_Name.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_Name.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_Name.Color = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(70)))), ((int)(((byte)(77)))));
            this.TB_Name.IsPassword = false;
            this.TB_Name.Location = new System.Drawing.Point(66, 119);
            this.TB_Name.Logo = null;
            this.TB_Name.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_Name.Name = "TB_Name";
            this.TB_Name.OnlyNumbers = false;
            this.TB_Name.ShowLogo = false;
            this.TB_Name.Size = new System.Drawing.Size(608, 35);
            this.TB_Name.TabIndex = 2;
            this.TB_Name.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TB_Name.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TB_Name_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(63, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name";
            // 
            // frmGameManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(741, 578);
            this.Controls.Add(this.PN_BodyContainer);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.PN_Top);
            this.Controls.Add(this.PN_LeftContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmGameManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.PN_Top.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.PN_BodyContainer.ResumeLayout(false);
            this.PN_BodyContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PN_Top;
        private Controls.SKYNET_CloseBox BT_Close;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel PN_LeftContainer;
        private Panel PN_BodyContainer;
        private Label label1;
        private Controls.SKYNET_TextBox TB_ExecutablePath;
        private Label label2;
        private Controls.SKYNET_TextBox TB_Name;
        private Controls.SKYNET_TextBox TB_Parameters;
        private Label label3;
        private Controls.SKYNET_TextBox TB_SteamApiPath;
        private Label label4;
        private Controls.SKYNET_TextBox TB_AppId;
        private Label label5;
        private SKYNET_Button BT_Cancel;
        private SKYNET_Button BT_AddGame;
        private PictureBox PB_Avatar;
        private Label LB_Name;
        private Label label6;
        private Controls.SKYNET_Check CHB_WithoutEmu;
    }
}

