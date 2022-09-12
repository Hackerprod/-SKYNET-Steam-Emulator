using SKYNET.DB;
using SKYNET.Managers;
using System.IO;

namespace SKYNET
{
    public class MainServer
    {
        public async void Start()
        {
            Log.Write("MainServer", "Initializing Steam Server");
            EnsurePaths();

            var DatabaseResult = await DBManager.Initialize();

            if (!DatabaseResult)
            {
                return;
            }

            ConnectionsManager.Initialize();
            UserManager.Initialize();
            NetworkManager.Initialize();
            PluginManager.Initialize();
        }

        private void EnsurePaths()
        {
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "MongoDB"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Storage"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Images"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Images", "AppCache"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Images", "AvatarCache"));
        }

        public void Stop()
        {
            
        }
    }
}
