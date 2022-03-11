namespace SKYNET.GUI
{
    partial class frmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            this.PN_Top = new System.Windows.Forms.Panel();
            this.CloseBox = new SKYNET.Controls.SKYNET_CloseBox();
            this.P_LogoContainer = new System.Windows.Forms.Panel();
            this.Logo = new System.Windows.Forms.PictureBox();
            this.P_Body = new System.Windows.Forms.Panel();
            this.BT_Launch = new SKYNET.Controls.SKYNET_Button();
            this.TB_SteamId = new SKYNET.Controls.SKYNET_TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_Languaje = new SKYNET.Controls.SKYNET_TextBox();
            this.TB_UserName = new SKYNET.Controls.SKYNET_TextBox();
            this._password = new System.Windows.Forms.Label();
            this._account = new System.Windows.Forms.Label();
            this.P_Logo = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.L_Company = new System.Windows.Forms.Label();
            this.PN_Top.SuspendLayout();
            this.P_LogoContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            this.P_Body.SuspendLayout();
            this.P_Logo.SuspendLayout();
            this.SuspendLayout();
            // 
            // PN_Top
            // 
            this.PN_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.PN_Top.Controls.Add(this.CloseBox);
            this.PN_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.PN_Top.Location = new System.Drawing.Point(0, 0);
            this.PN_Top.Name = "PN_Top";
            this.PN_Top.Size = new System.Drawing.Size(282, 27);
            this.PN_Top.TabIndex = 0;
            // 
            // CloseBox
            // 
            this.CloseBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.CloseBox.Color = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.CloseBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.CloseBox.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(64)))), ((int)(((byte)(78)))));
            this.CloseBox.Location = new System.Drawing.Point(248, 0);
            this.CloseBox.MaximumSize = new System.Drawing.Size(34, 26);
            this.CloseBox.MinimumSize = new System.Drawing.Size(34, 26);
            this.CloseBox.Name = "CloseBox";
            this.CloseBox.Size = new System.Drawing.Size(34, 26);
            this.CloseBox.TabIndex = 0;
            this.CloseBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CloseBox_MouseClick);
            // 
            // P_LogoContainer
            // 
            this.P_LogoContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.P_LogoContainer.Controls.Add(this.Logo);
            this.P_LogoContainer.Location = new System.Drawing.Point(0, 4);
            this.P_LogoContainer.Name = "P_LogoContainer";
            this.P_LogoContainer.Size = new System.Drawing.Size(338, 116);
            this.P_LogoContainer.TabIndex = 38;
            // 
            // Logo
            // 
            this.Logo.Image = ((System.Drawing.Image)(resources.GetObject("Logo.Image")));
            this.Logo.Location = new System.Drawing.Point(88, -2);
            this.Logo.Name = "Logo";
            this.Logo.Size = new System.Drawing.Size(105, 105);
            this.Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Logo.TabIndex = 1;
            this.Logo.TabStop = false;
            // 
            // P_Body
            // 
            this.P_Body.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.P_Body.Controls.Add(this.BT_Launch);
            this.P_Body.Controls.Add(this.TB_SteamId);
            this.P_Body.Controls.Add(this.label2);
            this.P_Body.Controls.Add(this.TB_Languaje);
            this.P_Body.Controls.Add(this.TB_UserName);
            this.P_Body.Controls.Add(this._password);
            this.P_Body.Controls.Add(this._account);
            this.P_Body.Dock = System.Windows.Forms.DockStyle.Top;
            this.P_Body.Location = new System.Drawing.Point(0, 198);
            this.P_Body.Name = "P_Body";
            this.P_Body.Padding = new System.Windows.Forms.Padding(8);
            this.P_Body.Size = new System.Drawing.Size(282, 256);
            this.P_Body.TabIndex = 37;
            // 
            // BT_Launch
            // 
            this.BT_Launch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.BT_Launch.BackColorMouseOver = System.Drawing.Color.Empty;
            this.BT_Launch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BT_Launch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BT_Launch.ForeColor = System.Drawing.Color.White;
            this.BT_Launch.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.BT_Launch.ImageAlignment = SKYNET.Controls.SKYNET_Button.ImgAlign.Left;
            this.BT_Launch.ImageIcon = null;
            this.BT_Launch.Location = new System.Drawing.Point(45, 207);
            this.BT_Launch.MenuMode = false;
            this.BT_Launch.Name = "BT_Launch";
            this.BT_Launch.Rounded = false;
            this.BT_Launch.Size = new System.Drawing.Size(193, 28);
            this.BT_Launch.Style = SKYNET.Controls.SKYNET_Button._Style.TextOnly;
            this.BT_Launch.TabIndex = 76;
            this.BT_Launch.Text = "SAVE AND LAUNCH";
            this.BT_Launch.Click += new System.EventHandler(this.BT_Launch_Click);
            // 
            // TB_SteamId
            // 
            this.TB_SteamId.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.TB_SteamId.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.TB_SteamId.Color = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.TB_SteamId.IsPassword = false;
            this.TB_SteamId.Location = new System.Drawing.Point(45, 158);
            this.TB_SteamId.Logo = null;
            this.TB_SteamId.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_SteamId.Name = "TB_SteamId";
            this.TB_SteamId.OnlyNumbers = true;
            this.TB_SteamId.ShowLogo = true;
            this.TB_SteamId.Size = new System.Drawing.Size(193, 35);
            this.TB_SteamId.TabIndex = 75;
            this.TB_SteamId.TopSeparator = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(211)))), ((int)(((byte)(236)))));
            this.label2.Location = new System.Drawing.Point(42, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 17);
            this.label2.TabIndex = 74;
            this.label2.Text = "Generated SteamId";
            // 
            // TB_Languaje
            // 
            this.TB_Languaje.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.TB_Languaje.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.TB_Languaje.Color = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.TB_Languaje.IsPassword = false;
            this.TB_Languaje.Location = new System.Drawing.Point(45, 94);
            this.TB_Languaje.Logo = null;
            this.TB_Languaje.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_Languaje.Name = "TB_Languaje";
            this.TB_Languaje.OnlyNumbers = false;
            this.TB_Languaje.ShowLogo = true;
            this.TB_Languaje.Size = new System.Drawing.Size(193, 35);
            this.TB_Languaje.TabIndex = 73;
            this.TB_Languaje.TopSeparator = 6;
            // 
            // TB_UserName
            // 
            this.TB_UserName.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.TB_UserName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.TB_UserName.Color = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.TB_UserName.IsPassword = false;
            this.TB_UserName.Location = new System.Drawing.Point(45, 31);
            this.TB_UserName.Logo = null;
            this.TB_UserName.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_UserName.Name = "TB_UserName";
            this.TB_UserName.OnlyNumbers = false;
            this.TB_UserName.ShowLogo = true;
            this.TB_UserName.Size = new System.Drawing.Size(193, 35);
            this.TB_UserName.TabIndex = 72;
            this.TB_UserName.TopSeparator = 6;
            // 
            // _password
            // 
            this._password.AutoSize = true;
            this._password.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this._password.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(211)))), ((int)(((byte)(236)))));
            this._password.Location = new System.Drawing.Point(42, 74);
            this._password.Name = "_password";
            this._password.Size = new System.Drawing.Size(64, 17);
            this._password.TabIndex = 49;
            this._password.Text = "Languaje";
            // 
            // _account
            // 
            this._account.AutoSize = true;
            this._account.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this._account.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(211)))), ((int)(((byte)(236)))));
            this._account.Location = new System.Drawing.Point(42, 11);
            this._account.Name = "_account";
            this._account.Size = new System.Drawing.Size(75, 17);
            this._account.TabIndex = 38;
            this._account.Text = "User name";
            // 
            // P_Logo
            // 
            this.P_Logo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.P_Logo.Controls.Add(this.label1);
            this.P_Logo.Controls.Add(this.P_LogoContainer);
            this.P_Logo.Controls.Add(this.L_Company);
            this.P_Logo.Dock = System.Windows.Forms.DockStyle.Top;
            this.P_Logo.ForeColor = System.Drawing.Color.White;
            this.P_Logo.Location = new System.Drawing.Point(0, 27);
            this.P_Logo.Margin = new System.Windows.Forms.Padding(0);
            this.P_Logo.Name = "P_Logo";
            this.P_Logo.Size = new System.Drawing.Size(282, 171);
            this.P_Logo.TabIndex = 36;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(130)))), ((int)(((byte)(155)))));
            this.label1.Location = new System.Drawing.Point(0, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "STEAM API EMULATOR";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // L_Company
            // 
            this.L_Company.Font = new System.Drawing.Font("Segoe UI Emoji", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.L_Company.ForeColor = System.Drawing.Color.White;
            this.L_Company.Location = new System.Drawing.Point(1, 114);
            this.L_Company.Name = "L_Company";
            this.L_Company.Size = new System.Drawing.Size(279, 31);
            this.L_Company.TabIndex = 37;
            this.L_Company.Text = "SKYNET";
            this.L_Company.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 455);
            this.Controls.Add(this.P_Body);
            this.Controls.Add(this.P_Logo);
            this.Controls.Add(this.PN_Top);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.PN_Top.ResumeLayout(false);
            this.P_LogoContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            this.P_Body.ResumeLayout(false);
            this.P_Body.PerformLayout();
            this.P_Logo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PN_Top;
        private System.Windows.Forms.Panel P_LogoContainer;
        private System.Windows.Forms.PictureBox Logo;
        private System.Windows.Forms.Panel P_Body;
        private System.Windows.Forms.Label _password;
        private System.Windows.Forms.Label _account;
        private System.Windows.Forms.Panel P_Logo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label L_Company;
        private Controls.SKYNET_CloseBox CloseBox;
        private Controls.SKYNET_TextBox TB_UserName;
        private Controls.SKYNET_TextBox TB_Languaje;
        private Controls.SKYNET_TextBox TB_SteamId;
        private System.Windows.Forms.Label label2;
        private Controls.SKYNET_Button BT_Launch;
    }
}