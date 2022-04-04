using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET
{
    public class InterfaceImplementation
    {
        public string Name { get; set; }

        public List<MethodInfo> Methods { get; set; }

        public Type Type { get; set; }

        public List<List<System.Delegate>> Delegates { get; set; }

        public InterfaceImplementation()
        {
            Methods = new List<MethodInfo>();
            Delegates = new List<List<System.Delegate>>();
        }
    }
}
