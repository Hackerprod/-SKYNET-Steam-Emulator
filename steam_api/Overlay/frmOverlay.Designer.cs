namespace SKYNET.Overlay
{
    partial class frmOverlay
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
            this.PN_Container = new System.Windows.Forms.Panel();
            this.PB_Close = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Close)).BeginInit();
            this.SuspendLayout();
            // 
            // PN_Container
            // 
            this.PN_Container.AutoScroll = true;
            this.PN_Container.Location = new System.Drawing.Point(23, 49);
            this.PN_Container.Name = "PN_Container";
            this.PN_Container.Size = new System.Drawing.Size(654, 291);
            this.PN_Container.TabIndex = 3;
            // 
            // PB_Close
            // 
            this.PB_Close.Image = global::SKYNET.Properties.Resources.close;
            this.PB_Close.Location = new System.Drawing.Point(659, 23);
            this.PB_Close.Name = "PB_Close";
            this.PB_Close.Size = new System.Drawing.Size(18, 18);
            this.PB_Close.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PB_Close.TabIndex = 2;
            this.PB_Close.TabStop = false;
            this.PB_Close.Click += new System.EventHandler(this.PB_Close_Click);
            this.PB_Close.MouseLeave += new System.EventHandler(this.PB_Close_MouseLeave);
            this.PB_Close.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PB_Close_MouseMove);
            // 
            // frmOverlay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(700, 363);
            this.Controls.Add(this.PN_Container);
            this.Controls.Add(this.PB_Close);
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmOverlay";
            this.Opacity = 0.9D;
            this.Padding = new System.Windows.Forms.Padding(20);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "v";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.FrmOverlay_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Event_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Event_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Event_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Close)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox PB_Close;
        private System.Windows.Forms.Panel PN_Container;
    }
}