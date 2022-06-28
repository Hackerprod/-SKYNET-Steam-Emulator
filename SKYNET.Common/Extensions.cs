using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.Common
{
    public static class Extensions
    {
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

        public static T FromJson<T>(this string @string)
        {
            return new JavaScriptSerializer().Deserialize<T>(@string);
        }

        public static T Deserialize<T>(this byte[] bytes)
        {
            string @string = Encoding.Default.GetString(bytes);
            return new JavaScriptSerializer().Deserialize<T>(@string);
        }

        public static string ToJson(this object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }

        public static string Serialize(this object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
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
