using System.Windows.Forms;

namespace SKYNET
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.PN_Top = new System.Windows.Forms.Panel();
            this.BT_Minimize = new SKYNET.Controls.SKYNET_MinimizeBox();
            this.BT_Close = new SKYNET.Controls.SKYNET_CloseBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.LB_Status = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gradiantBox1 = new SKYNET.Controls.GradiantBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.PN_LeftContainer = new System.Windows.Forms.Panel();
            this.LB_Add = new System.Windows.Forms.Label();
            this.PB_Add = new System.Windows.Forms.PictureBox();
            this.PN_GameContainer = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.TB_Search = new SKYNET.Controls.SKYNET_TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.PB_Banner = new System.Windows.Forms.PictureBox();
            this.PB_Logo = new System.Windows.Forms.PictureBox();
            this.shadowBox1 = new System.Windows.Forms.Panel();
            this.skyneT_Button1 = new SKYNET_Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.webContainer = new System.Windows.Forms.Panel();
            this.PN_Top.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gradiantBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.PN_LeftContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Add)).BeginInit();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Banner)).BeginInit();
            this.PB_Banner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Logo)).BeginInit();
            this.shadowBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PN_Top
            // 
            this.PN_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.PN_Top.Controls.Add(this.BT_Minimize);
            this.PN_Top.Controls.Add(this.BT_Close);
            this.PN_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.PN_Top.Location = new System.Drawing.Point(249, 0);
            this.PN_Top.Name = "PN_Top";
            this.PN_Top.Size = new System.Drawing.Size(893, 26);
            this.PN_Top.TabIndex = 0;
            // 
            // BT_Minimize
            // 
            this.BT_Minimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.BT_Minimize.Color = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.BT_Minimize.Dock = System.Windows.Forms.DockStyle.Right;
            this.BT_Minimize.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(43)))), ((int)(((byte)(45)))));
            this.BT_Minimize.Location = new System.Drawing.Point(825, 0);
            this.BT_Minimize.MaximumSize = new System.Drawing.Size(34, 26);
            this.BT_Minimize.MinimumSize = new System.Drawing.Size(34, 26);
            this.BT_Minimize.Name = "BT_Minimize";
            this.BT_Minimize.Size = new System.Drawing.Size(34, 26);
            this.BT_Minimize.TabIndex = 1;
            this.BT_Minimize.Clicked += new System.EventHandler(this.Minimize_Clicked);
            // 
            // BT_Close
            // 
            this.BT_Close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.BT_Close.Color = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.BT_Close.Dock = System.Windows.Forms.DockStyle.Right;
            this.BT_Close.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(43)))), ((int)(((byte)(45)))));
            this.BT_Close.Location = new System.Drawing.Point(859, 0);
            this.BT_Close.MaximumSize = new System.Drawing.Size(34, 26);
            this.BT_Close.MinimumSize = new System.Drawing.Size(34, 26);
            this.BT_Close.Name = "BT_Close";
            this.BT_Close.Size = new System.Drawing.Size(34, 26);
            this.BT_Close.TabIndex = 0;
            this.BT_Close.Clicked += new System.EventHandler(this.Close_Clicked);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(46)))), ((int)(((byte)(51)))));
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.LB_Status);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(249, 565);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(893, 44);
            this.panel3.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Segoe UI Emoji", 8F);
            this.label6.ForeColor = System.Drawing.Color.DarkGray;
            this.label6.Location = new System.Drawing.Point(3, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(812, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "Connected to 10.31.0.1";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LB_Status
            // 
            this.LB_Status.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Status.ForeColor = System.Drawing.Color.Gainsboro;
            this.LB_Status.Location = new System.Drawing.Point(3, 5);
            this.LB_Status.Name = "LB_Status";
            this.LB_Status.Size = new System.Drawing.Size(812, 15);
            this.LB_Status.TabIndex = 10;
            this.LB_Status.Text = "ONLINE";
            this.LB_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(33)))), ((int)(((byte)(35)))));
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(249, 26);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(893, 40);
            this.panel4.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(208)))), ((int)(((byte)(218)))));
            this.label4.Location = new System.Drawing.Point(253, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 26);
            this.label4.TabIndex = 2;
            this.label4.Text = "HACKERPROD";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(208)))), ((int)(((byte)(218)))));
            this.label3.Location = new System.Drawing.Point(104, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 26);
            this.label3.TabIndex = 1;
            this.label3.Text = "COMMUNITY";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Emoji", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(208)))), ((int)(((byte)(218)))));
            this.label2.Location = new System.Drawing.Point(13, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 26);
            this.label2.TabIndex = 0;
            this.label2.Text = "STORE";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.gradiantBox1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(249, 89);
            this.panel1.TabIndex = 7;
            // 
            // gradiantBox1
            // 
            this.gradiantBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(40)))), ((int)(((byte)(47)))));
            this.gradiantBox1.Controls.Add(this.label5);
            this.gradiantBox1.Controls.Add(this.label1);
            this.gradiantBox1.Controls.Add(this.pictureBox1);
            this.gradiantBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gradiantBox1.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(111)))), ((int)(((byte)(124)))));
            this.gradiantBox1.Location = new System.Drawing.Point(0, 10);
            this.gradiantBox1.Mode = SKYNET.Controls.Mode.Vertical;
            this.gradiantBox1.Name = "gradiantBox1";
            this.gradiantBox1.RigthColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(40)))), ((int)(((byte)(47)))));
            this.gradiantBox1.Size = new System.Drawing.Size(249, 79);
            this.gradiantBox1.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(187, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "SKYNET";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(63, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 25);
            this.label1.TabIndex = 7;
            this.label1.Text = "STEAM EMU";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::SKYNET.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(27, 46);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 30);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(111)))), ((int)(((byte)(124)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(249, 10);
            this.panel2.TabIndex = 8;
            // 
            // PN_LeftContainer
            // 
            this.PN_LeftContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(46)))), ((int)(((byte)(51)))));
            this.PN_LeftContainer.Controls.Add(this.LB_Add);
            this.PN_LeftContainer.Controls.Add(this.PB_Add);
            this.PN_LeftContainer.Controls.Add(this.PN_GameContainer);
            this.PN_LeftContainer.Controls.Add(this.panel6);
            this.PN_LeftContainer.Controls.Add(this.panel5);
            this.PN_LeftContainer.Controls.Add(this.panel1);
            this.PN_LeftContainer.Dock = System.Windows.Forms.DockStyle.Left;
            this.PN_LeftContainer.Location = new System.Drawing.Point(0, 0);
            this.PN_LeftContainer.Name = "PN_LeftContainer";
            this.PN_LeftContainer.Size = new System.Drawing.Size(249, 609);
            this.PN_LeftContainer.TabIndex = 6;
            // 
            // LB_Add
            // 
            this.LB_Add.AutoSize = true;
            this.LB_Add.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25F);
            this.LB_Add.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.LB_Add.Location = new System.Drawing.Point(40, 578);
            this.LB_Add.Name = "LB_Add";
            this.LB_Add.Size = new System.Drawing.Size(92, 15);
            this.LB_Add.TabIndex = 9;
            this.LB_Add.Text = "ADD NEW GAME";
            this.LB_Add.MouseLeave += new System.EventHandler(this.Add_MouseLeave);
            this.LB_Add.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Add_MouseMove);
            // 
            // PB_Add
            // 
            this.PB_Add.Image = global::SKYNET.Properties.Resources.add;
            this.PB_Add.Location = new System.Drawing.Point(10, 571);
            this.PB_Add.Name = "PB_Add";
            this.PB_Add.Size = new System.Drawing.Size(28, 28);
            this.PB_Add.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Add.TabIndex = 9;
            this.PB_Add.TabStop = false;
            this.PB_Add.MouseLeave += new System.EventHandler(this.Add_MouseLeave);
            this.PB_Add.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Add_MouseMove);
            // 
            // PN_GameContainer
            // 
            this.PN_GameContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(40)))), ((int)(((byte)(47)))));
            this.PN_GameContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.PN_GameContainer.Location = new System.Drawing.Point(0, 141);
            this.PN_GameContainer.Name = "PN_GameContainer";
            this.PN_GameContainer.Size = new System.Drawing.Size(249, 424);
            this.PN_GameContainer.TabIndex = 12;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(40)))), ((int)(((byte)(47)))));
            this.panel6.Controls.Add(this.TB_Search);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 94);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(249, 47);
            this.panel6.TabIndex = 11;
            // 
            // TB_Search
            // 
            this.TB_Search.ActivatedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
            this.TB_Search.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
            this.TB_Search.Color = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(35)))), ((int)(((byte)(40)))));
            this.TB_Search.IsPassword = false;
            this.TB_Search.Location = new System.Drawing.Point(12, 6);
            this.TB_Search.Logo = global::SKYNET.Properties.Resources.search;
            this.TB_Search.LogoCursor = System.Windows.Forms.Cursors.Default;
            this.TB_Search.Name = "TB_Search";
            this.TB_Search.ShowLogo = true;
            this.TB_Search.Size = new System.Drawing.Size(225, 35);
            this.TB_Search.TabIndex = 9;
            this.TB_Search.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TB_Search_KeyUp);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(40)))), ((int)(((byte)(47)))));
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 89);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(249, 5);
            this.panel5.TabIndex = 10;
            // 
            // PB_Banner
            // 
            this.PB_Banner.Controls.Add(this.PB_Logo);
            this.PB_Banner.Controls.Add(this.shadowBox1);
            this.PB_Banner.Dock = System.Windows.Forms.DockStyle.Top;
            this.PB_Banner.Image = global::SKYNET.Properties.Resources._570_library_hero;
            this.PB_Banner.Location = new System.Drawing.Point(249, 66);
            this.PB_Banner.Name = "PB_Banner";
            this.PB_Banner.Size = new System.Drawing.Size(893, 0);
            this.PB_Banner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PB_Banner.TabIndex = 9;
            this.PB_Banner.TabStop = false;
            // 
            // PB_Logo
            // 
            this.PB_Logo.BackColor = System.Drawing.Color.Transparent;
            this.PB_Logo.Image = global::SKYNET.Properties.Resources._570_logo;
            this.PB_Logo.Location = new System.Drawing.Point(255, 183);
            this.PB_Logo.Name = "PB_Logo";
            this.PB_Logo.Size = new System.Drawing.Size(309, 67);
            this.PB_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_Logo.TabIndex = 10;
            this.PB_Logo.TabStop = false;
            // 
            // shadowBox1
            // 
            this.shadowBox1.BackColor = System.Drawing.Color.Transparent;
            this.shadowBox1.Controls.Add(this.skyneT_Button1);
            this.shadowBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.shadowBox1.Location = new System.Drawing.Point(0, -50);
            this.shadowBox1.Name = "shadowBox1";
            this.shadowBox1.Size = new System.Drawing.Size(893, 50);
            this.shadowBox1.TabIndex = 10;
            // 
            // skyneT_Button1
            // 
            this.skyneT_Button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(186)))), ((int)(((byte)(65)))));
            this.skyneT_Button1.BackColorMouseOver = System.Drawing.Color.Empty;
            this.skyneT_Button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.skyneT_Button1.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.skyneT_Button1.ForeColor = System.Drawing.Color.White;
            this.skyneT_Button1.ForeColorMouseOver = System.Drawing.Color.Empty;
            this.skyneT_Button1.ImageAlignment = SKYNET_Button._ImgAlign.Left;
            this.skyneT_Button1.ImageIcon = null;
            this.skyneT_Button1.Location = new System.Drawing.Point(18, 11);
            this.skyneT_Button1.MenuMode = false;
            this.skyneT_Button1.Name = "skyneT_Button1";
            this.skyneT_Button1.Rounded = false;
            this.skyneT_Button1.Size = new System.Drawing.Size(100, 32);
            this.skyneT_Button1.Style = SKYNET_Button._Style.TextOnly;
            this.skyneT_Button1.TabIndex = 0;
            this.skyneT_Button1.Text = "JUGAR";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(73)))));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.richTextBox1.Location = new System.Drawing.Point(255, 486);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBox1.Size = new System.Drawing.Size(158, 27);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            this.richTextBox1.Visible = false;
            // 
            // webContainer
            // 
            this.webContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webContainer.Location = new System.Drawing.Point(249, 66);
            this.webContainer.Name = "webContainer";
            this.webContainer.Size = new System.Drawing.Size(893, 499);
            this.webContainer.TabIndex = 10;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(1142, 609);
            this.Controls.Add(this.webContainer);
            this.Controls.Add(this.PB_Banner);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.PN_Top);
            this.Controls.Add(this.PN_LeftContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.PN_Top.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.gradiantBox1.ResumeLayout(false);
            this.gradiantBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.PN_LeftContainer.ResumeLayout(false);
            this.PN_LeftContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Add)).EndInit();
            this.panel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Banner)).EndInit();
            this.PB_Banner.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Logo)).EndInit();
            this.shadowBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PN_Top;
        private Controls.SKYNET_MinimizeBox BT_Minimize;
        private Controls.SKYNET_CloseBox BT_Close;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private Controls.GradiantBox gradiantBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel PN_LeftContainer;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel PN_GameContainer;
        private System.Windows.Forms.Panel panel6;
        private Controls.SKYNET_TextBox TB_Search;
        private System.Windows.Forms.PictureBox PB_Add;
        private System.Windows.Forms.Label LB_Add;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox PB_Banner;
        private System.Windows.Forms.PictureBox PB_Logo;
        private System.Windows.Forms.Label LB_Status;
        private Panel shadowBox1;
        private SKYNET_Button skyneT_Button1;
        private Label label6;
        private RichTextBox richTextBox1;
        private Panel webContainer;
    }
}

