using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helper
{
    public class RegistrySettings
    {
        public event EventHandler OnKeyEmpty;
        public event EventHandler<Exception> OnError;
        private bool keyExists;

        private RegistryKey Key;

        public RegistrySettings(string subKey)
        {
            keyExists = true;
            Key = Registry.CurrentUser.OpenSubKey(subKey, true);
            if (Key == null)
            {
                keyExists = false;
                OnKeyEmpty?.Invoke(this, new EventArgs());
                Registry.CurrentUser.CreateSubKey(subKey);
                Key = Registry.CurrentUser.OpenSubKey(subKey, true);
            }
        }
        public T Get<T>(string name, object defaultValue)
        {
            try
            {
                object Value = Key.GetValue(name);
                Type PropertyType = typeof(T);
                object Result = null;

                if (Value == null)
                {
                    Set(name, defaultValue);
                    return (T)defaultValue;
                }

                if (PropertyType == typeof(string))
                {
                    Result = Value.ToString();
                }
                else if (PropertyType.IsPrimitive)
                {
                    Result = Convert.ChangeType(Value.ToString(), PropertyType, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (PropertyType == typeof(bool))
                {
                    Result = bool.Parse(Value.ToString());
                }
                else if (PropertyType == typeof(IPAddress))
                {
                    Result = IPAddress.Parse(Value.ToString());
                }
                else if (PropertyType == typeof(Enum))
                {
                    Result = Value;
                }

                return (T)Result;
            }
            catch (Exception)
            {

            }
            return (T)defaultValue;
        }

        public bool KeyExists()
        {
            return keyExists;
        }

        public void Set(string name, object val)
        {
            Key.SetValue(name, val);
            try
            {
                if (val is Enum)
                {
                    Key.SetValue(name, (int)val);
                }
                else
                    Key.SetValue(name, val);

            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
            }
        }
    }
}
