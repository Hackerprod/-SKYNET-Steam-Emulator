using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using TsudaKageyu;
using SKYNET.Helper;

namespace SKYNET
{
    public partial class GameBox : UserControl
    {
        private Color _Color_MouseHover;
        private Game _game;
        private Color _backColor;
        private bool _selected;

        [Category("SKYNET")]
        public event EventHandler<MouseEventArgs> BoxClicked;

        [Category("SKYNET")]
        public event EventHandler<GameBox> BoxDoubleClicked;

        [Category("SKYNET")]
        public Game Game
        {
            get
            {
                return _game;
            }
            set
            {
                _game = value;

                if (!string.IsNullOrEmpty(Game.AvatarHex))
                {
                    var imageBytes = Convert.FromBase64String(Game.AvatarHex);
                    Bitmap Avatar = (Bitmap)ImageHelper.ImageFromBytes(imageBytes);
                    PB_Avatar.Image = Avatar;
                }

                LB_Name.Text = _game.Name;

            }
        }

        [Category("SKYNET")]
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                if (value)
                    BackColor = Color_MouseHover;
                else
                    BackColor = Color;

                PB_Avatar.BackColor = BackColor;
            }
        }

        [Category("SKYNET")]
        public Color Color
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
            }
        }

        [Category("SKYNET")]
        public Color Color_MouseHover
        {
            get
            {
                return _Color_MouseHover;
            }
            set
            {
                _Color_MouseHover = value;

            }
        }


        public GameBox()
        {
            InitializeComponent();
            LB_Name.Text = "";
        }

        private void Box_Clicked(object sender, MouseEventArgs e)
        {
            BoxClicked?.Invoke(this, e);
        }

        private void Avatar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_selected) return;
            BackColor = Color_MouseHover;
            PB_Avatar.BackColor = BackColor;
        }

        private void Avatar_MouseLeave(object sender, EventArgs e)
        {
            if (_selected) return;
            BackColor = Color;
            PB_Avatar.BackColor = BackColor;
        }

        private void Box_DoubleClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                BoxDoubleClicked?.Invoke(this, this);
            }
        }
    }
}
