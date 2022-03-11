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
        private Color borderColor;
        private bool _OnlyNumbers;
        private Color backColor;
        private Color color;
        private Color focused_BackColor;

        public SKYNET_TextBox()
        {
            InitializeComponent();
            textBox.BackColor = BackColor;

            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.KeyDown += TextBox_KeyDown;
            textBox.KeyUp += TextBox_KeyUp;
            textBox.KeyPress += TextBox_KeyPress;

            BackColor = Color;
        }

        [Category("SKYNET")]
        public event EventHandler OnLogoClicked;

        [Category("SKYNET")]
        public EventHandler OnReturnPressed;

        [Category("SKYNET")]
        public int TopSeparator
        {
            get
            {
                return PN_Top.Height;
            }
            set
            {
                PN_Top.Height = value;
            }
        }

        [Category("SKYNET")]
        public bool OnlyNumbers
        {
            get
            {
                return _OnlyNumbers;
            }
            set
            {
                _OnlyNumbers = value;
            }
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
        [Category("SKYNET")]
        public override Color BackColor
        {
            get { return backColor; }
            set
            {
                backColor = value;
                PN_Top.BackColor = value;
                panel2.BackColor = value;
                panel3.BackColor = value;
                panel4.BackColor = value;
                P_Container.BackColor = value;
                textBox.BackColor = backColor;
                textBox.Refresh();
            }
        }

        [Category("SKYNET")]
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
            }
        }


        [Category("SKYNET")]
        public override Color ForeColor
        {
            get { return textBox.ForeColor; }
            set
            {
                textBox.ForeColor = value;
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
        private bool isPassword;

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
        protected override void OnPaint(PaintEventArgs e)
        {
            BackColor = Color;
            base.OnPaint(e);
        }

        private void textBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (_OnlyNumbers)
            {
                switch (e.KeyData.ToString())
                {
                    case "D0":
                        base.OnKeyDown(e);
                        break;
                    case "D1":
                        base.OnKeyDown(e);
                        break;
                    case "D2":
                        base.OnKeyDown(e);
                        break;
                    case "D3":
                        base.OnKeyDown(e);
                        break;
                    case "D4":
                        base.OnKeyDown(e);
                        break;
                    case "D5":
                        base.OnKeyDown(e);
                        break;
                    case "D6":
                        base.OnKeyDown(e);
                        break;
                    case "D7":
                        base.OnKeyDown(e);
                        break;
                    case "D8":
                        base.OnKeyDown(e);
                        break;
                    case "D9":
                        base.OnKeyDown(e);
                        break;
                    case "NumPad0":
                        base.OnKeyDown(e);
                        break;
                    case "NumPad1":
                        base.OnKeyDown(e);
                        break;
                    case "NumPad2":
                        base.OnKeyDown(e);
                        break;
                    case "NumPad3":
                        base.OnKeyDown(e);
                        break;
                    case "NumPad4":
                        base.OnKeyDown(e);
                        break;
                    case "NumPad5":
                        base.OnKeyDown(e);
                        break;
                    case "NumPad6":
                        base.OnKeyDown(e);
                        break;
                    case "NumPad7":
                        base.OnKeyDown(e);
                        break;
                    case "NumPad8":
                        base.OnKeyDown(e);
                        break;
                    case "NumPad9":
                        base.OnKeyDown(e);
                        break;
                    case "Delete":
                        base.OnKeyDown(e);
                        break;
                    case "Back":
                        base.OnKeyDown(e);
                        break;
                    case "Left":
                        base.OnKeyDown(e);
                        break;
                    case "Up":
                        base.OnKeyDown(e);
                        break;
                    case "Right":
                        base.OnKeyDown(e);
                        break;
                    case "Down":
                        base.OnKeyDown(e);
                        break;
                    default:
                        e.SuppressKeyPress = true;
                        break;
                }
            }
            else
                base.OnKeyDown(e);
        }
    }
}
