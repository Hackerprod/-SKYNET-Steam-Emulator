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

        // Store/catalog visibility is independent from license ownership and install state.
        public bool Available { get; set; } = true;
        public bool Owned { get; set; } = true;
        public bool Installed { get; set; } = true;
    }
}
