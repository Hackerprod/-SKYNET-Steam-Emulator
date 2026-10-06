using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace SKYNET.Managers
{
    public class MemoryManager
    {
        public static List<Delegate> StoredDelegates;

        public static string ErrorMessage;

        static MemoryManager()
        {
            StoredDelegates = new List<Delegate>();
        }

        public static IntPtr CreateInterface(Type type, List<MethodInfo> Methods)
        {
            CleanErrorMessage();

            string Name = type.Name;
            var Instance = Activator.CreateInstance(type);
            var new_delegates = new List<Delegate>();

            try
            {
                foreach (var methodInfo in Methods)
                {
                    Type DelegateType = CreateDelegate(methodInfo);

                    Delegate new_delegate = null;
                    try
                    {
                        new_delegate = Delegate.CreateDelegate(DelegateType, Instance, methodInfo, true);
                        new_delegates.Add(new_delegate);
                    }
                    catch (Exception e)
                    {
                        ErrorMessage = ($"Exception whilst binding function {methodInfo.Name}, class {Name} - {e.Message} {e.StackTrace}");
                        SteamEmulator.Write("MemoryManager", ErrorMessage);
                        return IntPtr.Zero;
                    }
                }

                var ptr_size = Marshal.SizeOf(typeof(IntPtr));

                var vtable = Marshal.AllocHGlobal(Methods.Count * ptr_size);

                for (var i = 0; i < new_delegates.Count; i++)
                {
                    try
                    {
                        var FunctionPointer = Marshal.GetFunctionPointerForDelegate(new_delegates[i]);
                        Marshal.WriteIntPtr(vtable, i * ptr_size, FunctionPointer);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Error Writing Delegate ({new_delegates[i]} in {Name}) into Process: {ex.Message}";
                        SteamEmulator.Write("MemoryManager", ErrorMessage);
                    }
                }

                var new_context = Marshal.AllocHGlobal(ptr_size);

                Marshal.WriteIntPtr(new_context, vtable);

                StoredDelegates.AddRange(new_delegates);

                return new_context;

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message + " " + ex.StackTrace;
            }

            return IntPtr.Zero;
        }

        public static IntPtr CreateMethod(Object Instance, MethodInfo methodInfo)
        {
            Type type = Instance.GetType();
            string Name = type.Name;
            var new_delegates = new List<System.Delegate>();
            Type DelegateType = CreateDelegate(methodInfo);

            System.Delegate new_delegate = null;
            try
            {
                new_delegate = System.Delegate.CreateDelegate(DelegateType, Instance, methodInfo, true);
                new_delegates.Add(new_delegate);
            }
            catch (Exception e)
            {
                ErrorMessage = ($"EXCEPTION whilst binding function {methodInfo.Name}, class {Name} - {e.Message} {e.StackTrace}");
                SteamEmulator.Write("MemoryManager", ErrorMessage);
                return IntPtr.Zero;
            }

            var ptr_size = Marshal.SizeOf(typeof(IntPtr));

            var vtable = Marshal.AllocHGlobal(1 * ptr_size);

            for (var i = 0; i < new_delegates.Count; i++)
            {
                try
                {
                    Marshal.WriteIntPtr(vtable, i * ptr_size, Marshal.GetFunctionPointerForDelegate(new_delegates[i]));
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error Injecting Delegate {new_delegates[i]} in {Name}: {ex.Message}";
                    SteamEmulator.Write("MemoryManager", ErrorMessage);
                }
            }

            var new_context = Marshal.AllocHGlobal(ptr_size);

            Marshal.WriteIntPtr(new_context, vtable);

            StoredDelegates.AddRange(new_delegates);

            return new_context;
        }

        public static T GetFromMemory<T>(IntPtr mAddress) where T : struct
        {
            try
            {
                return Marshal.PtrToStructure<T>(mAddress);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message + " " + ex.StackTrace;
                return default;
            }
        }

        public static Type CreateDelegate(MethodInfo methodInfo)
        {
            string Name = methodInfo.Name;
            AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Name), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule(Name, Name + ".dll");

            TypeBuilder del = moduleBuilder.DefineType(methodInfo.Name, TypeAttributes.Class | TypeAttributes.Sealed, typeof(MulticastDelegate));

            CustomAttributeBuilder unmanagedPointer = new CustomAttributeBuilder(typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new[] { typeof(CallingConvention) }), new object[] { CallingConvention.ThisCall });
            del.SetCustomAttribute(unmanagedPointer);

            MethodAttributes ctorAttr = MethodAttributes.RTSpecialName | MethodAttributes.Public;
            ConstructorBuilder ctor = del.DefineConstructor(ctorAttr, CallingConventions.Standard, new Type[] { typeof(object), typeof(System.IntPtr) });
            ctor.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            Type[] parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();

            MethodBuilder invokeMethod = del.DefineMethod("Invoke", methodInfo.Attributes & ~MethodAttributes.Abstract, methodInfo.ReturnType, parameterTypes);
            invokeMethod.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
            return del.CreateType();
        }

        public static List<MethodInfo> ResolveLayoutMethods(Type type, string version, string[] methodNames)
        {
            var declared = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var resolved = new List<MethodInfo>(methodNames.Length);
            foreach (var methodName in methodNames)
            {
                var matches = declared.Where(method => method.Name == methodName).ToArray();
                if (matches.Length != 1)
                {
                    throw new InvalidOperationException(
                        $"{type.FullName} layout '{version}' method '{methodName}' resolved to {matches.Length} public methods (expected exactly 1).");
                }

                resolved.Add(matches[0]);
            }

            return resolved;
        }

        /// <summary>
        /// Serializes a structure to a byte array.
        /// </summary>
        /// <param name="value">The structure to serialize to a byte array.</param>
        /// <returns>The object serialized to a byte array, ready to write to a stream.</returns>
        public static byte[] Serialize<T>(T value)
        {
            // Get the size of our structure in bytes
            var structSize = Marshal.SizeOf(value);

            // This will contain the result, and be returned
            var bytes = new byte[structSize];

            // Allocate some unmanaged memory for our structure
            var pointer = IntPtr.Zero;

            try
            {
                pointer = Marshal.AllocHGlobal(structSize);

                // Write the structure to the unmanaged memory
                Marshal.StructureToPtr(value, pointer, false);

                // Copy the resulting bytes from unmanaged memory to our result array
                Marshal.Copy(pointer, bytes, 0, structSize);

                return bytes;
            }
            finally
            {
                if (pointer != IntPtr.Zero)
                {
                    // Free up our unmanaged memory
                    Marshal.FreeHGlobal(pointer);
                }
            }
        }

        /// <summary>
        /// Deserializes a structure from a byte array.
        /// </summary>
        /// <param name="data">The object binary data to deserialize from.</param>
        /// <returns>The deserialized structure.</returns>
        public static T Deserialize<T>(byte[] data)
        {
            var pinnedPacket = new GCHandle();

            T result;

            try
            {
                pinnedPacket = GCHandle.Alloc(data, GCHandleType.Pinned);
                result = (T)Marshal.PtrToStructure(pinnedPacket.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                pinnedPacket.Free();
            }

            return result;
        }

        public static object BytesToDataStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);

            if (size > bytes.Length)
            {
                return null;
            }

            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, structPtr, size);
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }

        public static byte[] StructToBytes(object anyStruct)
        {
            int size = Marshal.SizeOf(anyStruct);
            IntPtr bytesPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(anyStruct, bytesPtr, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(bytesPtr, bytes, 0, size);
            Marshal.FreeHGlobal(bytesPtr);

            return bytes;
        }

        private static void CleanErrorMessage()
        {
            ErrorMessage = "";
        }
    }
}
