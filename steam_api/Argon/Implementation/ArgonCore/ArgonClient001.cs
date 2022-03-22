using System;

using Core.Interface;
using Core.IPC;

namespace InterfaceCore
{
    ///// <summary>
    ///// Exports Argon functions to unmanaged code
    ///// </summary>
    //[Impl(Name = "ArgonClient001", ServerMapped = false)]
    //public class ArgonClient001 : IBaseInterface
    //{
    //    /// <summary>
    //    /// Linked to <see cref="Client.CreateInterfaceNoUser(string)"/>
    //    /// </summary>
    //    /// <param name="name"></param>
    //    public IntPtr CreateInterface(IntPtr _, int pipe_id, string name)
    //    {
    //        Console.WriteLine("Core001.CreateInterface({0})", name);
    //        return Client.Client.CreateInterfaceNoUser(pipe_id, name);
    //    }

    //    public IntPtr CreateInterfaceNoPipe(IntPtr _, string name)
    //    {
    //        Console.WriteLine("Core001.CreateInterfaceNoPipe({0})", name);
    //        return Client.Client.CreateInterfaceNoUserNoPipe(name);
    //    }

    //    /// <summary>
    //    /// Linked to <see cref="Client.GetCallback"/>
    //    /// </summary>
    //    /// <param name="pipe"></param>
    //    /// <param name="c"></param>
    //    /// <returns>Whether there is a new callback</returns>
    //    public bool GetCallback(IntPtr _, int pipe_id, ref CallbackMsg c)
    //    {
    //        var new_callback = Client.Client.GetCallback(pipe_id);

    //        if (new_callback == null) return false;

    //        c = (CallbackMsg)new_callback;

    //        return true;
    //    }

    //    /// <summary>
    //    /// Frees the last callback to this pipe
    //    /// </summary>
    //    /// <param name="c"></param>
    //    public void FreeLastCallback(IntPtr _, int pipe_id)
    //    {
    //        Client.Client.FreeCallback(pipe_id);
    //    }
    //}
}
