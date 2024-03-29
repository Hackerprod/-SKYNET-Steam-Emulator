﻿using SKYNET.Callback;
using SKYNET.Steamworks;
using System;
using System.Text;
using System.Web.Script.Serialization;

namespace SKYNET.Helpers
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
