using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace SKYNET
{
    public partial class TXTSendMessage : UserControl
    {
        public TXTSendMessage()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            lblSearch.Text = EmptyMessage;

        }
        public string EmptyMessage 
        {
            get 
            {
                return "Escribe un mensaje...";
            } 
        }

        private void TexBox_TextChanged(object sender, EventArgs e)
        {
            textBox.ForeColor = this.ForeColor;
            lblSearch.Text = EmptyMessage;

            if (textBox.Text.Length == 0)
            {
                InvokeAction(lblSearch, new Action(() => { lblSearch.Visible = true; }));
                lblSearch.Visible = true;
                //modCommon.Show(lblSearch.Text);
            }
            else
            {
                InvokeAction(lblSearch, new Action(() => { lblSearch.Visible = false; }));
                lblSearch.Visible = false;
            }
            Text = textBox.Text;
            base.OnTextChanged(e);
            
        }

        public static void InvokeAction(Control control, Action action)
        {
            try 
            { 
                control.Invoke(action); 
            }
            catch { }
        }

        private void TexBox_Enter(object sender, EventArgs e)
        {
            ChangeImeMode(InputLanguage.CurrentInputLanguage.Culture);
        }
        private void ChangeImeMode(CultureInfo lang)
        {
            if (lang.ToString() == "zh-CN")
            {
                textBox.ImeMode = ImeMode.On;
            }
            else
            {
                textBox.ImeMode = ImeMode.NoControl;
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
           
            if (textBox.Text.Length == 0)
            {
                lblSearch.Text = EmptyMessage;
                InvokeAction(lblSearch, new Action(() => { lblSearch.Visible = true; }));
            }
        }

        private void LblSearch_Click(object sender, EventArgs e)
        {
            textBox.Focus();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        internal void Clear()
        {
            textBox.Lines = new string[0];
            textBox.Text = string.Empty;

            textBox.Multiline = false;
            textBox.Multiline = true;
        }

        private void TXTMessage_BackColorChanged(object sender, EventArgs e)
        {
            lblSearch.BackColor = this.BackColor;
            textBox.BackColor = this.BackColor;
            panel1.BackColor = this.BackColor;
            panel2.BackColor = this.BackColor;
            panel3.BackColor = this.BackColor;
            panel4.BackColor = this.BackColor;
            panel5.BackColor = this.BackColor;
            panel6.BackColor = this.BackColor;
        }

        internal void SetEmtyMsg()
        {
            lblSearch.Text = EmptyMessage;
        }
    }
}
