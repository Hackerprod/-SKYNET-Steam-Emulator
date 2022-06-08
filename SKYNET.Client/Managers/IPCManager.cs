using SKYNET.IPC;
using System;

namespace SKYNET.Managers
{
    public class IPCManager
    {
        private static PipeServer<IPCMessage> server;
        public static void Initialize()
        {
            server = new PipeServer<IPCMessage>("SKYNET");
            server.ClientConnected += OnClientConnected; 
            server.ClientDisconnected += OnClientDisconnected;
            server.MessageReceived += OnMessageReceived;
            server.ExceptionOccurred += OnExceptionOccurred;

            server.StartAsync();
        }

        private static void OnMessageReceived(object sender, ConnectionMessageEventArgs<IPCMessage> e)
        {
            Console.WriteLine($"MessageReceived {e.Message.Sender}: {e.Message.Message}");
        }

        private static void OnClientConnected(object sender, ConnectionEventArgs<IPCMessage> args)
        {
            Console.WriteLine($"Client {args.Connection.PipeName} is now connected!");

            SendWelcomeData(args.Connection);
        }

        private static void SendWelcomeData(PipeConnection<IPCMessage> connection)
        {

        }

        private static void OnClientDisconnected(object sender, ConnectionEventArgs<IPCMessage> e)
        {
            Console.WriteLine($"Client {e.Connection.PipeName} disconnected");
        }

        private static void OnExceptionOccurred(object sender, ExceptionEventArgs e)
        {
            Console.WriteLine("Exception Occurred!!!");
        }
    }
}