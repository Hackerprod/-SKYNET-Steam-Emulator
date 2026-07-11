using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public class DLC
    {
        public string Name { get; set; }
        public uint AppId { get; set; }

        // Whether the DLC is reported as available/installed to the game.
        // Defaults to true so a configured DLC is owned unless explicitly disabled.
        public bool Available { get; set; } = true;
    }
}
