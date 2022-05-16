using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helper.Export
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class DllExportAttribute : Attribute
    {
        /// <summary>
        /// Specified calling convention.
        ///
        /// __cdecl is the default convention in .NET DllExport like for other C/C++ programs (Microsoft Specific).
        /// __stdCall mostly used with winapi.
        ///
        /// https://msdn.microsoft.com/en-us/library/zkwh89ks.aspx
        /// https://msdn.microsoft.com/en-us/library/56h2zst2.aspx
        /// https://github.com/3F/Conari also uses __cdecl by default
        /// </summary>
        public CallingConvention CallingConvention { get; set; } = CallingConvention.Cdecl;


        /// <summary>
        /// Optional name for C-exported function.
        /// </summary>
        public string ExportName { get; set; }

        /// <param name="function">Optional name for C-exported function.</param>
        /// <param name="convention">Specified calling convention. __cdecl is the default convention in .NET DllExport.</param>
        public DllExportAttribute(string function, CallingConvention convention)
        {
        }

        /// <param name="function">Optional name for C-exported function.</param>
        public DllExportAttribute(string function)
        {
        }

        /// <param name="convention">Specified calling convention. __cdecl is the default convention in .NET DllExport.</param>
        public DllExportAttribute(CallingConvention convention)
        {
        }

        /// <summary>
        /// To export this as __cdecl C-exported function. Named as current method where is used attribute.
        /// </summary>
        public DllExportAttribute()
        {
        }
    }
}
