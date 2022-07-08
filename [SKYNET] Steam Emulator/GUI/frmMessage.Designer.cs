using SKYNET.Controls;

namespace SKYNET.GUI
{
    partial class frmMessage
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
            this.PN_Top = new SKYNET.Controls.GradiantBox();
            this.PN_Bottom = new System.Windows.Forms.Panel();
            this.PN_Right = new System.Windows.Forms.Panel();
            this.PN_Left = new System.Windows.Forms.Panel();
            this.PN_Container = new System.Windows.Forms.Panel();
            this.cancel = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.BT_Cancel = new SKYNET_Button();
            this.BT_Ok = new SKYNET_Button();
            this.LB_Message = new System.Windows.Forms.Label();
            this.PN_Container.SuspendLayout();
            this.SuspendLayout();
            // 
            // PN_Top
            // 
            this.PN_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.PN_Top.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(115)))), ((int)(((byte)(139)))));
            this.PN_Top.Location = new System.Drawing.Point(0, 0);
            this.PN_Top.Mode = SKYNET.Controls.Mode.Horizontal;
            this.PN_Top.Name = "PN_Top";
            this.PN_Top.RigthColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(69)))), ((int)(((byte)(120)))));
            this.PN_Top.Size = new System.Drawing.Size(523, 2);
            this.PN_Top.TabIndex = 0;
            // 
            // PN_Bottom
            // 
            this.PN_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(60)))), ((int)(((byte)(67)))));
            this.PN_Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PN_Bottom.Location = new System.Drawing.Point(0, 232);
            this.PN_Bottom.Name = "PN_Bottom";
            this.PN_Bottom.Size = new System.Drawing.Size(523, 1);
            this.PN_Bottom.TabIndex = 1;
            // 
            // PN_Right
            // 
            this.PN_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(60)))), ((int)(((byte)(67)))));
            this.PN_Right.Dock = System.Windows.Forms.DockStyle.Right;
            this.PN_Right.Location = new System.Drawing.Point(522, 2);
            this.PN_Right.Name = "PN_Right";
            this.PN_Right.Size = new System.Drawing.Size(1, 230);
            this.PN_Right.TabIndex = 2;
            // 
            // PN_Left
            // 
            this.PN_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(60)))), ((int)(((byte)(67)))));
            this.PN_Left.Dock = System.Windows.Forms.DockStyle.Left;
            this.PN_Left.Location = new System.Drawing.Point(0, 2);
            this.PN_Left.Name = "PN_Left";
            this.PN_Left.Size = new System.Drawing.Size(1, 230);
            this.PN_Left.TabIndex = 3;
            // 
            // PN_Container
            // 
            this.PN_Container.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.PN_Container.Controls.Add(this.cancel);
            this.PN_Container.Controls.Add(this.ok);
            this.PN_Container.Controls.Add(this.BT_Cancel);
            this.PN_Container.Controls.Add(this.BT_Ok);
            this.PN_Container.Controls.Add(this.LB_Message);
            this.PN_Container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PN_Container.Location = new System.Drawing.Point(1, 2);
            this.PN_Container.Name = "PN_Container";
            this.PN_Container.Size = new System.Drawing.Size(521, 230);
            this.PN_Container.TabIndex = 4;
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(-10, -10);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(0, 0);
            this.cancel.TabIndex = 4;
            this.cancel.Text = "cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // ok
            // 
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Location = new System.Drawing.Point(-10, -10);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(0, 0);
            this.ok.TabIndex = 3;
            this.ok.Text = "ok";
            this.ok.UseVisualStyleBackColor = true;
            // 
            // BT_Cancel
            // 
            this.BT_Cancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(46)))), ((int)(((byte)(88)))));
            this.BT_Cancel.BackColorMouseOver = System.Drawing.Color.Empty;
            this.BT_Cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BT_Cancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BT_Cancel.ForeColor = System.Drawing.Color.White;
            this.BT_Cancel.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.BT_Cancel.ImageAlignment = SKYNET_Button._ImgAlign.Left;
            this.BT_Cancel.ImageIcon = null;
            this.BT_Cancel.Location = new System.Drawing.Point(223, 174);
            this.BT_Cancel.MenuMode = false;
            this.BT_Cancel.Name = "BT_Cancel";
            this.BT_Cancel.Rounded = false;
            this.BT_Cancel.Size = new System.Drawing.Size(115, 32);
            this.BT_Cancel.Style = SKYNET_Button._Style.TextOnly;
            this.BT_Cancel.TabIndex = 2;
            this.BT_Cancel.Text = "CANCEL";
            this.BT_Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // BT_Ok
            // 
            this.BT_Ok.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(131)))), ((int)(((byte)(246)))));
            this.BT_Ok.BackColorMouseOver = System.Drawing.Color.Empty;
            this.BT_Ok.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BT_Ok.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BT_Ok.ForeColor = System.Drawing.Color.White;
            this.BT_Ok.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.BT_Ok.ImageAlignment = SKYNET_Button._ImgAlign.Left;
            this.BT_Ok.ImageIcon = null;
            this.BT_Ok.Location = new System.Drawing.Point(349, 174);
            this.BT_Ok.MenuMode = false;
            this.BT_Ok.Name = "BT_Ok";
            this.BT_Ok.Rounded = false;
            this.BT_Ok.Size = new System.Drawing.Size(115, 32);
            this.BT_Ok.Style = SKYNET_Button._Style.TextOnly;
            this.BT_Ok.TabIndex = 1;
            this.BT_Ok.Text = "OK";
            this.BT_Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // LB_Message
            // 
            this.LB_Message.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.LB_Message.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.LB_Message.Location = new System.Drawing.Point(24, 38);
            this.LB_Message.Name = "LB_Message";
            this.LB_Message.Size = new System.Drawing.Size(470, 118);
            this.LB_Message.TabIndex = 0;
            // 
            // frmMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 233);
            this.Controls.Add(this.PN_Container);
            this.Controls.Add(this.PN_Left);
            this.Controls.Add(this.PN_Right);
            this.Controls.Add(this.PN_Bottom);
            this.Controls.Add(this.PN_Top);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(1360, 728);
            this.Name = "frmMessage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMessage";
            this.PN_Container.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GradiantBox PN_Top;
        private System.Windows.Forms.Panel PN_Bottom;
        private System.Windows.Forms.Panel PN_Right;
        private System.Windows.Forms.Panel PN_Left;
        private System.Windows.Forms.Panel PN_Container;
        private System.Windows.Forms.Label LB_Message;
        private SKYNET_Button BT_Cancel;
        private SKYNET_Button BT_Ok;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button ok;
    }
}