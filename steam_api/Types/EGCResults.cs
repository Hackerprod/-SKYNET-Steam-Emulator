using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public enum EGCResults
    {
        k_EGCResultOK = 0,
        k_EGCResultNoMessage = 1,           // There is no message in the queue
        k_EGCResultBufferTooSmall = 2,      // The buffer is too small for the requested message
        k_EGCResultNotLoggedOn = 3,         // The client is not logged onto Steam
        k_EGCResultInvalidMessage = 4,      // Something was wrong with the message being sent with SendMessage
    };
}
