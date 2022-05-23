namespace SKYNET.Overlay
{
    partial class SteamUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LB_PersonaName = new System.Windows.Forms.Label();
            this.LB_AppID = new System.Windows.Forms.Label();
            this.LB_AccountID = new System.Windows.Forms.Label();
            this.LB_IPAddress = new System.Windows.Forms.Label();
            this.PB_Avatar = new System.Windows.Forms.PictureBox();
            this.LB_Invite = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).BeginInit();
            this.SuspendLayout();
            // 
            // LB_PersonaName
            // 
            this.LB_PersonaName.AutoSize = true;
            this.LB_PersonaName.Font = new System.Drawing.Font("Tahoma", 11.25F);
            this.LB_PersonaName.ForeColor = System.Drawing.Color.White;
            this.LB_PersonaName.Location = new System.Drawing.Point(77, 7);
            this.LB_PersonaName.Name = "LB_PersonaName";
            this.LB_PersonaName.Size = new System.Drawing.Size(80, 18);
            this.LB_PersonaName.TabIndex = 1;
            this.LB_PersonaName.Text = "SteamUser";
            this.LB_PersonaName.MouseLeave += new System.EventHandler(this.SteamUserControl_MouseLeave);
            this.LB_PersonaName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SteamUserControl_MouseMove);
            // 
            // LB_AppID
            // 
            this.LB_AppID.AutoSize = true;
            this.LB_AppID.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.LB_AppID.Location = new System.Drawing.Point(77, 42);
            this.LB_AppID.Name = "LB_AppID";
            this.LB_AppID.Size = new System.Drawing.Size(83, 13);
            this.LB_AppID.TabIndex = 2;
            this.LB_AppID.Text = "Playing AppID 0";
            this.LB_AppID.MouseLeave += new System.EventHandler(this.SteamUserControl_MouseLeave);
            this.LB_AppID.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SteamUserControl_MouseMove);
            // 
            // LB_AccountID
            // 
            this.LB_AccountID.AutoSize = true;
            this.LB_AccountID.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.LB_AccountID.Location = new System.Drawing.Point(77, 27);
            this.LB_AccountID.Name = "LB_AccountID";
            this.LB_AccountID.Size = new System.Drawing.Size(84, 13);
            this.LB_AccountID.TabIndex = 4;
            this.LB_AccountID.Text = "AccountID 0000";
            this.LB_AccountID.MouseLeave += new System.EventHandler(this.SteamUserControl_MouseLeave);
            this.LB_AccountID.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SteamUserControl_MouseMove);
            // 
            // LB_IPAddress
            // 
            this.LB_IPAddress.AutoSize = true;
            this.LB_IPAddress.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.LB_IPAddress.Location = new System.Drawing.Point(77, 58);
            this.LB_IPAddress.Name = "LB_IPAddress";
            this.LB_IPAddress.Size = new System.Drawing.Size(107, 13);
            this.LB_IPAddress.TabIndex = 5;
            this.LB_IPAddress.Text = "IPAddress 127.0.0.1";
            this.LB_IPAddress.MouseLeave += new System.EventHandler(this.SteamUserControl_MouseLeave);
            this.LB_IPAddress.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SteamUserControl_MouseMove);
            // 
            // PB_Avatar
            // 
            this.PB_Avatar.Location = new System.Drawing.Point(6, 9);
            this.PB_Avatar.Name = "PB_Avatar";
            this.PB_Avatar.Size = new System.Drawing.Size(64, 64);
            this.PB_Avatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PB_Avatar.TabIndex = 0;
            this.PB_Avatar.TabStop = false;
            this.PB_Avatar.MouseLeave += new System.EventHandler(this.SteamUserControl_MouseLeave);
            this.PB_Avatar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SteamUserControl_MouseMove);
            // 
            // LB_Invite
            // 
            this.LB_Invite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_Invite.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.LB_Invite.Location = new System.Drawing.Point(229, 46);
            this.LB_Invite.Name = "LB_Invite";
            this.LB_Invite.Size = new System.Drawing.Size(60, 25);
            this.LB_Invite.TabIndex = 3;
            this.LB_Invite.Text = "Invite";
            this.LB_Invite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LB_Invite.MouseLeave += new System.EventHandler(this.LB_Invite_MouseLeave);
            this.LB_Invite.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LB_Invite_MouseMove);
            // 
            // SteamUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.Controls.Add(this.LB_Invite);
            this.Controls.Add(this.LB_IPAddress);
            this.Controls.Add(this.LB_AccountID);
            this.Controls.Add(this.LB_AppID);
            this.Controls.Add(this.LB_PersonaName);
            this.Controls.Add(this.PB_Avatar);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "SteamUserControl";
            this.Size = new System.Drawing.Size(300, 80);
            this.MouseLeave += new System.EventHandler(this.SteamUserControl_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SteamUserControl_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PB_Avatar;
        private System.Windows.Forms.Label LB_PersonaName;
        private System.Windows.Forms.Label LB_AppID;
        private System.Windows.Forms.Label LB_AccountID;
        private System.Windows.Forms.Label LB_IPAddress;
        private System.Windows.Forms.Label LB_Invite;
    }
}
