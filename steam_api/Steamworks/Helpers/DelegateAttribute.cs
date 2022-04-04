using System;
using System.Collections.Generic;
using System.Text;

namespace SKYNET
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DelegateAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
