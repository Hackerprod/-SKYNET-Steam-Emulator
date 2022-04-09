using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steam4NET.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class VTableSlotAttribute : Attribute
    {
        public int Slot { get; set; }

        public VTableSlotAttribute(int s)
        {
            Slot = s;
        }
    }
}
