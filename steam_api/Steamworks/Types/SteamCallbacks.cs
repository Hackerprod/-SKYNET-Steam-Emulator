using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct HTTPRequestCompleted_t
    {
        public const int k_iCallback = Constants.k_iClientHTTPCallbacks + 1;

        // Handle value for the request that has completed.
        public HTTPRequestHandle m_hRequest;

        // Context value that the user defined on the request that this callback is associated with, 0 if
        // no context value was set.
        public ulong m_ulContextValue;

        // This will be true if we actually got any sort of response from the server (even an error).
        // It will be false if we failed due to an internal error or client side network failure.
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bRequestSuccessful;

        // Will be the HTTP status code value returned by the server, k_EHTTPStatusCode200OK is the normal
        // OK response, if you get something else you probably need to treat it as a failure.
        public EHTTPStatusCode m_eStatusCode;

        public uint m_unBodySize; // Same as GetHTTPResponseBodySize()
    }
}
