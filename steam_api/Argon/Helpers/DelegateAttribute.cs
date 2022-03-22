using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interface
{
    /// <summary>
    /// Used to signal to <see cref="Loader"/> that this class is used for interface delegates
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class DelegateAttribute : Attribute
    {
        /// <summary>
        /// General name of the interfaces that use these delegates
        /// </summary>
        public string Name { get; set; }
    }
}
