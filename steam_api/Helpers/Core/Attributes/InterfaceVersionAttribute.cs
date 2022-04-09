using System;
using System.Collections.Generic;
using System.Text;

namespace Steam4NET.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    class InterfaceVersionAttribute : Attribute
    {
        public string Identifier { get; set; }

        public InterfaceVersionAttribute(string versionIdentifier)
        {
            Identifier = versionIdentifier;
        }
    }
}
