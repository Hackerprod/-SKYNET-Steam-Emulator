using System;

namespace SKYNET.Callback
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
    internal class CallbackIdentityAttribute : Attribute
    {
        public int Identity { get; set; }

        public CallbackIdentityAttribute(int callbackNum)
        {
            Identity = callbackNum;
        }
    }
}