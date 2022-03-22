using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Core;
using Core.Interface;
using SKYNET;

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
                //Write($"Finding delegate for type {mi.Name}");

                if (type == null)
                {
                    Write(string.Format("Unable to find delegate for {0} in {1}! (maybe you need to regen autogen?)", mi.Name, iface.name));
                    return (IntPtr.Zero, null);
                }

                // Create new delegates that are bounded to this instance
                Delegate new_delegate;
                try
                {
                    new_delegate = Delegate.CreateDelegate(type, instance, mi, true);
                    new_delegates.Add(new_delegate);
                }
                catch (Exception e)
                {
                    Write(string.Format("EXCEPTION whilst binding function {0}, class {1}", mi.Name, impl) + Environment.NewLine + e.Message);
                }
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
                // Main.Write ("Testing " + new_delegates[i].Method);
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
            Loader.Load();

            foreach (var p in Loader.LoadedPlugins)
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
            Loader.Load();

            foreach (var p in Loader.LoadedPlugins)
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
            Loader.Load();

            foreach (var p in Loader.LoadedPlugins)
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
                Write(string.Format("Unable to find implementation for interface {0}", name));
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
                Write(string.Format("Unable to find map for interface {0}", name));
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
        public static (IntPtr, IBaseInterface) CreateInterface(string name)
        {
            // TODO: this really should "try" create map this should fail if it cant find a map and we want a map
            // Otherwise we are going to get unexpected behaviour
            // But then again some createinterfaces dont know whether they are going to be a map or not so...

            // Ensure that we are loaded before trying to query loaded plugins
            Loader.Load();

            Plugin.InterfaceImpl impl = null;

            // Not trying to find a map or we couldnt find one (in the case that an interface is not servermapped)
            if (impl == null)
            {
                impl = FindImpl(name);
            }

            if (impl == null)
            {
                Write(string.Format("Unable to find map or impl for interface {0}", name));
                return (IntPtr.Zero, null);
            }

            var iface = FindInterfaceDelegates(impl.name);

            if (iface == null)
            {
                Write(string.Format("Unable to find delegates for interface that implements {0}", impl.name));
                return (IntPtr.Zero, null);
            }

            // Try to create a new context based on this interface + impl pair
            var (context, instance) = Create(iface, impl);
            return (context, instance);
        }

        public static (IntPtr, IBaseInterface) CreateInterface(Type type)
        {
            string Name = type.ToString();

            var iface = FindInterfaceDelegates(Name);

            if (iface == null)
            {
                Write(string.Format("Unable to find delegates for interface that implements {0}", Name));
                return (IntPtr.Zero, null);
            }

            var impl = new Plugin.InterfaceImpl
            {
                name = Name,
                this_type = type,
                methods = Loader.InterfaceMethodsForType(type)
            };

            // Try to create a new context based on this interface + impl pair
            var (context, instance) = Create(iface, impl);

            return (context, instance);
        }

        private static void Write(string v)
        {
            Main.Write("Context", v);
        }
    }
}
