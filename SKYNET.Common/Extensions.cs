using SKYNET.Steamworks;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace SKYNET
{
    public static class Extensions
    {
        public static T CopyTo<T>(this object obj) where T : class
        {
            try
            {
                T instance = Activator.CreateInstance<T>();

                Type sourceType = obj.GetType();
                Type targetType = instance.GetType();

                foreach (var property in sourceType.GetProperties())
                {
                    try
                    {
                        var targetProperty = targetType.GetProperties().ToList().Find(p => p.Name == property.Name);
                        if (targetProperty != null)
                        {
                            var value = property.GetValue(obj, null);
                            targetProperty.SetValue(instance, value);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                return instance;
            }
            catch 
            {
                return default;
            }
        }

        public static uint ToTimestamp(this DateTime d)
        {
            var epoch = d - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            return (uint)epoch.TotalSeconds;
        }

        public static DateTime ToDateTime(this uint timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            dateTime = dateTime.AddSeconds(timestamp);
            return dateTime;
        }

        public static uint GetAccountID(this ulong SteamID)
        {
            try
            {
                return new CSteamID(SteamID).AccountID;
            }
            catch (Exception)
            {
                return (uint)SteamID;
            }
        }

        public static ulong ToSteamID(this uint AccountID)
        {
            try
            {
                return (ulong)new CSteamID(AccountID, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
            }
            catch (Exception)
            {
                return (ulong)AccountID;
            }
        }

        public static string GetString(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        public static byte[] GetBytes(this string @string)
        {
            return Encoding.Default.GetBytes(@string);
        }

        public static byte[] GetBytesFromBase64String(this string @string)
        {
            return Convert.FromBase64String(@string);
        }

        public static T Deserialize<T>(this string @string)
        {
            try
            {
                var js = new JavaScriptSerializer() { MaxJsonLength = 500000000 };
                return js.Deserialize<T>(@string);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static T Deserialize<T>(this byte[] bytes)
        {
            var js = new JavaScriptSerializer() { MaxJsonLength = 500000000 };
            string @string = Encoding.Default.GetString(bytes);
            return js.Deserialize<T>(@string);
        }

        public static string Serialize(this object obj)
        {
            var js = new JavaScriptSerializer() { MaxJsonLength = 500000000 };
            return js.Serialize(obj);
        }


        public static uint Swap(this uint address)
        {
            return ((address & 0x000000ff) << 24) +
                   ((address & 0x0000ff00) << 8) +
                   ((address & 0x00ff0000) >> 8) +
                   ((address & 0xff000000) >> 24);
        }

        public static uint Swap(this long address)
        {
            return ((uint)address).Swap();
        }
    }
}
