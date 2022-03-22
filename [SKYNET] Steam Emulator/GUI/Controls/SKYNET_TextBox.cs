using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SKYNET.Controls
{
    public partial class SKYNET_TextBox : UserControl
    {
        private Color _borderColor;
        private bool _isPassword;
        private Color _focused_BackColor;
        private bool _ShowLogo;
        private Color _color;
        private Color _backColor;
        private bool _OnlyNumber;

        [Category("SKYNET")]
        public HorizontalAlignment TextAlign
        {
            get { return textBox.TextAlign; }
            set { textBox.TextAlign = value; }
        }

        [Category("SKYNET")]
        public event EventHandler OnLogoClicked;

        [Category("SKYNET")]
        public EventHandler OnReturnPressed { get; internal set; }

        [Category("SKYNET")]
        public override string Text
        {
            get 
            { 
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
            }
        }

        [Category("SKYNET")]
        public bool OnlyNumbers
        {
            get
            {
                return _OnlyNumber;
            }
            set
            {
                _OnlyNumber = value;
            }
        }

        [Category("SKYNET")]
        public override Color BackColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                PN_Top.BackColor = value;
                PN_Buttom.BackColor = value;
                PN_Left.BackColor = value;
                PN_Right.BackColor = value;
                PN_Container.BackColor = value;
                textBox.BackColor = value;
                textBox.Refresh();
            }
        }

        [Category("SKYNET")]
        public Color Color
        {
            get
            { 
                return _color; 
            }
            set
            {
                _color = value;
            }
        }

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
            get 
            { 
                return _focused_BackColor; 
            }
            set
            {
                _focused_BackColor = value;
            }
        }

        [Category("SKYNET")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                Container.BackColor = value;
            }
        }

        [Category("SKYNET")]
        public bool IsPassword
        {
            get 
            { 
                return _isPassword; 
            }
            set
            {
                _isPassword = value;
                if (value)
                {
                    textBox.PasswordChar = '*';
                    textBox.UseSystemPasswordChar = true;
                }
                else
                    textBox.UseSystemPasswordChar = false;
            }
        }
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
            if (_OnlyNumber)
            {
                switch (e.KeyData)
                {
                    case Keys.D0:
                    case Keys.D1:
                    case Keys.D2:
                    case Keys.D3:
                    case Keys.D4:
                    case Keys.D5:
                    case Keys.D6:
                    case Keys.D7:
                    case Keys.D8:
                    case Keys.D9:
                    case Keys.NumPad0:
                    case Keys.NumPad1:
                    case Keys.NumPad2:
                    case Keys.NumPad3:
                    case Keys.NumPad4:
                    case Keys.NumPad5:
                    case Keys.NumPad6:
                    case Keys.NumPad7:
                    case Keys.NumPad8:
                    case Keys.NumPad9:
                    case Keys.Delete:
                    case Keys.Back:
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Right:
                    case Keys.Down:
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
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            base.OnTextChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            BackColor = Color;
            base.OnPaint(e);
        }
        public override Font Font { get => textBox.Font; set => textBox.Font = value; }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
        }
    }
}
