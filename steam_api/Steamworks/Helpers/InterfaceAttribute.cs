using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InterfaceAttribute : Attribute
    {
        public string Name { get; set; }

        public InterfaceAttribute(string Name) { this.Name = Name; }
    }
}
