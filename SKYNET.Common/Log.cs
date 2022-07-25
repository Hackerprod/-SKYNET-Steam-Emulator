using System;

namespace SKYNET
{
    public class Log
    {
        public static event EventHandler<LogEventArgs> OnMessage;

        public static void Write(string sender, object msg)
        {
            OnMessage?.Invoke("", new LogEventArgs(sender, msg));
        }

        public static void Info(string sender, object msg)
        {
            OnMessage?.Invoke("", new LogEventArgs(sender, msg));
        }

        public static void Error(string sender, object msg)
        {
            OnMessage?.Invoke("", new LogEventArgs(sender, msg));
        }
    }

    public class LogEventArgs : EventArgs
    {
        public string Sender { get; set; }
        public object Message { get; set; }

        public LogEventArgs(string sender, object message)
        {
            Sender = sender;
            Message = message;
        }
    }
}
