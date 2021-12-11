
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace Steamworks
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class CallbackIdentityAttribute : Attribute 
	{
		private int _Identity_k__BackingField; 
	
		public int Identity { get; set; }

        public CallbackIdentityAttribute(int callbackNum) { _Identity_k__BackingField = callbackNum; }
        public CallbackIdentityAttribute() { }
    }
}
