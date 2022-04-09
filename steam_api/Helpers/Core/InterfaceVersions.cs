using System;
using Steam4NET.Attributes;

namespace Steam4NET.Core
{
    public class InterfaceVersions
    {
        public static string GetInterfaceIdentifier(Type targetClass)
        {
            foreach (InterfaceVersionAttribute attribute in targetClass.GetCustomAttributes(typeof(InterfaceVersionAttribute), false))
            {
                return attribute.Identifier;
            }

            throw new Exception("Version identifier not found for class " + targetClass);
        }
    }
}
