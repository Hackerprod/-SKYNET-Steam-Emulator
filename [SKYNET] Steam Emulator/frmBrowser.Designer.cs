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
            this.PN_WebContainer = new System.Windows.Forms.Panel();
            this.BT_JSFunction = new System.Windows.Forms.Button();
            this.TB_Message = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.TB_Message);
            this.panel1.Controls.Add(this.BT_JSFunction);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 50);
            this.panel1.TabIndex = 0;
            // 
            // PN_WebContainer
            // 
            this.PN_WebContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PN_WebContainer.Location = new System.Drawing.Point(0, 50);
            this.PN_WebContainer.Name = "PN_WebContainer";
            this.PN_WebContainer.Size = new System.Drawing.Size(800, 400);
            this.PN_WebContainer.TabIndex = 1;
            // 
            // BT_JSFunction
            // 
            this.BT_JSFunction.Location = new System.Drawing.Point(178, 12);
            this.BT_JSFunction.Name = "BT_JSFunction";
            this.BT_JSFunction.Size = new System.Drawing.Size(113, 23);
            this.BT_JSFunction.TabIndex = 0;
            this.BT_JSFunction.Text = "Call JS Function";
            this.BT_JSFunction.UseVisualStyleBackColor = true;
            this.BT_JSFunction.Click += new System.EventHandler(this.BT_JSFunction_Click);
            // 
            // TB_Message
            // 
            this.TB_Message.Location = new System.Drawing.Point(12, 14);
            this.TB_Message.Name = "TB_Message";
            this.TB_Message.Size = new System.Drawing.Size(160, 20);
            this.TB_Message.TabIndex = 0;
            this.TB_Message.Text = "SKYNET Message";
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel PN_WebContainer;
        private System.Windows.Forms.Button BT_JSFunction;
        private System.Windows.Forms.TextBox TB_Message;
    }
}