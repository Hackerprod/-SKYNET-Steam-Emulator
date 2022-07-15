using System.Windows.Forms;

namespace SKYNET.GUI.Controls
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
            this.PB_Avatar = new PictureBox();
            this.LB_Name = new Label();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_Avatar
            // 
            this.PB_Avatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.PB_Avatar.Cursor = Cursors.Hand;
            this.PB_Avatar.Location = new System.Drawing.Point(9, 4);
            this.PB_Avatar.Name = "PB_Avatar";
            this.PB_Avatar.Size = new System.Drawing.Size(30, 30);
            this.PB_Avatar.SizeMode = PictureBoxSizeMode.StretchImage;
            this.PB_Avatar.TabIndex = 0;
            this.PB_Avatar.TabStop = false;
            this.PB_Avatar.MouseClick += new MouseEventHandler(this.Box_Clicked);
            this.PB_Avatar.MouseDoubleClick += new MouseEventHandler(this.Box_DoubleClicked);
            this.PB_Avatar.MouseLeave += new System.EventHandler(this.Avatar_MouseLeave);
            this.PB_Avatar.MouseMove += new MouseEventHandler(this.Avatar_MouseMove);
            // 
            // LB_Name
            // 
            this.LB_Name.Cursor = Cursors.Hand;
            this.LB_Name.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.LB_Name.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.LB_Name.Location = new System.Drawing.Point(45, 6);
            this.LB_Name.Name = "LB_Name";
            this.LB_Name.Size = new System.Drawing.Size(184, 25);
            this.LB_Name.TabIndex = 1;
            this.LB_Name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_Name.MouseClick += new MouseEventHandler(this.Box_Clicked);
            this.LB_Name.MouseDoubleClick += new MouseEventHandler(this.Box_DoubleClicked);
            this.LB_Name.MouseLeave += new System.EventHandler(this.Avatar_MouseLeave);
            this.LB_Name.MouseMove += new MouseEventHandler(this.Avatar_MouseMove);
            // 
            // GameBox
            // 
            this.AutoScaleMode = AutoScaleMode.None;
            this.Controls.Add(this.LB_Name);
            this.Controls.Add(this.PB_Avatar);
            this.Cursor = Cursors.Hand;
            this.Name = "GameBox";
            this.Padding = new Padding(6);
            this.Size = new System.Drawing.Size(237, 38);
            this.MouseClick += new MouseEventHandler(this.Box_Clicked);
            this.MouseDoubleClick += new MouseEventHandler(this.Box_DoubleClicked);
            this.MouseLeave += new System.EventHandler(this.Avatar_MouseLeave);
            this.MouseMove += new MouseEventHandler(this.Avatar_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox PB_Avatar;
        public Label LB_Name;
    }
}
