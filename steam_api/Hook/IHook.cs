using EasyHook;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public abstract class IHook
    {
        public abstract string Library { get; }

        public abstract string Method { get; }

        public bool Installed { get; set; }

        public abstract LocalHook Hook { get; set; }

        public abstract System.Delegate Delegate { get; }

        public IntPtr ProcAddress { get; internal set; }

        public void Write(object msg)
        {
            string sender = Method.ToUpper();
            Main.Write(sender, msg);
        }
    }
}
