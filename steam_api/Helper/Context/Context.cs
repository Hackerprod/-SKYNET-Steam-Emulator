using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Core;
using Core.Interface;
using SKYNET;
using SKYNET.Helper;

namespace Core.Interface
{
    public class Context
    {

        /// <summary>
        /// Create an instance based on this implementation
        /// </summary>
        /// <param name="impl"></param>
        /// <returns></returns>
        public static object CreateInterfaceInstance(Plugin.InterfaceImpl impl)
        {
            return Activator.CreateInstance(impl.this_type);
        }

        /// <summary>
        /// Create a new implementation context based on the delegate types and implementation
        /// </summary>
        /// <param name="iface"></param>
        /// <param name="impl"></param>
        /// <returns></returns>
        private static (IntPtr, IBaseInterface) Create(Plugin.InterfaceDelegates iface, Plugin.InterfaceImpl impl)
        {
            var instance = CreateInterfaceInstance(impl);

            var new_delegates = new List<Delegate>();

            for (var i = 0; i < impl.methods.Count; i++)
            {
                // Find the delegate type that matches the method
                var mi = impl.methods[i];

                var type = iface.delegate_types.Find(x => x.Name.Equals(mi.Name));

                if (type == null)
                {
                    Log.Write(string.Format("Unable to find delegate for {0} in {1}! (maybe you need to regen autogen?)", mi.Name, iface.name));
                    return (IntPtr.Zero, null);
                }

                // Create new delegates that are bounded to this instance
                Delegate new_delegate;
                try
                {
                    new_delegate = Delegate.CreateDelegate(type, instance, mi, true);
                }
                catch (Exception e)
                {
                    Log.Write(string.Format("EXCEPTION whilst binding function {0}", mi.Name));
                    throw e;
                }

                new_delegates.Add(new_delegate);
            }

            impl.stored_delegates.Add(new_delegates);


            // Create a new context (class) that mimics what the C++ compiler would generate

            // class:
            //  vtable:
            //   1
            //   2
            //   3
            //   ...

            var ptr_size = Marshal.SizeOf(typeof(IntPtr));

            // Allocate enough space for the new pointers in local memory
            var vtable = Marshal.AllocHGlobal(impl.methods.Count * ptr_size);

            for (var i = 0; i < new_delegates.Count; i++)
            {
                // Create all function pointers as neccessary
                Marshal.WriteIntPtr(vtable, i * ptr_size, Marshal.GetFunctionPointerForDelegate(new_delegates[i]));
            }

            impl.stored_function_pointers.Add(vtable);

            // create the context
            var new_context = Marshal.AllocHGlobal(ptr_size);

            // Write the pointer to the vtable at the address pointed to by new_context;
            Marshal.WriteIntPtr(new_context, vtable);

            return (new_context, (IBaseInterface)instance);
        }

        /// <summary>
        /// Get an interface implementation by its exported name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Plugin.InterfaceImpl FindInterfaceImpl(string name)
        {
            foreach (var p in InterfaceManager.LoadedPlugins)
            {
                foreach (var impl in p.interface_impls)
                {
                    if (impl.name == name)
                    {
                        return impl;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get interface delegates by their exported name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Plugin.InterfaceDelegates FindInterfaceDelegates(string name)
        {
            foreach (var p in InterfaceManager.LoadedPlugins)
            {
                foreach (var dels in p.interface_delegates)
                {
                    if (dels.name == name)
                    {
                        return dels;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Find a map implementation for the interface name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Plugin.InterfaceMap FindInterfaceMap(string name)
        {
            foreach (var p in InterfaceManager.LoadedPlugins)
            {
                foreach (var m in p.interface_maps)
                {
                    if (m.name == name)
                    {
                        return m;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Find an implementation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Plugin.InterfaceImpl FindImpl(string name)
        {
            var impl = FindInterfaceImpl(name);

            if (impl == null)
            {
                Log.Write(string.Format("Unable to find implementation for interface {0}", name));
                return null;
            }

            return impl;
        }

        /// <summary>
        /// Find a map
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Plugin.InterfaceMap FindMap(string name)
        {
            var map = FindInterfaceMap(name);

            if (map == null)
            {
                Log.Write(string.Format("Unable to find map for interface {0}", name));
                return null;
            }
            return map;
        }

        /// <summary>
        /// Create an interface instance and context and whether to try and create a map
        /// </summary>
        /// <param name="name"></param>
        /// <param name="try_create_map"></param>
        /// <returns></returns>
        public static (IntPtr, IBaseInterface, bool) CreateInterface(string name, bool try_create_map = false)
        {
            // TODO: this really should "try" create map this should fail if it cant find a map and we want a map
            // Otherwise we are going to get unexpected behaviour
            // But then again some createinterfaces dont know whether they are going to be a map or not so...

            // Ensure that we are loaded before trying to query loaded plugins

            Plugin.InterfaceImpl impl = null;

            var is_map = try_create_map;

            if (try_create_map)
            {
                impl = FindInterfaceMap(name);
            }

            // Not trying to find a map or we couldnt find one (in the case that an interface is not servermapped)
            if (impl == null)
            {
                is_map = false;

                impl = FindImpl(name);
            }

            if (impl == null)
            {
                Log.Write(string.Format("Unable to find map or impl for interface {0}", name));
                return (IntPtr.Zero, null, false);
            }

            var iface = FindInterfaceDelegates(impl.name);

            if (iface == null)
            {
                Log.Write(string.Format("Unable to find delegates for interface that implements {0}", impl.name));
                return (IntPtr.Zero, null, false);
            }

            // Try to create a new context based on this interface + impl pair
            var (context, instance) = Create(iface, impl);
            return (context, instance, is_map);
        }
    }
}
