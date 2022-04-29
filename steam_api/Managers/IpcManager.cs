using SKYNET.Helpers;
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
        private static System.Timers.Timer Timer;
        private static string channel;
        private static IpcCommunicator CommunicatorServer;
        private static IpcCommunicator CommunicatorClient;
        private static bool ClientConnected;

        public IpcManager()
        {
            CommunicatorServer = new IpcCommunicator();
            CommunicatorServer.OnMessage += CommunicatorServer_OnMessage;
            Timer = new System.Timers.Timer();
        }

        private void CommunicatorServer_OnMessage(object sender, object e)
        {
            Write("Received IPC message from server");
        }

        private static void CommunicatorClient_OnMessage(object sender, object e)
        {
            Write("Received IPC message from client");
        }

        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(CreateIpcThread);
            ThreadPool.QueueUserWorkItem(StartTimer);
        }

        private static void ConnectIpcThread(object state)
        {
        }

        private static void CreateIpcThread(object state)
        {
            channel = Process.GetCurrentProcess().Id.ToString();
            IpcCreateServer(ref channel, WellKnownObjectMode.Singleton, CommunicatorServer);
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

        private static void StartTimer(object threadObj)
        {
            Timer = new System.Timers.Timer();
            Timer.AutoReset = false;
            Timer.Interval = 5000;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (ClientConnected) return;

                string EmulatorChannel = "SKYNET";
                CommunicatorClient = (IpcCommunicator)Activator.GetObject(typeof(IpcCommunicator), "ipc://" + EmulatorChannel + "/" + EmulatorChannel);
                if (CommunicatorClient != null)
                {
                    CommunicatorClient.OnMessage += CommunicatorClient_OnMessage;
                    Write("Connected to IPC client");
                    ClientConnected = true;
                }
            }
            catch
            {
            }

            Timer.Interval = 5000;
            Timer.Start();
        }

        private static void Write(string v)
        {
            SteamEmulator.Write("Ipc Manager", v);
        }
    }
}
