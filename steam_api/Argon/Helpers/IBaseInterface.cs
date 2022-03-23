using SKYNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interface
{
    /// <summary>
    /// Interface that all interface implementations must inherit from
    /// </summary>
    public class IBaseInterface
    {
        /// <summary>
        /// Set by <see cref="Client"/> to allow interfaces to know what user they belong too
        /// </summary>
        public int ClientId { get; set; }

        public int InterfaceId { get; set; }

        public int PipeId { get; set; }

        public Plugin.InterfaceImpl Implementation { get; set; }
        public IntPtr BaseAddress { get; set; }
        public string InterfaceVersion { get; set; }

    }

    public class IBaseInterfaceMap : IBaseInterface
    {
    }
}
