namespace SKYNET.Controls
{
    partial class SKYNET_TextBox
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
            this.P_Container = new System.Windows.Forms.Panel();
            this.textBox = new System.Windows.Forms.TextBox();
            this.logo_box = new System.Windows.Forms.PictureBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.PN_Top = new System.Windows.Forms.Panel();
            this.Container = new System.Windows.Forms.Panel();
            this.P_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo_box)).BeginInit();
            this.Container.SuspendLayout();
            this.SuspendLayout();
            // 
            // P_Container
            // 
            this.P_Container.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.P_Container.Controls.Add(this.textBox);
            this.P_Container.Controls.Add(this.logo_box);
            this.P_Container.Controls.Add(this.panel4);
            this.P_Container.Controls.Add(this.panel3);
            this.P_Container.Controls.Add(this.panel2);
            this.P_Container.Controls.Add(this.PN_Top);
            this.P_Container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P_Container.Location = new System.Drawing.Point(1, 1);
            this.P_Container.Name = "P_Container";
            this.P_Container.Size = new System.Drawing.Size(218, 33);
            this.P_Container.TabIndex = 0;
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.textBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.textBox.Location = new System.Drawing.Point(10, 6);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(177, 18);
            this.textBox.TabIndex = 93;
            this.textBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClick);
            // 
            // logo_box
            // 
            this.logo_box.BackColor = System.Drawing.Color.Transparent;
            this.logo_box.Dock = System.Windows.Forms.DockStyle.Right;
            this.logo_box.Location = new System.Drawing.Point(187, 6);
            this.logo_box.Name = "logo_box";
            this.logo_box.Size = new System.Drawing.Size(21, 21);
            this.logo_box.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logo_box.TabIndex = 5;
            this.logo_box.TabStop = false;
            this.logo_box.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LogoClick);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(208, 6);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(10, 21);
            this.panel4.TabIndex = 4;
            this.panel4.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClick);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 6);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(10, 21);
            this.panel3.TabIndex = 3;
            this.panel3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClick);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 27);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(218, 6);
            this.panel2.TabIndex = 2;
            this.panel2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClick);
            // 
            // PN_Top
            // 
            this.PN_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.PN_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.PN_Top.Location = new System.Drawing.Point(0, 0);
            this.PN_Top.Name = "PN_Top";
            this.PN_Top.Size = new System.Drawing.Size(218, 6);
            this.PN_Top.TabIndex = 1;
            this.PN_Top.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClick);
            // 
            // Container
            // 
            this.Container.Controls.Add(this.P_Container);
            this.Container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Container.Location = new System.Drawing.Point(0, 0);
            this.Container.Name = "Container";
            this.Container.Padding = new System.Windows.Forms.Padding(1);
            this.Container.Size = new System.Drawing.Size(220, 35);
            this.Container.TabIndex = 94;
            // 
            // SKYNET_TextBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.Container);
            this.Name = "SKYNET_TextBox";
            this.Size = new System.Drawing.Size(220, 35);
            this.P_Container.ResumeLayout(false);
            this.P_Container.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo_box)).EndInit();
            this.Container.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel P_Container;
        private System.Windows.Forms.Panel PN_Top;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.PictureBox logo_box;
        public System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Panel Container;
    }
}
