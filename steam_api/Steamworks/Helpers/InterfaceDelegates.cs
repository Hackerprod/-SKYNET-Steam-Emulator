using SKYNET;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SKYNET.Delegate.Helper
{
    public class InterfaceDelegates
    {
        public string Name;

        public List<Type> DelegateTypes;

        public InterfaceDelegates()
        {
            DelegateTypes = new List<Type>();
        }
    }
}
