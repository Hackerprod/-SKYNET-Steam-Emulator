using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class InterfaceAttribute : Attribute
    {
        public InterfaceAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Name that this interface wants to be exported as
        /// </summary>
        public string Name { get; set; }
    }
}
