using System;

namespace SKYNET.Steamworks.Interfaces
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class InterfaceAttribute : Attribute
    {
        public string Name { get; set; }

        public InterfaceAttribute(string Name) { this.Name = Name; }
    }
}
