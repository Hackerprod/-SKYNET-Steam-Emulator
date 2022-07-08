namespace SKYNET.Types
{
    public class GameMessage
    {
        public uint AppID;
        public string Sender;
        public object Msg;

        public GameMessage(uint appId, string sender, object msg)
        {
            this.AppID = appId;
            this.Sender = sender;
            this.Msg = msg;
        }
    }
}