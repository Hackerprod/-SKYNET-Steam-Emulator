using SKYNET.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class IpcManager
    {
        public static IpcCommunicator IpcClient;
        private static string channel;
        private static System.Timers.Timer _timer;

        static IpcManager()
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = false;
            _timer.Interval = 5000;
        }

        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(StartConnection);
        }

        public static void StartConnection(object state)
        {
            channel = null;
            _timer.Start();
        }



        private static void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SteamEmulator.Write("IPCManager", "Connecting to Ipc server");
            bool restart = true;
            try
            {
                string channel = "SteamEmulator";
                IpcClient = (IpcCommunicator)Activator.GetObject(typeof(IpcCommunicator), "ipc://" + channel + "/" + channel);
                if (IpcClient != null)
                {
                    SteamEmulator.Write("IPCManager", "Ipc client connected successfully");
                    restart = false;
                    IpcClient.OnMessage += IpcClient_OnMessage;
                    IpcClient.InvokeMessage("Ipc client connected successfully from " + Process.GetCurrentProcess().ProcessName);
                }
            }
            catch 
            {

            }
            if (restart)
            {
                _timer.Interval = 5000;
                _timer.Start();
            }
        }

        private static void IpcClient_OnMessage(object sender, object e)
        {
            SteamEmulator.Write("IPCManager", e.ToString());
        }


        public static IpcServerChannel IpcCreateServer<TRemoteObject>(ref string RefChannelName, WellKnownObjectMode InObjectMode, TRemoteObject ipcInterface, params WellKnownSidType[] InAllowedClientSIDs) where TRemoteObject : MarshalByRefObject
        {
            string text = RefChannelName;
            IDictionary dictionary = new Hashtable();
            dictionary["name"] = text;
            dictionary["portName"] = text;
            DiscretionaryAcl discretionaryAcl = new DiscretionaryAcl(isContainer: false, isDS: false, 1);
            if (InAllowedClientSIDs.Length == 0)
            {
                discretionaryAcl.AddAccess(AccessControlType.Allow, new SecurityIdentifier(WellKnownSidType.WorldSid, null), -1, InheritanceFlags.None, PropagationFlags.None);
            }
            else
            {
                for (int i = 0; i < InAllowedClientSIDs.Length; i++)
                {
                    discretionaryAcl.AddAccess(AccessControlType.Allow, new SecurityIdentifier(InAllowedClientSIDs[i], null), -1, InheritanceFlags.None, PropagationFlags.None);
                }
            }
            CommonSecurityDescriptor securityDescriptor = new CommonSecurityDescriptor(isContainer: false, isDS: false, ControlFlags.OwnerDefaulted | ControlFlags.GroupDefaulted | ControlFlags.DiscretionaryAclPresent, null, null, null, discretionaryAcl);
            BinaryServerFormatterSinkProvider binaryServerFormatterSinkProvider = new BinaryServerFormatterSinkProvider();
            binaryServerFormatterSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;
            IpcServerChannel ipcServerChannel = new IpcServerChannel(dictionary, binaryServerFormatterSinkProvider, securityDescriptor);
            ChannelServices.RegisterChannel(ipcServerChannel, ensureSecurity: false);
            if (ipcInterface == null)
            {
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(TRemoteObject), text, InObjectMode);
            }
            else
            {
                RemotingServices.Marshal(ipcInterface, text);
            }
            RefChannelName = text;
            return ipcServerChannel;
        }

    }
}
