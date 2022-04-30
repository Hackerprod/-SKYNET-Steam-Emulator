using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Network.Packets
{
    public class NET_Base
    {

    }

    public class NET_Announce : NET_Base
    {
        public string PersonaName { get; set; }
        public uint AccountID { get; set; }
        public uint AppID { get; set; }
    }

    public class NET_AvatarRequest : NET_Base
    {

    }

    public class NET_AvatarResponse : NET_Base
    {
        public uint AccountID { get; set; }
        public string HexAvatar { get; set; }
    }

    public enum MessageType : int
    {
        NET_Announce,
        NET_AnnounceResponse,
        NET_AvatarRequest,
        NET_AvatarResponse,
    }
}
