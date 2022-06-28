
namespace SKYNET.GUI
{
    partial class frmPlayerNotify
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlayerNotify));
            this.panelTop = new System.Windows.Forms.Panel();
            this.acceptBtn = new System.Windows.Forms.Button();
            this.panelBody = new System.Windows.Forms.Panel();
            this.LB_PersonaName = new System.Windows.Forms.Label();
            this.LB_PlayingMsg = new System.Windows.Forms.Label();
            this.Avatar = new CircularPictureBox();
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.Logo = new System.Windows.Forms.PictureBox();
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
            this.panelTop.Size = new System.Drawing.Size(243, 20);
            this.panelTop.TabIndex = 5;
            this.panelTop.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseClick);
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
            // panelBody
            // 
            this.panelBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.panelBody.Controls.Add(this.LB_PersonaName);
            this.panelBody.Controls.Add(this.LB_PlayingMsg);
            this.panelBody.Controls.Add(this.Avatar);
            this.panelBody.Controls.Add(this.ErrorLabel);
            this.panelBody.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBody.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.panelBody.Location = new System.Drawing.Point(0, 20);
            this.panelBody.Name = "panelBody";
            this.panelBody.Padding = new System.Windows.Forms.Padding(8);
            this.panelBody.Size = new System.Drawing.Size(243, 73);
            this.panelBody.TabIndex = 34;
            this.panelBody.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseClick);
            // 
            // LB_PersonaName
            // 
            this.LB_PersonaName.AutoSize = true;
            this.LB_PersonaName.Font = new System.Drawing.Font("Malgun Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_PersonaName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(131)))), ((int)(((byte)(246)))));
            this.LB_PersonaName.Location = new System.Drawing.Point(89, 9);
            this.LB_PersonaName.Name = "LB_PersonaName";
            this.LB_PersonaName.Size = new System.Drawing.Size(91, 20);
            this.LB_PersonaName.TabIndex = 85;
            this.LB_PersonaName.Text = "Hackerprod";
            this.LB_PersonaName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseClick);
            // 
            // LB_PlayingMsg
            // 
            this.LB_PlayingMsg.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.LB_PlayingMsg.ForeColor = System.Drawing.Color.White;
            this.LB_PlayingMsg.Location = new System.Drawing.Point(91, 31);
            this.LB_PlayingMsg.Name = "LB_PlayingMsg";
            this.LB_PlayingMsg.Size = new System.Drawing.Size(140, 34);
            this.LB_PlayingMsg.TabIndex = 84;
            this.LB_PlayingMsg.Text = "Is playing Dota 2 ";
            this.LB_PlayingMsg.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseClick);
            // 
            // Avatar
            // 
            this.Avatar.Image = global::SKYNET.Properties.Resources.profile_picture;
            this.Avatar.Location = new System.Drawing.Point(16, 10);
            this.Avatar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Avatar.Name = "Avatar";
            this.Avatar.Size = new System.Drawing.Size(60, 60);
            this.Avatar.TabIndex = 60;
            this.Avatar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Controls_MouseClick);
            // 
            // ErrorLabel
            // 
            this.ErrorLabel.AutoSize = true;
            this.ErrorLabel.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorLabel.ForeColor = System.Drawing.Color.Red;
            this.ErrorLabel.Location = new System.Drawing.Point(63, 199);
            this.ErrorLabel.Name = "ErrorLabel";
            this.ErrorLabel.Size = new System.Drawing.Size(0, 15);
            this.ErrorLabel.TabIndex = 56;
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
            this.panelLogo.Location = new System.Drawing.Point(0, 20);
            this.panelLogo.Margin = new System.Windows.Forms.Padding(0);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(243, 0);
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
            // frmPlayerNotify
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(46)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(243, 100);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.panelLogo);
            this.Controls.Add(this.acceptBtn);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI Emoji", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmPlayerNotify";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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
        private System.Windows.Forms.Panel panelBody;
        private System.Windows.Forms.PictureBox Logo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Label ErrorLabel;
        private CircularPictureBox Avatar;
        private System.Windows.Forms.Label LB_PlayingMsg;
        private System.Windows.Forms.Label LB_PersonaName;
    }
}