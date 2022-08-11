﻿using System.Windows.Forms;

namespace SKYNET.GUI.Controls
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
            this.PN_Container = new Panel();
            this.textBox = new TextBox();
            this.logo_box = new PictureBox();
            this.PN_Right = new Panel();
            this.PN_Left = new Panel();
            this.PN_Buttom = new Panel();
            this.PN_Top = new Panel();
            this.Container = new Panel();
            this.PN_Container.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo_box)).BeginInit();
            this.Container.SuspendLayout();
            this.SuspendLayout();
            // 
            // PN_Container
            // 
            this.PN_Container.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.PN_Container.Controls.Add(this.textBox);
            this.PN_Container.Controls.Add(this.logo_box);
            this.PN_Container.Controls.Add(this.PN_Right);
            this.PN_Container.Controls.Add(this.PN_Left);
            this.PN_Container.Controls.Add(this.PN_Buttom);
            this.PN_Container.Controls.Add(this.PN_Top);
            this.PN_Container.Dock = DockStyle.Fill;
            this.PN_Container.Location = new System.Drawing.Point(1, 1);
            this.PN_Container.Name = "PN_Container";
            this.PN_Container.Size = new System.Drawing.Size(218, 33);
            this.PN_Container.TabIndex = 0;
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.textBox.BorderStyle = BorderStyle.None;
            this.textBox.Dock = DockStyle.Fill;
            this.textBox.Font = new System.Drawing.Font("Segoe UI Emoji", 9.75F);
            this.textBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.textBox.Location = new System.Drawing.Point(10, 6);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(177, 18);
            this.textBox.TabIndex = 93;
            this.textBox.MouseClick += new MouseEventHandler(this.OnMouseClick);
            // 
            // logo_box
            // 
            this.logo_box.BackColor = System.Drawing.Color.Transparent;
            this.logo_box.Dock = DockStyle.Right;
            this.logo_box.Location = new System.Drawing.Point(187, 6);
            this.logo_box.Name = "logo_box";
            this.logo_box.Size = new System.Drawing.Size(21, 21);
            this.logo_box.SizeMode = PictureBoxSizeMode.StretchImage;
            this.logo_box.TabIndex = 5;
            this.logo_box.TabStop = false;
            this.logo_box.MouseClick += new MouseEventHandler(this.LogoClick);
            // 
            // PN_Right
            // 
            this.PN_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.PN_Right.Dock = DockStyle.Right;
            this.PN_Right.Location = new System.Drawing.Point(208, 6);
            this.PN_Right.Name = "PN_Right";
            this.PN_Right.Size = new System.Drawing.Size(10, 21);
            this.PN_Right.TabIndex = 4;
            this.PN_Right.MouseClick += new MouseEventHandler(this.OnMouseClick);
            // 
            // PN_Left
            // 
            this.PN_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.PN_Left.Dock = DockStyle.Left;
            this.PN_Left.Location = new System.Drawing.Point(0, 6);
            this.PN_Left.Name = "PN_Left";
            this.PN_Left.Size = new System.Drawing.Size(10, 21);
            this.PN_Left.TabIndex = 3;
            this.PN_Left.MouseClick += new MouseEventHandler(this.OnMouseClick);
            // 
            // PN_Buttom
            // 
            this.PN_Buttom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.PN_Buttom.Dock = DockStyle.Bottom;
            this.PN_Buttom.Location = new System.Drawing.Point(0, 27);
            this.PN_Buttom.Name = "PN_Buttom";
            this.PN_Buttom.Size = new System.Drawing.Size(218, 6);
            this.PN_Buttom.TabIndex = 2;
            this.PN_Buttom.MouseClick += new MouseEventHandler(this.OnMouseClick);
            // 
            // PN_Top
            // 
            this.PN_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.PN_Top.Dock = DockStyle.Top;
            this.PN_Top.Location = new System.Drawing.Point(0, 0);
            this.PN_Top.Name = "PN_Top";
            this.PN_Top.Size = new System.Drawing.Size(218, 6);
            this.PN_Top.TabIndex = 1;
            this.PN_Top.MouseClick += new MouseEventHandler(this.OnMouseClick);
            // 
            // Container
            // 
            this.Container.Controls.Add(this.PN_Container);
            this.Container.Dock = DockStyle.Fill;
            this.Container.Location = new System.Drawing.Point(0, 0);
            this.Container.Name = "Container";
            this.Container.Padding = new Padding(1);
            this.Container.Size = new System.Drawing.Size(220, 35);
            this.Container.TabIndex = 94;
            // 
            // SKYNET_TextBox
            // 
            this.AutoScaleMode = AutoScaleMode.None;
            this.Controls.Add(this.Container);
            this.Name = "SKYNET_TextBox";
            this.Size = new System.Drawing.Size(220, 35);
            this.PN_Container.ResumeLayout(false);
            this.PN_Container.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo_box)).EndInit();
            this.Container.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel PN_Container;
        private Panel PN_Top;
        private Panel PN_Left;
        private Panel PN_Buttom;
        private Panel PN_Right;
        private PictureBox logo_box;
        private TextBox textBox;
        private Panel Container;
    }
}
