using System;

namespace SKYNET.Steamworks.Interfaces
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class InterfaceLayoutAttribute : Attribute
    {
        public string Name { get; }
        public string[] MethodNames { get; }

        public InterfaceLayoutAttribute(string Name, params string[] MethodNames)
        {
            this.Name = Name;
            this.MethodNames = MethodNames;
        }
    }
}
