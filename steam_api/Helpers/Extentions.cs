using SKYNET.Callback;
using SKYNET.Steamworks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.Helper
{
    public static class Extentions
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

        public static bool IsGameServer(this CCallbackBase Base)
        {
            bool GS = false;
            try
            {
                GS = (Base.m_nCallbackFlags & CCallbackBase.k_ECallbackFlagsGameServer) != 0;
                return GS;
            }
            catch (Exception)
            {
                return GS;
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

        public static CallbackType GetCallbackType(this int iCallback)
        {
            int type = 0;
            try
            {
                type = ((iCallback / 100) * 100);
            }
            catch { }
            return (CallbackType)type;
        }

        //public static string Serialize(this NET_Base Base)
        //{
        //    try
        //    {
        //        return new JavaScriptSerializer().Serialize(Base);
        //    }
        //    catch (Exception ex)
        //    {
        //        modCommon.Show(ex);
        //        return default;
        //    }
        //}

        //public static byte[] Serialize(this NetworkMessage Base)
        //{
        //    try
        //    {
        //        string json = new JavaScriptSerializer().Serialize(Base);
        //        return Encoding.Default.GetBytes(json);
        //    }
        //    catch (Exception ex)
        //    {
        //        modCommon.Show(ex);
        //        return default;
        //    }
        //}

        public static T Deserialize<T>(this byte[] bytes)
        {
            try
            {
                string json = Encoding.Default.GetString(bytes);
                T Body = new JavaScriptSerializer().Deserialize<T>(json);
                return (T)Body;
            }
            catch
            {
                return default;
            }
        }

        public static T Deserialize<T>(this string json)
        {
            try
            {
                T Body = new JavaScriptSerializer().Deserialize<T>(json);
                return (T)Body;
            }
            catch
            {
                return default;
            }
        }
    }
}
