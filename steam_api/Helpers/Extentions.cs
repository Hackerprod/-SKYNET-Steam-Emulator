using SKYNET.Callback;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helper
{
    public static class Extentions
    {
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

    }
}
