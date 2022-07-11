using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET
{

    public partial class GameSchema
    {
        public GameDetails Game { get; set; }

        public partial class GameDetails
        {
            public string GameName { get; set; }
            public long GameVersion { get; set; }
            public AvailableStats AvailableGameStats { get; set; }

            public partial class AvailableStats
            {
                public List<Achievement> Achievements { get; set; }
                public List<Stat> Stats { get; set; }

                public partial class Achievement
                {
                    public string Name { get; set; }
                    public long Defaultvalue { get; set; }
                    public string DisplayName { get; set; }
                    public long Hidden { get; set; }
                    public string Description { get; set; }
                    public Uri Icon { get; set; }
                    public Uri Icongray { get; set; }
                }

                public partial class Stat
                {
                    public string Name { get; set; }
                    public long Defaultvalue { get; set; }
                    public string DisplayName { get; set; }
                }
            }

        }

    }


}

