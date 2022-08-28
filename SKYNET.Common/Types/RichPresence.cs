using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public class RichPresence
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public RichPresence(string name, string value)
        {
            Key = name;
            Value = value;
        }
    }
}
