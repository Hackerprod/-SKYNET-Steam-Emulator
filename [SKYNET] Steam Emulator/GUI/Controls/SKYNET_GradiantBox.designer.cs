using System.Windows.Forms;

namespace SKYNET.GUI.Controls
{
    partial class SKYNET_GradiantBox
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
            this.Box = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Box)).BeginInit();
            this.SuspendLayout();
            // 
            // Box
            // 
            this.Box.Dock = DockStyle.Fill;
            this.Box.Location = new System.Drawing.Point(0, 0);
            this.Box.Name = "Box";
            this.Box.Size = new System.Drawing.Size(150, 150);
            this.Box.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Box.TabIndex = 0;
            this.Box.TabStop = false;
            this.Box.MouseLeave += new System.EventHandler(this.Box_MouseLeave);
            this.Box.MouseMove += new MouseEventHandler(this.Box_MouseMove);
            // 
            // GradiantBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.Box);
            this.Name = "GradiantBox";
            ((System.ComponentModel.ISupportInitialize)(this.Box)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox Box;
    }
}
