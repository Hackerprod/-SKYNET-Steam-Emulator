using SKYNET.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SKYNET
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

        public static void Write(object v)
        {
            Log.Write(v);
        }
    }

    public class IBaseInterfaceMap : IBaseInterface
    {
    }
}
