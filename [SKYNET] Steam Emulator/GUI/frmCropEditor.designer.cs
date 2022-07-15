using SKYNET.GUI.Controls;
using System.Windows.Forms;

namespace SKYNET.GUI
{
    partial class frmCropEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCropEditor));
            this.panelTop = new Panel();
            this.CloseBox = new Panel();
            this.ClosePic = new PictureBox();
            this.acceptBtn = new Button();
            this.Browser = new WebBrowser();
            this.panelBody = new Panel();
            this.panel3 = new Panel();
            this.l_Preview = new Label();
            this.panel2 = new Panel();
            this.panel16 = new Panel();
            this.BT_Rotate = new SKYNET_Button();
            this.Redondear = new SKYNET_Check();
            this._circled = new Label();
            this.ShowLine = new SKYNET_Check();
            this._showlines = new Label();
            this.panel1 = new Panel();
            this.p_Preview = new PictureBox();
            this.BT_Apply = new SKYNET_Button();
            this.ImageCrop = new ImageCropControl();
            this.label6 = new Label();
            this.panelLogo = new Panel();
            this.Logo = new PictureBox();
            this.panelTop.SuspendLayout();
            this.CloseBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClosePic)).BeginInit();
            this.panelBody.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.p_Preview)).BeginInit();
            this.panelLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(46)))), ((int)(((byte)(60)))));
            this.panelTop.Controls.Add(this.CloseBox);
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.ForeColor = System.Drawing.Color.White;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(725, 26);
            this.panelTop.TabIndex = 5;
            // 
            // CloseBox
            // 
            this.CloseBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(47)))), ((int)(((byte)(61)))));
            this.CloseBox.Controls.Add(this.ClosePic);
            this.CloseBox.Dock = DockStyle.Right;
            this.CloseBox.Location = new System.Drawing.Point(691, 0);
            this.CloseBox.Name = "CloseBox";
            this.CloseBox.Size = new System.Drawing.Size(34, 26);
            this.CloseBox.TabIndex = 12;
            this.CloseBox.MouseClick += new MouseEventHandler(this.CloseBox_MouseClick);
            this.CloseBox.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.CloseBox.MouseMove += new MouseEventHandler(this.Control_MouseMove);
            // 
            // ClosePic
            // 
            this.ClosePic.Image = global::SKYNET.Properties.Resources.close;
            this.ClosePic.Location = new System.Drawing.Point(9, 5);
            this.ClosePic.Name = "ClosePic";
            this.ClosePic.Size = new System.Drawing.Size(16, 16);
            this.ClosePic.SizeMode = PictureBoxSizeMode.CenterImage;
            this.ClosePic.TabIndex = 4;
            this.ClosePic.TabStop = false;
            this.ClosePic.MouseClick += new MouseEventHandler(this.CloseBox_MouseClick);
            this.ClosePic.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.ClosePic.MouseMove += new MouseEventHandler(this.Control_MouseMove);
            // 
            // acceptBtn
            // 
            this.acceptBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.acceptBtn.Location = new System.Drawing.Point(819, 372);
            this.acceptBtn.Name = "acceptBtn";
            this.acceptBtn.Size = new System.Drawing.Size(75, 23);
            this.acceptBtn.TabIndex = 16;
            this.acceptBtn.Text = "button1";
            this.acceptBtn.UseVisualStyleBackColor = true;
            // 
            // Browser
            // 
            this.Browser.Location = new System.Drawing.Point(-21, -2);
            this.Browser.Name = "Browser";
            this.Browser.Size = new System.Drawing.Size(16, 20);
            this.Browser.TabIndex = 18;
            // 
            // panelBody
            // 
            this.panelBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.panelBody.Controls.Add(this.panel3);
            this.panelBody.Controls.Add(this.panel2);
            this.panelBody.Controls.Add(this.panel16);
            this.panelBody.Controls.Add(this.BT_Rotate);
            this.panelBody.Controls.Add(this.Redondear);
            this.panelBody.Controls.Add(this._circled);
            this.panelBody.Controls.Add(this.ShowLine);
            this.panelBody.Controls.Add(this._showlines);
            this.panelBody.Controls.Add(this.panel1);
            this.panelBody.Controls.Add(this.BT_Apply);
            this.panelBody.Controls.Add(this.ImageCrop);
            this.panelBody.Dock = DockStyle.Top;
            this.panelBody.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(33)))), ((int)(((byte)(43)))));
            this.panelBody.Location = new System.Drawing.Point(0, 26);
            this.panelBody.Name = "panelBody";
            this.panelBody.Padding = new Padding(8);
            this.panelBody.Size = new System.Drawing.Size(725, 361);
            this.panelBody.TabIndex = 34;
            this.panelBody.Click += new System.EventHandler(this.PanelBody_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.panel3.Controls.Add(this.l_Preview);
            this.panel3.Location = new System.Drawing.Point(560, 11);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(145, 25);
            this.panel3.TabIndex = 100;
            // 
            // l_Preview
            // 
            this.l_Preview.AutoSize = true;
            this.l_Preview.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.l_Preview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.l_Preview.Location = new System.Drawing.Point(35, 5);
            this.l_Preview.Name = "l_Preview";
            this.l_Preview.Size = new System.Drawing.Size(76, 15);
            this.l_Preview.TabIndex = 96;
            this.l_Preview.Text = "Vista previa";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.panel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(27)))), ((int)(((byte)(32)))));
            this.panel2.Location = new System.Drawing.Point(543, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1, 338);
            this.panel2.TabIndex = 99;
            // 
            // panel16
            // 
            this.panel16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.panel16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(27)))), ((int)(((byte)(32)))));
            this.panel16.Location = new System.Drawing.Point(562, 224);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(145, 1);
            this.panel16.TabIndex = 98;
            // 
            // Rotate_L
            // 
            this.BT_Rotate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.BT_Rotate.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.BT_Rotate.Cursor = Cursors.Hand;
            this.BT_Rotate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BT_Rotate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.BT_Rotate.ForeColorMouseOver = System.Drawing.Color.White;
            this.BT_Rotate.ImageAlignment = SKYNET_Button._ImgAlign.Left;
            this.BT_Rotate.ImageIcon = null;
            this.BT_Rotate.Location = new System.Drawing.Point(560, 261);
            this.BT_Rotate.Name = "Rotate_L";
            this.BT_Rotate.Rounded = true;
            this.BT_Rotate.Size = new System.Drawing.Size(145, 25);
            this.BT_Rotate.Style = SKYNET_Button._Style.TextOnly;
            this.BT_Rotate.TabIndex = 97;
            this.BT_Rotate.Text = "Rotar Imagen";
            this.BT_Rotate.Click += new System.EventHandler(this.Rotate_L_Click);
            // 
            // Redondear
            // 
            this.Redondear.BackColor = System.Drawing.Color.Transparent;
            this.Redondear.Checked = false;
            this.Redondear.Cursor = Cursors.Hand;
            this.Redondear.Location = new System.Drawing.Point(671, 230);
            this.Redondear.Name = "Redondear";
            this.Redondear.Size = new System.Drawing.Size(34, 25);
            this.Redondear.TabIndex = 96;
            this.Redondear.MouseClick += new MouseEventHandler(this.Redondear_MouseClick);
            // 
            // _circled
            // 
            this._circled.AutoSize = true;
            this._circled.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Bold);
            this._circled.ForeColor = System.Drawing.Color.White;
            this._circled.Location = new System.Drawing.Point(559, 235);
            this._circled.Name = "_circled";
            this._circled.Size = new System.Drawing.Size(71, 15);
            this._circled.TabIndex = 95;
            this._circled.Text = "Redondear";
            // 
            // ShowLine
            // 
            this.ShowLine.BackColor = System.Drawing.Color.Transparent;
            this.ShowLine.Checked = false;
            this.ShowLine.Cursor = Cursors.Hand;
            this.ShowLine.Location = new System.Drawing.Point(671, 193);
            this.ShowLine.Name = "ShowLine";
            this.ShowLine.Size = new System.Drawing.Size(34, 25);
            this.ShowLine.TabIndex = 92;
            this.ShowLine.MouseClick += new MouseEventHandler(this.ShowLine_MouseClick);
            // 
            // _showlines
            // 
            this._showlines.AutoSize = true;
            this._showlines.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Bold);
            this._showlines.ForeColor = System.Drawing.Color.White;
            this._showlines.Location = new System.Drawing.Point(559, 198);
            this._showlines.Name = "_showlines";
            this._showlines.Size = new System.Drawing.Size(91, 15);
            this._showlines.TabIndex = 91;
            this._showlines.Text = "Mostrar lineas";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.panel1.Controls.Add(this.p_Preview);
            this.panel1.Location = new System.Drawing.Point(560, 42);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new Padding(2);
            this.panel1.Size = new System.Drawing.Size(145, 145);
            this.panel1.TabIndex = 38;
            // 
            // p_Preview
            // 
            this.p_Preview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.p_Preview.Dock = DockStyle.Fill;
            this.p_Preview.Location = new System.Drawing.Point(2, 2);
            this.p_Preview.Name = "p_Preview";
            this.p_Preview.Size = new System.Drawing.Size(141, 141);
            this.p_Preview.SizeMode = PictureBoxSizeMode.StretchImage;
            this.p_Preview.TabIndex = 1;
            this.p_Preview.TabStop = false;
            // 
            // btn_Apply
            // 
            this.BT_Apply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(56)))), ((int)(((byte)(70)))));
            this.BT_Apply.BackColorMouseOver = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(61)))), ((int)(((byte)(75)))));
            this.BT_Apply.Cursor = Cursors.Hand;
            this.BT_Apply.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BT_Apply.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))));
            this.BT_Apply.ForeColorMouseOver = System.Drawing.Color.White;
            this.BT_Apply.ImageAlignment = SKYNET_Button._ImgAlign.Left;
            this.BT_Apply.ImageIcon = null;
            this.BT_Apply.Location = new System.Drawing.Point(560, 325);
            this.BT_Apply.Name = "btn_Apply";
            this.BT_Apply.Rounded = true;
            this.BT_Apply.Size = new System.Drawing.Size(145, 25);
            this.BT_Apply.Style = SKYNET_Button._Style.TextOnly;
            this.BT_Apply.TabIndex = 37;
            this.BT_Apply.Text = "Guardar cambios";
            this.BT_Apply.Click += new System.EventHandler(this.Btn_Apply_Click);
            // 
            // ImageCrop
            // 
            this.ImageCrop.AccessibleName = "Image crop pane";
            this.ImageCrop.AccessibleRole = AccessibleRole.Pane;
            this.ImageCrop.AspectRatio = 1D;
            this.ImageCrop.Bitmap = ((System.Drawing.Bitmap)(resources.GetObject("ImageCrop.Bitmap")));
            this.ImageCrop.GridLines = false;
            this.ImageCrop.Location = new System.Drawing.Point(11, 12);
            this.ImageCrop.Name = "ImageCrop";
            this.ImageCrop.Size = new System.Drawing.Size(526, 339);
            this.ImageCrop.TabIndex = 0;
            this.ImageCrop.CropRectangleChanged += new System.EventHandler(this.ImageCrop_CropRectangleChanged);
            this.ImageCrop.AspectRatioChanged += new System.EventHandler(this.ImageCrop_AspectRatioChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(101, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 17);
            this.label6.TabIndex = 37;
            this.label6.Text = "Game Coordinator Client";
            // 
            // panelLogo
            // 
            this.panelLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(164)))), ((int)(((byte)(245)))));
            this.panelLogo.Controls.Add(this.label6);
            this.panelLogo.Controls.Add(this.Logo);
            this.panelLogo.Dock = DockStyle.Top;
            this.panelLogo.ForeColor = System.Drawing.Color.White;
            this.panelLogo.Location = new System.Drawing.Point(0, 26);
            this.panelLogo.Margin = new Padding(0);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(725, 0);
            this.panelLogo.TabIndex = 33;
            // 
            // Logo
            // 
            this.Logo.Image = global::SKYNET.Properties.Resources.sc_button_steam_lg;
            this.Logo.Location = new System.Drawing.Point(136, 7);
            this.Logo.Name = "Logo";
            this.Logo.Size = new System.Drawing.Size(105, 105);
            this.Logo.SizeMode = PictureBoxSizeMode.Zoom;
            this.Logo.TabIndex = 1;
            this.Logo.TabStop = false;
            // 
            // frmCropEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(39)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(725, 406);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.panelLogo);
            this.Controls.Add(this.Browser);
            this.Controls.Add(this.acceptBtn);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI Emoji", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(17)))), ((int)(((byte)(22)))));
            this.FormBorderStyle = FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new Padding(3, 4, 3, 4);
            this.Name = "frmCropEditor";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "[SKYNET] Dota2 GCS";
            this.Deactivate += new System.EventHandler(this.FrmProfileEdit_Deactivate);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.panelTop.ResumeLayout(false);
            this.CloseBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ClosePic)).EndInit();
            this.panelBody.ResumeLayout(false);
            this.panelBody.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.p_Preview)).EndInit();
            this.panelLogo.ResumeLayout(false);
            this.panelLogo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Panel panelTop;
        private Button acceptBtn;
        private WebBrowser Browser;
        private Panel panelBody;
        private PictureBox Logo;
        private Label label6;
        private Panel panelLogo;
        private Panel CloseBox;
        private PictureBox ClosePic;
        private ImageCropControl ImageCrop;
        private SKYNET_Button BT_Apply;
        private SKYNET_Check ShowLine;
        private Label _showlines;
        private SKYNET_Check Redondear;
        private Label _circled;
        private SKYNET_Button BT_Rotate;
        private Panel panel16;
        private Panel panel2;
        private Panel panel3;
        private Label l_Preview;
        private Panel panel1;
        private PictureBox p_Preview;
    }
}