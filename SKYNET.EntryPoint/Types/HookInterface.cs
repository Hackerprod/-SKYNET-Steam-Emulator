using EasyHook;
using SKYNET.Types;
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

        public string SerializedGame { get; set; }

        public string SerializedSettings { get; set; }

        public InjectionOptions InjectionOptions { get; set; }
        public bool SendLog { get; set; }

        public event EventHandler<string> PingNotify;

        public event EventHandler<GameMessage> OnMessage;

        public event EventHandler<string> OnShowMessage;
        
        public event EventHandler<string> OnUninstall;

        public void Ping(string callbackChannel)
        {
            this.PingNotify?.Invoke(this, callbackChannel);
        }

        public void InvokeMessage(uint appId, string sender, object msg)
        {
            this.OnMessage?.Invoke(this, new GameMessage(appId, sender, msg));
        }

        public void InvokeShowMessage(object msg)
        {
            OnShowMessage?.Invoke(this, msg.ToString());
        }
    }
}