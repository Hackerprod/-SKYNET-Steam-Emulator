using SKYNET;
using SKYNET.Interface;
using SKYNET.Managers;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class InterfaceManager
{
    public static List<SteamInterface> Interfaces;
    public static List<SteamInterface> Delegates;
    static InterfaceManager()
    {
        Interfaces = new List<SteamInterface>();
        Delegates = new List<SteamInterface>();
        InitializeDelegates();
    }

    public static void InitializeDelegates()
    {
        
    }

    public static SteamInterface GetInterface(string Name)
    {
        foreach (var inter in Interfaces)
        {
            Type type = inter.GetType();
            var attributes = type.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                InterfaceAttribute attr = (InterfaceAttribute)attribute;
                if (attr != null)
                {
                    if (Name == attr.Name)
                    {
                        return inter;
                    }
                    if (Name.StartsWith(attr.Name))
                    {
                        //Check if is valid version number
                        if (int.TryParse(Name.Replace(attr.Name, ""), out _))
                        {
                            return inter;
                        }
                    }
                }
            }
        }
        return null;
    }
    public static IntPtr CreateInterface(IntPtr ver)
    {
        string version = Marshal.PtrToStringBSTR(ver);
        SteamInterface inter = GetInterface(version);
        return (IntPtr)Activator.CreateInstance(inter.GetType());
    }

    public static IntPtr CreateInterface(IntPtr hSteamUser, IntPtr pszVersion)
    {
        string version = Marshal.PtrToStringBSTR(pszVersion);
        SteamInterface inter = GetInterface(version);
        return (IntPtr)Activator.CreateInstance(inter.GetType());
    }

    public static IntPtr FindOrCreateGameServerInterface(IntPtr hSteamUser, IntPtr pszVersion)
    {
        string version = Marshal.PtrToStringBSTR(pszVersion);
        SteamInterface inter = GetInterface(version);
        return (IntPtr)Activator.CreateInstance(inter.GetType());
    }

    public static IntPtr GetGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
    {
        SteamInterface inter = GetInterface(pchVersion);
        return (IntPtr)Activator.CreateInstance(inter.GetType());
    }
}