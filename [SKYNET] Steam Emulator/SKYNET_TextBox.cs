using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET.Controls
{
    public partial class SKYNET_TextBox : UserControl
    {
        [Category("SKYNET")]
        public event EventHandler OnLogoClicked;

        [Category("SKYNET")]
        public EventHandler OnReturnPressed { get; internal set; }

        private bool isPassword;

        public SKYNET_TextBox()
        {
            InitializeComponent();
            textBox.BackColor = BackColor;

            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.KeyDown += TextBox_KeyDown;
            textBox.KeyUp += TextBox_KeyUp;
            textBox.KeyPress += TextBox_KeyPress;
            textBox.TextChanged += TextBox_TextChanged;

            BackColor = Color;
        }

        [Category("SKYNET")]
        public override string Text
        {
            get { return textBox.Text; }
            set
            {
                textBox.Text = value;
            }
        }

        public override Font Font { get => textBox.Font; set => textBox.Font = value; }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
        }

        [Category("SKYNET")]
        public override Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                panel1.BackColor = value;
                panel2.BackColor = value;
                panel3.BackColor = value;
                panel4.BackColor = value;
                P_Container.BackColor = value;
                textBox.BackColor = backColor;
                textBox.Refresh();
            }
        }
        private Color backColor;
        [Category("SKYNET")]
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
            }
        }
        private Color color;


        [Category("SKYNET")]
        public override Color ForeColor
        {
            get 
            { 
                return textBox.ForeColor; 
            }
            set
            {
                textBox.ForeColor = value;
                base.ForeColor = value;
            }
        }
        [Category("SKYNET")]
        public bool ShowLogo
        {
            get
            {
                return _ShowLogo;
            }
            set
            {
                _ShowLogo = value;
                logo_box.Visible = value;
            }
        }
        bool _ShowLogo = true;
        [Category("SKYNET")]
        public Image Logo
        {
            get
            {
                return logo_box.Image;
            }
            set
            {
                logo_box.Image = value;
            }
        }

        [Category("SKYNET")]
        public Cursor LogoCursor
        {
            get { return logo_box.Cursor; }
            set { logo_box.Cursor = value; }
        }

        [Category("SKYNET")]
        public Color ActivatedBackColor
        {
            get { return focused_BackColor; }
            set
            {
                focused_BackColor = value;
            }
        }
        private Color focused_BackColor;

        [Category("SKYNET")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Container.BackColor = value;
            }
        }
        [Category("SKYNET")]
        public bool IsPassword
        {
            get { return isPassword; }
            set
            {
                isPassword = value;
                if (value)
                {
                    textBox.PasswordChar = '*';
                    textBox.UseSystemPasswordChar = true;
                }
                else
                    textBox.UseSystemPasswordChar = false;
            }
        }


        private Color borderColor;

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            textBox.Focus();
        }

        private void LogoClick(object sender, MouseEventArgs e)
        {
            OnLogoClicked?.Invoke(this, e);
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            BackColor = ActivatedBackColor;
            base.OnGotFocus(e);
        }
        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            BackColor = Color;
            base.OnLostFocus(e);
        }
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            base.OnTextChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            BackColor = Color;
            base.OnPaint(e);
        }

    }
}
