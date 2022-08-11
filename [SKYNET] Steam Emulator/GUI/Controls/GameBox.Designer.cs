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
            this.PB_Avatar = new System.Windows.Forms.PictureBox();
            this.LB_Name = new System.Windows.Forms.Label();
            this.LB_Time = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_Avatar
            // 
            this.PB_Avatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.PB_Avatar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PB_Avatar.Location = new System.Drawing.Point(15, 6);
            this.PB_Avatar.Name = "PB_Avatar";
            this.PB_Avatar.Size = new System.Drawing.Size(26, 26);
            this.PB_Avatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PB_Avatar.TabIndex = 0;
            this.PB_Avatar.TabStop = false;
            this.PB_Avatar.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Box_Clicked);
            this.PB_Avatar.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Box_DoubleClicked);
            this.PB_Avatar.MouseLeave += new System.EventHandler(this.Avatar_MouseLeave);
            this.PB_Avatar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Avatar_MouseMove);
            // 
            // LB_Name
            // 
            this.LB_Name.BackColor = System.Drawing.Color.Transparent;
            this.LB_Name.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_Name.Font = new System.Drawing.Font("Segoe UI Emoji", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Name.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.LB_Name.Location = new System.Drawing.Point(47, 5);
            this.LB_Name.Name = "LB_Name";
            this.LB_Name.Size = new System.Drawing.Size(182, 15);
            this.LB_Name.TabIndex = 1;
            this.LB_Name.Text = "Done &";
            this.LB_Name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_Name.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Box_Clicked);
            this.LB_Name.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Box_DoubleClicked);
            this.LB_Name.MouseLeave += new System.EventHandler(this.Avatar_MouseLeave);
            this.LB_Name.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Avatar_MouseMove);
            // 
            // LB_Time
            // 
            this.LB_Time.AutoSize = true;
            this.LB_Time.BackColor = System.Drawing.Color.Transparent;
            this.LB_Time.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LB_Time.Font = new System.Drawing.Font("Trebuchet MS", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Time.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.LB_Time.Location = new System.Drawing.Point(47, 19);
            this.LB_Time.Name = "LB_Time";
            this.LB_Time.Size = new System.Drawing.Size(63, 15);
            this.LB_Time.TabIndex = 2;
            this.LB_Time.Text = "Never played";
            this.LB_Time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_Time.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Box_Clicked);
            this.LB_Time.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Box_DoubleClicked);
            // 
            // GameBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.LB_Time);
            this.Controls.Add(this.LB_Name);
            this.Controls.Add(this.PB_Avatar);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "GameBox";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.Size = new System.Drawing.Size(237, 38);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Box_Clicked);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Box_DoubleClicked);
            this.MouseLeave += new System.EventHandler(this.Avatar_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Avatar_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox PB_Avatar;
        public Label LB_Name;
        public Label LB_Time;
    }
}
