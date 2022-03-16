using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interface
{
    /// <summary>
    /// Used to signal to <see cref="Loader"/> that this class is used for interface implementations
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ImplAttribute : Attribute
    {
        /// <summary>
        /// Name that this interface wants to be exported as
        /// </summary>
        public string Name { get; set; }

        public bool ServerMapped { get; set; }
    }
}
