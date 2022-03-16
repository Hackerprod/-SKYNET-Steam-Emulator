using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    public class DelegateClass
    {
        public string Name;

        public List<Type> Types { get; set; }

        public DelegateClass()
        {
            Types = new List<Type>();
        }

    }
}
