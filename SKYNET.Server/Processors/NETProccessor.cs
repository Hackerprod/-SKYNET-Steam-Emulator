using SKYNET.Interfaces;
using SKYNET.Network;

namespace SKYNET.Processors
{
    public class NETProcessor
    {
        public static void Process(IConnection connection, NETMessage message)
        {
            switch (message.MessageType)
            {
                default:
                    break;
            }
        }
    }
}
