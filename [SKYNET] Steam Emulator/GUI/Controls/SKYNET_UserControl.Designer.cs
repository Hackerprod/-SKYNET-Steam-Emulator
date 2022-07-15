using System.Windows.Forms;

namespace SKYNET.GUI.Controls
{
    partial class SKYNET_UserControl
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
            this.PB_Avatar = new PictureBox();
            this.LB_PersonaName = new Label();
            this.LB_IPAddress = new Label();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_Avatar
            // 
            this.PB_Avatar.Location = new System.Drawing.Point(6, 5);
            this.PB_Avatar.Name = "PB_Avatar";
            this.PB_Avatar.Size = new System.Drawing.Size(30, 30);
            this.PB_Avatar.SizeMode = PictureBoxSizeMode.StretchImage;
            this.PB_Avatar.TabIndex = 0;
            this.PB_Avatar.TabStop = false;
            this.PB_Avatar.MouseLeave += new System.EventHandler(this.SKYNET_UserControl_MouseLeave);
            this.PB_Avatar.MouseMove += new MouseEventHandler(this.SKYNET_UserControl_MouseMove);
            // 
            // LB_PersonaName
            // 
            this.LB_PersonaName.AutoSize = true;
            this.LB_PersonaName.Font = new System.Drawing.Font("Segoe UI Emoji", 10.25F);
            this.LB_PersonaName.ForeColor = System.Drawing.Color.White;
            this.LB_PersonaName.Location = new System.Drawing.Point(42, 3);
            this.LB_PersonaName.Name = "LB_PersonaName";
            this.LB_PersonaName.Size = new System.Drawing.Size(37, 19);
            this.LB_PersonaName.TabIndex = 1;
            this.LB_PersonaName.Text = "User";
            this.LB_PersonaName.MouseLeave += new System.EventHandler(this.SKYNET_UserControl_MouseLeave);
            this.LB_PersonaName.MouseMove += new MouseEventHandler(this.SKYNET_UserControl_MouseMove);
            // 
            // LB_IPAddress
            // 
            this.LB_IPAddress.AutoSize = true;
            this.LB_IPAddress.Font = new System.Drawing.Font("Segoe UI Emoji", 8F);
            this.LB_IPAddress.ForeColor = System.Drawing.Color.White;
            this.LB_IPAddress.Location = new System.Drawing.Point(42, 20);
            this.LB_IPAddress.Name = "LB_IPAddress";
            this.LB_IPAddress.Size = new System.Drawing.Size(38, 15);
            this.LB_IPAddress.TabIndex = 2;
            this.LB_IPAddress.Text = "label2";
            this.LB_IPAddress.MouseLeave += new System.EventHandler(this.SKYNET_UserControl_MouseLeave);
            this.LB_IPAddress.MouseMove += new MouseEventHandler(this.SKYNET_UserControl_MouseMove);
            // 
            // SKYNET_UserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(43)))), ((int)(((byte)(53)))));
            this.Controls.Add(this.LB_IPAddress);
            this.Controls.Add(this.LB_PersonaName);
            this.Controls.Add(this.PB_Avatar);
            this.Name = "SKYNET_UserControl";
            this.Size = new System.Drawing.Size(167, 40);
            this.MouseLeave += new System.EventHandler(this.SKYNET_UserControl_MouseLeave);
            this.MouseMove += new MouseEventHandler(this.SKYNET_UserControl_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox PB_Avatar;
        private Label LB_PersonaName;
        private Label LB_IPAddress;
    }
}
