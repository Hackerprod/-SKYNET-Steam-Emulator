using SKYNET.GUI.Controls;
using System.Windows.Forms;

namespace SKYNET.GUI
{
    partial class frmGameDownload
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
            this.PN_Top = new SKYNET_GradiantBox();
            this.PN_Bottom = new Panel();
            this.PN_Right = new Panel();
            this.PN_Left = new Panel();
            this.PN_Container = new Panel();
            this.PB_Progress = new SKYNET_ProgressBar();
            this.LB_Info = new Label();
            this.LB_Name = new Label();
            this.PB_Avatar = new PictureBox();
            this.cancel = new Button();
            this.ok = new Button();
            this.BT_Cancel = new SKYNET_Button();
            this.PN_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).BeginInit();
            this.SuspendLayout();
            // 
            // PN_Top
            // 
            this.PN_Top.Dock = DockStyle.Top;
            this.PN_Top.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(115)))), ((int)(((byte)(139)))));
            this.PN_Top.Location = new System.Drawing.Point(0, 0);
            this.PN_Top.Mode = Mode.Horizontal;
            this.PN_Top.Name = "PN_Top";
            this.PN_Top.RigthColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(69)))), ((int)(((byte)(120)))));
            this.PN_Top.Size = new System.Drawing.Size(523, 2);
            this.PN_Top.TabIndex = 0;
            // 
            // PN_Bottom
            // 
            this.PN_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(60)))), ((int)(((byte)(67)))));
            this.PN_Bottom.Dock = DockStyle.Bottom;
            this.PN_Bottom.Location = new System.Drawing.Point(0, 232);
            this.PN_Bottom.Name = "PN_Bottom";
            this.PN_Bottom.Size = new System.Drawing.Size(523, 1);
            this.PN_Bottom.TabIndex = 1;
            // 
            // PN_Right
            // 
            this.PN_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(60)))), ((int)(((byte)(67)))));
            this.PN_Right.Dock = DockStyle.Right;
            this.PN_Right.Location = new System.Drawing.Point(522, 2);
            this.PN_Right.Name = "PN_Right";
            this.PN_Right.Size = new System.Drawing.Size(1, 230);
            this.PN_Right.TabIndex = 2;
            // 
            // PN_Left
            // 
            this.PN_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(60)))), ((int)(((byte)(67)))));
            this.PN_Left.Dock = DockStyle.Left;
            this.PN_Left.Location = new System.Drawing.Point(0, 2);
            this.PN_Left.Name = "PN_Left";
            this.PN_Left.Size = new System.Drawing.Size(1, 230);
            this.PN_Left.TabIndex = 3;
            // 
            // PN_Container
            // 
            this.PN_Container.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.PN_Container.Controls.Add(this.PB_Progress);
            this.PN_Container.Controls.Add(this.LB_Info);
            this.PN_Container.Controls.Add(this.LB_Name);
            this.PN_Container.Controls.Add(this.PB_Avatar);
            this.PN_Container.Controls.Add(this.cancel);
            this.PN_Container.Controls.Add(this.ok);
            this.PN_Container.Controls.Add(this.BT_Cancel);
            this.PN_Container.Dock = DockStyle.Fill;
            this.PN_Container.Location = new System.Drawing.Point(1, 2);
            this.PN_Container.Name = "PN_Container";
            this.PN_Container.Size = new System.Drawing.Size(521, 230);
            this.PN_Container.TabIndex = 4;
            // 
            // PB_Progress
            // 
            this.PB_Progress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(54)))), ((int)(((byte)(68)))));
            this.PB_Progress.DrawHatch = true;
            this.PB_Progress.ForeColor = System.Drawing.SystemColors.Control;
            this.PB_Progress.Location = new System.Drawing.Point(28, 135);
            this.PB_Progress.Maximum = 100;
            this.PB_Progress.Minimum = 0;
            this.PB_Progress.Name = "PB_Progress";
            this.PB_Progress.ProgressColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.PB_Progress.ProgressColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(200)))));
            this.PB_Progress.ShowPercentage = true;
            this.PB_Progress.Size = new System.Drawing.Size(467, 23);
            this.PB_Progress.TabIndex = 16;
            this.PB_Progress.Text = "skyneT_ProgressBar1";
            this.PB_Progress.Value = 0;
            this.PB_Progress.ValueAlignment = SKYNET_ProgressBar.Alignment.Right;
            // 
            // LB_Info
            // 
            this.LB_Info.AutoSize = true;
            this.LB_Info.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.LB_Info.ForeColor = System.Drawing.Color.White;
            this.LB_Info.Location = new System.Drawing.Point(25, 110);
            this.LB_Info.Name = "LB_Info";
            this.LB_Info.Size = new System.Drawing.Size(62, 17);
            this.LB_Info.TabIndex = 15;
            this.LB_Info.Text = "Starting...";
            // 
            // LB_Name
            // 
            this.LB_Name.AutoSize = true;
            this.LB_Name.Font = new System.Drawing.Font("Segoe UI Emoji", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Name.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.LB_Name.Location = new System.Drawing.Point(107, 46);
            this.LB_Name.Name = "LB_Name";
            this.LB_Name.Size = new System.Drawing.Size(83, 32);
            this.LB_Name.TabIndex = 14;
            this.LB_Name.Text = "Name";
            // 
            // PB_Avatar
            // 
            this.PB_Avatar.Location = new System.Drawing.Point(28, 32);
            this.PB_Avatar.Name = "PB_Avatar";
            this.PB_Avatar.Size = new System.Drawing.Size(60, 60);
            this.PB_Avatar.SizeMode = PictureBoxSizeMode.Zoom;
            this.PB_Avatar.TabIndex = 13;
            this.PB_Avatar.TabStop = false;
            // 
            // cancel
            // 
            this.cancel.DialogResult = DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(-10, -10);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(0, 0);
            this.cancel.TabIndex = 4;
            this.cancel.Text = "cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // ok
            // 
            this.ok.DialogResult = DialogResult.OK;
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
            this.BT_Cancel.Cursor = Cursors.Hand;
            this.BT_Cancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BT_Cancel.ForeColor = System.Drawing.Color.White;
            this.BT_Cancel.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.BT_Cancel.ImageAlignment = SKYNET_Button._ImgAlign.Left;
            this.BT_Cancel.ImageIcon = null;
            this.BT_Cancel.Location = new System.Drawing.Point(380, 167);
            this.BT_Cancel.MenuMode = false;
            this.BT_Cancel.Name = "BT_Cancel";
            this.BT_Cancel.Rounded = false;
            this.BT_Cancel.Size = new System.Drawing.Size(115, 32);
            this.BT_Cancel.Style = SKYNET_Button._Style.TextOnly;
            this.BT_Cancel.TabIndex = 2;
            this.BT_Cancel.Text = "CANCEL";
            this.BT_Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // frmGameDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 233);
            this.Controls.Add(this.PN_Container);
            this.Controls.Add(this.PN_Left);
            this.Controls.Add(this.PN_Right);
            this.Controls.Add(this.PN_Bottom);
            this.Controls.Add(this.PN_Top);
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(1360, 728);
            this.Name = "frmGameDownload";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "frmMessage";
            this.PN_Container.ResumeLayout(false);
            this.PN_Container.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Avatar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SKYNET_GradiantBox PN_Top;
        private Panel PN_Bottom;
        private Panel PN_Right;
        private Panel PN_Left;
        private Panel PN_Container;
        private SKYNET_Button BT_Cancel;
        private Button cancel;
        private Button ok;
        private Label LB_Name;
        private PictureBox PB_Avatar;
        private Label LB_Info;
        private SKYNET_ProgressBar PB_Progress;
    }
}