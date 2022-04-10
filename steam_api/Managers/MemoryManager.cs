using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class MemoryManager
    {
        public static List<System.Delegate> StoredDelegates;

        public static string ErrorMessage;

        static MemoryManager()
        {
            StoredDelegates = new List<System.Delegate>();
        }

        public static T CreateInterface<T>(out IntPtr MemoryAddress)
        {
            var (instance, memoryAddress) = CreateInterface<T>();
            MemoryAddress = memoryAddress;
            return (T)instance;
        }

        public static IntPtr CreateInterface(Type type)
        {
            CleanErrorMessage();

            string Name = type.Name;
            var Instance = Activator.CreateInstance(type);
            var new_delegates = new List<System.Delegate>();

            try
            {
                var Methods = InterfaceMethodsForType(type);

                foreach (var methodInfo in Methods)
                {
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
                        SteamEmulator.Write(ErrorMessage);
                        return IntPtr.Zero;
                    }
                }

                var ptr_size = Marshal.SizeOf(typeof(IntPtr));

                var vtable = Marshal.AllocHGlobal(Methods.Count * ptr_size);

                for (var i = 0; i < new_delegates.Count; i++)
                {
                    try
                    {
                        Marshal.WriteIntPtr(vtable, i * ptr_size, Marshal.GetFunctionPointerForDelegate(new_delegates[i]));
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Error Injecting Delegate {new_delegates[i]} in {Name}: {ex.Message}";
                        SteamEmulator.Write(ErrorMessage);
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

        public static (T, IntPtr) CreateInterface<T>()
        {
            CleanErrorMessage();

            Type type = typeof(T);
            string Name = type.Name;
            var Instance = Activator.CreateInstance(type);
            var new_delegates = new List<System.Delegate>();

            try
            {
                var Methods = InterfaceMethodsForType(type);

                foreach (var methodInfo in Methods)
                {
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
                        SteamEmulator.Write(ErrorMessage);
                        return (default, IntPtr.Zero);
                    }
                }

                var ptr_size = Marshal.SizeOf(typeof(IntPtr));

                var vtable = Marshal.AllocHGlobal(Methods.Count * ptr_size);

                for (var i = 0; i < new_delegates.Count; i++)
                {
                    try
                    {
                        Marshal.WriteIntPtr(vtable, i * ptr_size, Marshal.GetFunctionPointerForDelegate(new_delegates[i]));
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Error Injecting Delegate {new_delegates[i]} in {Name}: {ex.Message}";
                        SteamEmulator.Write(ErrorMessage);
                    }
                }

                var new_context = Marshal.AllocHGlobal(ptr_size);

                Marshal.WriteIntPtr(new_context, vtable);

                StoredDelegates.AddRange(new_delegates);

                return ((T)Instance, new_context);

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message + " " + ex.StackTrace;
            }

            return ((T)Instance, IntPtr.Zero);
        }

        public static T CreateInterface<T>(IntPtr Address)
        {
            CleanErrorMessage();

            Type type = typeof(T);
            string Name = type.Name;
            var Instance = Activator.CreateInstance(type);
            var new_delegates = new List<System.Delegate>();

            try
            {
                var Methods = InterfaceMethodsForType(type);

                foreach (var methodInfo in Methods)
                {
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
                        return default;
                    }
                }

                var ptr_size = Marshal.SizeOf(typeof(IntPtr));

                var vtable = Marshal.AllocHGlobal(Methods.Count * ptr_size);

                for (var i = 0; i < new_delegates.Count; i++)
                {
                    try
                    {
                        Marshal.WriteIntPtr(vtable, i * ptr_size, Marshal.GetFunctionPointerForDelegate(new_delegates[i]));
                    }
                    catch (Exception)
                    {
                        ErrorMessage += $"Error Injecting Delegate {new_delegates[i]}\n";
                    }
                }

                Marshal.WriteIntPtr(Address, vtable);

                StoredDelegates.AddRange(new_delegates);

                return (T)Instance;

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message + " " + ex.StackTrace;
            }

            return default;
        }
        public static bool CreateInterface(object Instance, IntPtr Address)
        {
            CleanErrorMessage();
            bool successResult = true;

            Type type = Instance.GetType();
            string Name = type.Name;
            var new_delegates = new List<System.Delegate>();

            try
            {
                var Methods = InterfaceMethodsForType(type);

                foreach (var methodInfo in Methods)
                {
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
                    }
                }

                var ptr_size = Marshal.SizeOf(typeof(IntPtr));

                var vtable = Marshal.AllocHGlobal(Methods.Count * ptr_size);

                for (var i = 0; i < new_delegates.Count; i++)
                {
                    try
                    {
                        Marshal.WriteIntPtr(vtable, i * ptr_size, Marshal.GetFunctionPointerForDelegate(new_delegates[i]));
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage += $"Error Injecting Delegate {new_delegates[i]}\n";
                        successResult = false;
                    }
                }

                Marshal.WriteIntPtr(Address, vtable);

                StoredDelegates.AddRange(new_delegates);

                return successResult;

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message + " " + ex.StackTrace;
            }

            return false;
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
            return del.CreateType(); ;
        }

        public static bool SaveDelegates(Type type, string filename)
        {
            try
            {
                string Name = type.Name;
                AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Name), AssemblyBuilderAccess.RunAndSave);
                ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule(Name, Name + ".dll");

                var Methods = InterfaceMethodsForType(type);

                foreach (var methodInfo in Methods)
                {
                    TypeBuilder del = moduleBuilder.DefineType(methodInfo.Name, TypeAttributes.Class | TypeAttributes.Sealed, typeof(MulticastDelegate));

                    CustomAttributeBuilder unmanagedPointer = new CustomAttributeBuilder(typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new[] { typeof(CallingConvention) }), new object[] { CallingConvention.ThisCall });
                    del.SetCustomAttribute(unmanagedPointer);

                    MethodAttributes ctorAttr = MethodAttributes.RTSpecialName | MethodAttributes.Public;
                    ConstructorBuilder ctor = del.DefineConstructor(ctorAttr, CallingConventions.Standard, new Type[] { typeof(object), typeof(System.IntPtr) });
                    ctor.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

                    Type[] parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();

                    MethodBuilder invokeMethod = del.DefineMethod("Invoke", methodInfo.Attributes & ~MethodAttributes.Abstract, methodInfo.ReturnType, parameterTypes);
                    invokeMethod.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
                    del.CreateType();
                }
                asmBuilder.Save(Name + ".dll");
                File.Copy(Name + ".dll", filename);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message + " " + ex.StackTrace;
                return false;
            }
        }

        public static List<MethodInfo> InterfaceMethodsForType(Type t)
        {
            var all_methods = new List<MethodInfo>(t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            all_methods.RemoveAll(x => x.Name.StartsWith("get_") || x.Name.StartsWith("set_"));
            return all_methods;
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
