using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Plugin
{
    public class Dota2GameCoordinator : IGameCoordinatorPlugin
    {
        private uint AppID;
        public EventHandler<Dictionary<uint, byte[]>> IsMessageAvailable { get; set; }

        public uint Initialize()
        {
            // TODO: Initialize all Game coordinator class
            return AppID;
        }

        public void MessageFromGame(byte[] bytes)
        {
            // Process message from game
            //uint MsgType = MsgUtil.GetGCMsg(new MemoryStream(bytes).ReadUInt32L());
            //IPacketGCMsg packetGCMsg = MsgUtil.GetPacketGcMsg(MsgType, bytes);
            // TODO: Process GC message
        }

        public void SendPacketToGame(uint msgType, byte[] packet)
        {
            Dictionary<uint, byte[]> message = new Dictionary<uint, byte[]>();
            message.Add(msgType, packet);
            IsMessageAvailable?.Invoke(this, message);
        }

        public void SendPacketToGame(Dictionary<uint, byte[]> messages)
        {
            IsMessageAvailable?.Invoke(this, messages);
        }
    }
}
