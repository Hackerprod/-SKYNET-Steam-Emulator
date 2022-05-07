using SKYNET.Callback;
using Steamworks.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Net
{
    internal class Callback_test
    {
        private CallResult<SetPersonaNameResponse_t> SetPersonaNameResponse;
        private Callback<PersonaStateChange_t> m_PersonaStateChange;

        public Callback_test()
        {
            SetPersonaNameResponse = CallResult<SetPersonaNameResponse_t>.Create(OnSetPersonaNameResponse);
            m_PersonaStateChange = Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);
        }

        public void SetPersonaName(string newName)
        {
            SteamAPICall_t handle = SteamEmulator.SteamFriends.SetPersonaName(newName);
            //SetPersonaNameResponse.Set(handle);
        }

        private void OnSetPersonaNameResponse(SetPersonaNameResponse_t param, bool bIOFailure)
        {
            Console.WriteLine($"PersonaNameResponse called with result {param.m_result}");
        }

        private void OnPersonaStateChange(PersonaStateChange_t pCallback)
        {
            Console.WriteLine($"Callback:   PersonaStateChange {pCallback.m_ulSteamID} {pCallback.m_nChangeFlags}");
        }
    }
}
