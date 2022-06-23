


using SKYNET.Controls;
using SKYNET.Properties;

namespace SKYNET
{
    partial class frmUpdateProfile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdateProfile));
            this.panelTop = new System.Windows.Forms.Panel();
            this.acceptBtn = new System.Windows.Forms.Button();
            this.Browser = new System.Windows.Forms.WebBrowser();
            this.panelBody = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this._password = new System.Windows.Forms.Label();
            this.Password = new SKYNET.Controls.SKYNET_TextBox();
            this.UserName = new SKYNET.Controls.SKYNET_TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.b_Cancel = new SKYNET_Button();
            this.PB_Avatar = new CircularPictureBox();
            this.b_Apply = new SKYNET_Button();
            this._username = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.Logo = new System.Windows.Forms.PictureBox();
            this.TB_AccountID = new SKYNET.Controls.SKYNET_TextBox();
            this.panelBody.SuspendLayout();
            this.panelLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.ForeColor = System.Drawing.Color.White;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(393, 26);
            this.panelTop.TabIndex = 5;
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
            // panelBody
            // 
            this.panelBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.panelBody.Controls.Add(this.TB_AccountID);
            this.panelBody.Controls.Add(this.label1);
            this.panelBody.Controls.Add(this._password);
            this.panelBody.Controls.Add(this.Password);
            this.panelBody.Controls.Add(this.UserName);
            this.panelBody.Controls.Add(this.textBox1);
            this.panelBody.Controls.Add(this.b_Cancel);
            this.panelBody.Controls.Add(this.PB_Avatar);
            this.panelBody.Controls.Add(this.b_Apply);
            this.panelBody.Controls.Add(this._username);
            this.panelBody.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBody.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.panelBody.Location = new System.Drawing.Point(0, 26);
            this.panelBody.Name = "panelBody";
            this.panelBody.Padding = new System.Windows.Forms.Padding(8);
            this.panelBody.Size = new System.Drawing.Size(393, 267);
            this.panelBody.TabIndex = 34;
            this.panelBody.Click += new System.EventHandler(this.PanelBody_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(131)))), ((int)(((byte)(246)))));
            this.label1.Location = new System.Drawing.Point(160, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 17);
            this.label1.TabIndex = 70;
            this.label1.Text = "Account ID";
            // 
            // _password
            // 
            this._password.AutoSize = true;
            this._password.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this._password.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(131)))), ((int)(((byte)(246)))));
            this._password.Location = new System.Drawing.Point(160, 82);
            this._password.Name = "_password";
            this._password.Size = new System.Drawing.Size(66, 17);
            this._password.TabIndex = 68;
            this._password.Text = "Password";
            // 
            // Password
            // 
            this.Password.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.Password.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.Password.Color = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.Password.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.Password.IsPassword = true;
            this.Password.Location = new System.Drawing.Point(163, 102);
            this.Password.Logo = global::SKYNET.Properties.Resources.key_2_60px;
            this.Password.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.Password.Name = "Password";
            this.Password.OnlyNumbers = false;
            this.Password.Padding = new System.Windows.Forms.Padding(2);
            this.Password.ShowLogo = true;
            this.Password.Size = new System.Drawing.Size(211, 37);
            this.Password.TabIndex = 67;
            this.Password.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // UserName
            // 
            this.UserName.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.UserName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.UserName.Color = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.UserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.UserName.IsPassword = false;
            this.UserName.Location = new System.Drawing.Point(164, 40);
            this.UserName.Logo = global::SKYNET.Properties.Resources.male_user_100px;
            this.UserName.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.UserName.Name = "UserName";
            this.UserName.OnlyNumbers = false;
            this.UserName.Padding = new System.Windows.Forms.Padding(2);
            this.UserName.ShowLogo = true;
            this.UserName.Size = new System.Drawing.Size(211, 37);
            this.UserName.TabIndex = 66;
            this.UserName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(429, 81);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 23);
            this.textBox1.TabIndex = 63;
            // 
            // b_Cancel
            // 
            this.b_Cancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(46)))), ((int)(((byte)(88)))));
            this.b_Cancel.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.b_Cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.b_Cancel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.b_Cancel.ForeColor = System.Drawing.Color.White;
            this.b_Cancel.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.b_Cancel.ImageAlignment = SKYNET_Button._ImgAlign.Left;
            this.b_Cancel.ImageIcon = null;
            this.b_Cancel.Location = new System.Drawing.Point(274, 217);
            this.b_Cancel.MenuMode = false;
            this.b_Cancel.Name = "b_Cancel";
            this.b_Cancel.Rounded = false;
            this.b_Cancel.Size = new System.Drawing.Size(100, 28);
            this.b_Cancel.Style = SKYNET_Button._Style.TextOnly;
            this.b_Cancel.TabIndex = 61;
            this.b_Cancel.Text = "Cancelar";
            this.b_Cancel.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // PB_Avatar
            // 
            this.PB_Avatar.Image = global::SKYNET.Properties.Resources.profile_picture;
            this.PB_Avatar.Location = new System.Drawing.Point(14, 23);
            this.PB_Avatar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PB_Avatar.Name = "PB_Avatar";
            this.PB_Avatar.Size = new System.Drawing.Size(133, 133);
            this.PB_Avatar.TabIndex = 60;
            this.PB_Avatar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Avatar_MouseClick);
            // 
            // b_Apply
            // 
            this.b_Apply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(131)))), ((int)(((byte)(246)))));
            this.b_Apply.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.b_Apply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.b_Apply.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.b_Apply.ForeColor = System.Drawing.Color.White;
            this.b_Apply.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.b_Apply.ImageAlignment = SKYNET_Button._ImgAlign.Left;
            this.b_Apply.ImageIcon = null;
            this.b_Apply.Location = new System.Drawing.Point(163, 217);
            this.b_Apply.MenuMode = false;
            this.b_Apply.Name = "b_Apply";
            this.b_Apply.Rounded = false;
            this.b_Apply.Size = new System.Drawing.Size(100, 28);
            this.b_Apply.Style = SKYNET_Button._Style.TextOnly;
            this.b_Apply.TabIndex = 59;
            this.b_Apply.Text = "Aplicar";
            this.b_Apply.Click += new System.EventHandler(this.Apply_Button_Click);
            // 
            // _username
            // 
            this._username.AutoSize = true;
            this._username.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Bold);
            this._username.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(131)))), ((int)(((byte)(246)))));
            this._username.Location = new System.Drawing.Point(160, 20);
            this._username.Name = "_username";
            this._username.Size = new System.Drawing.Size(75, 17);
            this._username.TabIndex = 38;
            this._username.Text = "User name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(101, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 17);
            this.label6.TabIndex = 37;
            this.label6.Text = "Game Coordinator Client";
            // 
            // panelLogo
            // 
            this.panelLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(164)))), ((int)(((byte)(245)))));
            this.panelLogo.Controls.Add(this.label6);
            this.panelLogo.Controls.Add(this.Logo);
            this.panelLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogo.ForeColor = System.Drawing.Color.White;
            this.panelLogo.Location = new System.Drawing.Point(0, 26);
            this.panelLogo.Margin = new System.Windows.Forms.Padding(0);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(393, 0);
            this.panelLogo.TabIndex = 33;
            // 
            // Logo
            // 
            this.Logo.Location = new System.Drawing.Point(136, 7);
            this.Logo.Name = "Logo";
            this.Logo.Size = new System.Drawing.Size(105, 105);
            this.Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Logo.TabIndex = 1;
            this.Logo.TabStop = false;
            // 
            // TB_AccountID
            // 
            this.TB_AccountID.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.TB_AccountID.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.TB_AccountID.Color = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.TB_AccountID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.TB_AccountID.IsPassword = false;
            this.TB_AccountID.Location = new System.Drawing.Point(162, 168);
            this.TB_AccountID.Logo = global::SKYNET.Properties.Resources.steam_home_os;
            this.TB_AccountID.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_AccountID.Name = "TB_AccountID";
            this.TB_AccountID.OnlyNumbers = false;
            this.TB_AccountID.ShowLogo = true;
            this.TB_AccountID.Size = new System.Drawing.Size(213, 35);
            this.TB_AccountID.TabIndex = 71;
            this.TB_AccountID.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // frmUpdateProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(46)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(393, 293);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.panelLogo);
            this.Controls.Add(this.Browser);
            this.Controls.Add(this.acceptBtn);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI Emoji", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmUpdateProfile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "[SKYNET] Dota2 GCS";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.panelBody.ResumeLayout(false);
            this.panelBody.PerformLayout();
            this.panelLogo.ResumeLayout(false);
            this.panelLogo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button acceptBtn;
        private System.Windows.Forms.WebBrowser Browser;
        private System.Windows.Forms.Panel panelBody;
        private System.Windows.Forms.Label _username;
        private System.Windows.Forms.PictureBox Logo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panelLogo;
        private SKYNET_Button b_Apply;
        private CircularPictureBox PB_Avatar;
        private SKYNET_Button b_Cancel;
        private System.Windows.Forms.TextBox textBox1;
        public SKYNET_TextBox UserName;
        public SKYNET_TextBox Password;
        private System.Windows.Forms.Label _password;
        private System.Windows.Forms.Label label1;
        private SKYNET_TextBox TB_AccountID;
    }
}