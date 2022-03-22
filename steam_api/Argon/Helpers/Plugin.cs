using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Core.Interface
{
    public class Plugin
    {
        public class InterfaceDelegates
        {
            /// <summary>
            /// Refer to <see cref="DelegateAttribute.Name"/>
            /// </summary>
            public string name;

            /// <summary>
            /// Delegate types that were extracted from the class at runtime
            /// </summary>
            public List<Type> delegate_types;

            public InterfaceDelegates()
            {
                delegate_types = new List<Type>();
            }
        }

        public class InterfaceImpl
        {
            /// <summary>
            /// Refer to <see cref="ImplAttribute.Name"/>
            /// </summary>
            public string name;

            /// <summary>
            /// Methods that were extracted from the class at runtime
            /// </summary>
            public List<MethodInfo> methods;

            /// <summary>
            /// The runtime type of this class (used in <see cref="Loader.CreateContext"/> to create an instance of this interface)
            /// </summary>
            public Type this_type;

            /// <summary>
            /// Delegates that are allocated by this interface at runtime
            /// </summary>
            public List<List<Delegate>> stored_delegates;

            /// <summary>
            /// Handles to memory that are used by this interface for storing unmanaged function pointers
            /// </summary>
            public List<IntPtr> stored_function_pointers;

            /// <summary>
            /// Handles to memory that are used by this interface for storing unmanaged context pointers
            /// </summary>
            public List<IntPtr> stored_contexts;

            public InterfaceImpl()
            {
                methods = new List<MethodInfo>();
                stored_delegates = new List<List<Delegate>>();
                stored_function_pointers = new List<IntPtr>();
                stored_contexts = new List<IntPtr>();
            }
        }

        public class InterfaceMap : InterfaceImpl
        {
        }

        /// <summary>
        /// Name of this plugin
        /// </summary>
        public string name;

        /// <summary>
        /// Interface delegates contained within this plugin
        /// </summary>
        public List<InterfaceDelegates> interface_delegates;

        /// <summary>
        /// Interface implementations contained within this plugin
        /// </summary>
        public List<InterfaceImpl> interface_impls;

        public List<InterfaceMap> interface_maps;


        public Plugin()
        {
            interface_delegates = new List<InterfaceDelegates>();
            interface_impls = new List<InterfaceImpl>();
            interface_maps = new List<InterfaceMap>();
        }
    }
}
