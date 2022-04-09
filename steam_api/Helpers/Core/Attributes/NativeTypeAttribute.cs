using System;
using System.Collections.Generic;
using System.Text;

namespace Steam4NET.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    class NativeTypeAttribute : Attribute
    {
        public Type NativeType { get; set; }

        public NativeTypeAttribute(Type t)
        {
            NativeType = t;
        }
    }
}
