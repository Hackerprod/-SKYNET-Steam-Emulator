namespace SKYNET
{
    public class ConsoleMessage
    {
        public string Sender;
        public object Msg;

        public ConsoleMessage(string sender, object msg)
        {
            this.Sender = sender;
            this.Msg = msg;
        }
    }
}