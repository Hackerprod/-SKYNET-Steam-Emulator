using System.Windows.Forms;

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
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }
            catch (System.Exception)
            {
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.panel1 = new System.Windows.Forms.Panel();
            this.SKYNET_MinimizeBox = new SKYNET.GUI.Controls.SKYNET_MinimizeBox();
            this.SKYNET_CloseBox = new SKYNET.GUI.Controls.SKYNET_CloseBox();
            this.WebContainer = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.panel1.Controls.Add(this.SKYNET_MinimizeBox);
            this.panel1.Controls.Add(this.SKYNET_CloseBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(895, 28);
            this.panel1.TabIndex = 0;
            // 
            // SKYNET_MinimizeBox
            // 
            this.SKYNET_MinimizeBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.SKYNET_MinimizeBox.Color = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.SKYNET_MinimizeBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.SKYNET_MinimizeBox.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.SKYNET_MinimizeBox.Location = new System.Drawing.Point(827, 0);
            this.SKYNET_MinimizeBox.MaximumSize = new System.Drawing.Size(34, 26);
            this.SKYNET_MinimizeBox.MinimumSize = new System.Drawing.Size(34, 26);
            this.SKYNET_MinimizeBox.Name = "SKYNET_MinimizeBox";
            this.SKYNET_MinimizeBox.Size = new System.Drawing.Size(34, 26);
            this.SKYNET_MinimizeBox.TabIndex = 1;
            this.SKYNET_MinimizeBox.Clicked += new System.EventHandler(this.MinimizeBox_Clicked);
            // 
            // SKYNET_CloseBox
            // 
            this.SKYNET_CloseBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.SKYNET_CloseBox.Color = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.SKYNET_CloseBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.SKYNET_CloseBox.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
            this.SKYNET_CloseBox.Location = new System.Drawing.Point(861, 0);
            this.SKYNET_CloseBox.MaximumSize = new System.Drawing.Size(34, 26);
            this.SKYNET_CloseBox.MinimumSize = new System.Drawing.Size(34, 26);
            this.SKYNET_CloseBox.Name = "SKYNET_CloseBox";
            this.SKYNET_CloseBox.Size = new System.Drawing.Size(34, 26);
            this.SKYNET_CloseBox.TabIndex = 0;
            this.SKYNET_CloseBox.Clicked += new System.EventHandler(this.CloseBox_Clicked);
            // 
            // WebContainer
            // 
            this.WebContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.WebContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebContainer.Location = new System.Drawing.Point(0, 28);
            this.WebContainer.Name = "WebContainer";
            this.WebContainer.Size = new System.Drawing.Size(895, 404);
            this.WebContainer.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 432);
            this.Controls.Add(this.WebContainer);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(1366, 728);
            this.Name = "frmMain";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TransparencyKey = System.Drawing.Color.Azure;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmBrowser_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Controls.SKYNET_CloseBox SKYNET_CloseBox;
        private Controls.SKYNET_MinimizeBox SKYNET_MinimizeBox;
        private Panel WebContainer;
    }
}