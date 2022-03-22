namespace SKYNET
{
    partial class GameBox
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
            this.Avatar = new System.Windows.Forms.PictureBox();
            this.LB_Name = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Avatar)).BeginInit();
            this.SuspendLayout();
            // 
            // Avatar
            // 
            this.Avatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.Avatar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Avatar.Location = new System.Drawing.Point(9, 4);
            this.Avatar.Name = "Avatar";
            this.Avatar.Size = new System.Drawing.Size(30, 30);
            this.Avatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Avatar.TabIndex = 0;
            this.Avatar.TabStop = false;
            this.Avatar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Box_Clicked);
            this.Avatar.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Box_DoubleClicked);
            this.Avatar.MouseLeave += new System.EventHandler(this.Avatar_MouseLeave);
            this.Avatar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Avatar_MouseMove);
            // 
            // LB_Name
            // 
            this.LB_Name.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_Name.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.LB_Name.Location = new System.Drawing.Point(45, 6);
            this.LB_Name.Name = "LB_Name";
            this.LB_Name.Size = new System.Drawing.Size(184, 25);
            this.LB_Name.TabIndex = 1;
            this.LB_Name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_Name.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Box_Clicked);
            this.LB_Name.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Box_DoubleClicked);
            this.LB_Name.MouseLeave += new System.EventHandler(this.Avatar_MouseLeave);
            this.LB_Name.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Avatar_MouseMove);
            // 
            // GameBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.LB_Name);
            this.Controls.Add(this.Avatar);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "GameBox";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.Size = new System.Drawing.Size(237, 38);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Box_Clicked);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Box_DoubleClicked);
            this.MouseLeave += new System.EventHandler(this.Avatar_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Avatar_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.Avatar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Avatar;
        private System.Windows.Forms.Label LB_Name;
    }
}
