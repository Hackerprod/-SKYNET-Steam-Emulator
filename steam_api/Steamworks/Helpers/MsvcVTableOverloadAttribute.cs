using System;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>
    /// Describes one C++ overload set whose members use distinct flat-API names
    /// in managed code. MSVC emits overloads in reverse declaration order in
    /// the interface vtable, so MemoryManager must apply that ABI rule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class MsvcVTableOverloadAttribute : Attribute
    {
        public MsvcVTableOverloadAttribute(params string[] methodNames)
        {
            MethodNames = methodNames ?? throw new ArgumentNullException(nameof(methodNames));
        }

        public string[] MethodNames { get; }
    }
}
