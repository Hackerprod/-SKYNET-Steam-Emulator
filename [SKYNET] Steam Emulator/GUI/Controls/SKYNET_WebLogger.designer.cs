namespace SKYNET.Controls
{
    partial class SKYNET_WebLogger
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
            this.webChat = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webChat
            // 
            this.webChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webChat.IsWebBrowserContextMenuEnabled = false;
            this.webChat.Location = new System.Drawing.Point(0, 0);
            this.webChat.MinimumSize = new System.Drawing.Size(20, 20);
            this.webChat.Name = "webChat";
            this.webChat.ScriptErrorsSuppressed = true;
            this.webChat.Size = new System.Drawing.Size(582, 360);
            this.webChat.TabIndex = 8;
            // 
            // WebBrowserLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.webChat);
            this.Name = "WebBrowserLogger";
            this.Size = new System.Drawing.Size(582, 360);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.WebBrowser webChat;
    }
}
