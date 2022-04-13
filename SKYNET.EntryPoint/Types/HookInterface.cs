using EasyHook;
using System;
using System.Collections.Concurrent;

namespace SKYNET
{
    public class HookInterface : MarshalByRefObject
    {
        public bool DumpToConsole { get; set; }

        public bool DumpToFile { get; set; }

        public string ChannelName { get; set; }

        public string EmulatorPath { get; set; }

        public string DllPath { get; set; }

        public Game Game { get; set; }

        public InjectionOptions InjectionOptions { get; set; }

        public event EventHandler<string> PingNotify;

        public event EventHandler<ConsoleMessage> OnMessage;

        public event EventHandler<string> OnShowMessage;
        
        public event EventHandler<string> OnUninstall;

        public void Ping(string callbackChannel)
        {
            this.PingNotify?.Invoke(this, callbackChannel);
        }

        public void InvokeMessage(string sender, object msg)
        {
            this.OnMessage?.Invoke(this, new ConsoleMessage(sender, msg));
        }

        public void InvokeShowMessage(object msg)
        {
            OnShowMessage?.Invoke(this, msg.ToString());
        }
    }
}