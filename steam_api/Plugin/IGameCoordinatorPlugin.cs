using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Plugin
{
    public interface IGameCoordinatorPlugin
    {
        uint Initialize();
        void MessageFromGame(byte[] bytes);
        EventHandler<byte[]> IsMessageAvailable { get; set; }
    }
}
