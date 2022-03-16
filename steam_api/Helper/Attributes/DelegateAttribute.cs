using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DelegateAttribute : Attribute
    {
        public DelegateAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// General name of the interfaces that use these delegates
        /// </summary>
        public string Name { get; set; }
    }
}
