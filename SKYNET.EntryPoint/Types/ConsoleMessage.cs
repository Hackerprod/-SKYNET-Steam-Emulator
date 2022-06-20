namespace SKYNET
{
    public class ConsoleMessage
    {
        public uint AppId;
        public string Sender;
        public object Msg;

        public ConsoleMessage(uint appId, string sender, object msg)
        {
            this.AppId = appId;
            this.Sender = sender;
            this.Msg = msg;
        }
    }
}