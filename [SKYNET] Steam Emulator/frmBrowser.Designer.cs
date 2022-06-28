namespace SKYNET
{
    partial class frmBrowser
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.TB_Url = new System.Windows.Forms.TextBox();
            this.BT_Go = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TB_Message = new System.Windows.Forms.TextBox();
            this.BT_JSFunction = new System.Windows.Forms.Button();
            this.PN_WebContainer = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.TB_Url);
            this.panel1.Controls.Add(this.BT_Go);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 50);
            this.panel1.TabIndex = 0;
            this.panel1.Visible = false;
            // 
            // TB_Url
            // 
            this.TB_Url.Location = new System.Drawing.Point(12, 16);
            this.TB_Url.Name = "TB_Url";
            this.TB_Url.Size = new System.Drawing.Size(453, 20);
            this.TB_Url.TabIndex = 2;
            // 
            // BT_Go
            // 
            this.BT_Go.Location = new System.Drawing.Point(471, 14);
            this.BT_Go.Name = "BT_Go";
            this.BT_Go.Size = new System.Drawing.Size(53, 23);
            this.BT_Go.TabIndex = 3;
            this.BT_Go.Text = "GO";
            this.BT_Go.UseVisualStyleBackColor = true;
            this.BT_Go.Click += new System.EventHandler(this.BT_Go_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.TB_Message);
            this.panel2.Controls.Add(this.BT_JSFunction);
            this.panel2.Location = new System.Drawing.Point(530, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(262, 41);
            this.panel2.TabIndex = 1;
            // 
            // TB_Message
            // 
            this.TB_Message.Location = new System.Drawing.Point(8, 11);
            this.TB_Message.Name = "TB_Message";
            this.TB_Message.Size = new System.Drawing.Size(129, 20);
            this.TB_Message.TabIndex = 0;
            this.TB_Message.Text = "SKYNET Message";
            // 
            // BT_JSFunction
            // 
            this.BT_JSFunction.Location = new System.Drawing.Point(141, 9);
            this.BT_JSFunction.Name = "BT_JSFunction";
            this.BT_JSFunction.Size = new System.Drawing.Size(113, 23);
            this.BT_JSFunction.TabIndex = 0;
            this.BT_JSFunction.Text = "Call JS Function";
            this.BT_JSFunction.UseVisualStyleBackColor = true;
            this.BT_JSFunction.Click += new System.EventHandler(this.BT_JSFunction_Click);
            // 
            // PN_WebContainer
            // 
            this.PN_WebContainer.BackColor = System.Drawing.Color.White;
            this.PN_WebContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PN_WebContainer.Location = new System.Drawing.Point(0, 50);
            this.PN_WebContainer.Name = "PN_WebContainer";
            this.PN_WebContainer.Size = new System.Drawing.Size(800, 400);
            this.PN_WebContainer.TabIndex = 1;
            // 
            // frmBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.PN_WebContainer);
            this.Controls.Add(this.panel1);
            this.Name = "frmBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmBrowser";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmBrowser_FormClosing);
            this.Load += new System.EventHandler(this.FrmBrowser_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel PN_WebContainer;
        private System.Windows.Forms.Button BT_JSFunction;
        private System.Windows.Forms.TextBox TB_Message;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox TB_Url;
        private System.Windows.Forms.Button BT_Go;
    }
}