using SKYNET.Helper;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace SKYNET.Managers
{
    public class IpcManager
    {
        private static IpcCommunicator CommunicatorServer;
        private static IpcCommunicator CommunicatorClient;

        static IpcManager()
        {
            CommunicatorServer = new IpcCommunicator();
            CommunicatorServer.ProcessID = Process.GetCurrentProcess().Id;
            CommunicatorServer.OnMessage += CommunicatorServer_OnMessage;
        }

        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(StartServer);
            ThreadPool.QueueUserWorkItem(StartConnection);
        }

        public static void StartServer(object state)
        {
            string channel = Process.GetCurrentProcess().Id.ToString();
            var InObject = WellKnownObjectMode.Singleton;
            IpcCreateServer(ref channel, InObject, CommunicatorServer);
        }

        public static void StartConnection(object state)
        {
            while (true)
            {
                try
                {
                    if (CommunicatorClient == null)
                    {
                        CommunicatorClient = (IpcCommunicator)Activator.GetObject(typeof(IpcCommunicator), "ipc://SKYNET/SKYNET");
                        if (CommunicatorClient != null)
                        {
                            Write("Ipc client connected successfully");
                            CommunicatorClient.InvokeMessage(Process.GetCurrentProcess().Id);
                        }
                    }
                    else
                    {
                        CommunicatorClient.Ping("");
                    }

                }
                catch 
                {
                    CommunicatorClient = null;
                }
                Thread.Sleep(7000);
            }
        }

        private static void CommunicatorServer_OnMessage(object sender, object e)
        {
            // Message received from client
            Write(e);
        }
        private static IpcServerChannel IpcCreateServer<TRemoteObject>(ref string RefChannelName, WellKnownObjectMode InObjectMode, TRemoteObject ipcInterface, params WellKnownSidType[] InAllowedClientSIDs) where TRemoteObject : MarshalByRefObject
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

        internal static void SendMsg(string v)
        {
            try
            {
                if (CommunicatorClient != null)
                {
                    CommunicatorClient.InvokeMessage(v);
                }   
            }
            catch (Exception)
            {
            }
        }

        private static void Write(object v)
        {
            SteamEmulator.Write("IpcManager", v);
        }
    }
}
