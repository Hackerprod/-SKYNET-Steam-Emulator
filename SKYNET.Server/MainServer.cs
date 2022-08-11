using SKYNET.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET
{
    public class MainServer
    {
        public void Start()
        {
            ConnectionsManager.Initialize();
        }

        public void Stop()
        {
            
        }
    }
}
