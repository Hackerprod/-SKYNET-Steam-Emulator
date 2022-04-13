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
using SKYNET.Types;

namespace SKYNET
{
    public partial class GameBox : UserControl
    {
        private Color _Color_MouseHover;
        private Game _game;
        private Color _backColor;
        private string _parametters;
        private uint _appId;
        private string _gamePath;
        private string _gameName;
        private bool _selected;

        [Category("SKYNET")]
        public event EventHandler<MouseEventArgs> BoxClicked;

        [Category("SKYNET")]
        public event EventHandler<GameBox> BoxDoubleClicked;

        [Category("SKYNET")]
        public Bitmap Image
        {
            get
            {
                return (Bitmap)PB_Avatar.Image;
            }
            set
            {
                PB_Avatar.Image = value;
            }
        }
        [Category("SKYNET")]
        public string GameName
        {
            get
            {
                return _gameName;
            }
            set
            {
                _gameName = value;
                LB_Name.Text = value;
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
        public string GamePath
        {
            get
            {
                return _gamePath;
            }
            set
            {
                _gamePath = value;
                if (File.Exists(value))
                {
                    Image = (Bitmap)IconFromFile(value);
                }
                if (string.IsNullOrEmpty(LB_Name.Text))
                {
                    LB_Name.Text = Path.GetFileNameWithoutExtension(LB_Name.Text);
                }
            }
        }

        [Category("SKYNET")]
        public uint AppId
        {
            get
            {
                return _appId;
            }
            set
            {
                _appId = value;
            }
        }

        [Category("SKYNET")]
        public string Parameters
        {
            get
            {
                return _parametters;
            }
            set
            {
                _parametters = value;
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
        public static Image IconFromFile(string filePath)
        {
            Image image = null;

            try
            {
                var extractor = new IconExtractor(filePath);
                var icon = extractor.GetIcon(0);

                Icon[] splitIcons = IconUtil.Split(icon);

                Icon selectedIcon = null;

                foreach (var item in splitIcons)
                {
                    if (selectedIcon == null)
                    {
                        selectedIcon = item;
                    }
                    else
                    {
                        if (IconUtil.GetBitCount(item) > IconUtil.GetBitCount(selectedIcon))
                        {
                            selectedIcon = item;
                        }
                        else if (IconUtil.GetBitCount(item) == IconUtil.GetBitCount(selectedIcon) && item.Width > selectedIcon.Width)
                        {
                            selectedIcon = item;
                        }
                    }
                }
                return selectedIcon.ToBitmap();
            }
            catch (Exception)
            {
                
            }

            try
            {
                image = Icon.ExtractAssociatedIcon(filePath)?.ToBitmap();
            }
            catch
            {
                image = new Icon(SystemIcons.Application, 256, 256).ToBitmap();
            }

            return image;
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
            BoxDoubleClicked?.Invoke(this, this);
        }

        public Game GetGame()
        {
            return _game;
        }
        public void SetGame(Game game)
        {
            GameName = game.Name;
            GamePath = game.ExecutablePath;
            AppId = game.AppId;
            Parameters = game.Parameters;
            _game = game;
        }
    }
}
