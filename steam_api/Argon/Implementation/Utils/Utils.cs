using System;

namespace InterfaceUtils
{
    class Utils 
    {
        public Utils()
        {

        }
        public int GetComputerTime()
        {
            return 0;
        }

        public string GetUILanguage()
        {
            return "English";
        }

        public void RunFrame()
        {
            //Write("RunFrame should not be called...");
        }

        public bool APICallFailed(int handle)
        {
            return false;
        }

        public bool APICalledFinished(int handle)
        {
            //return AsyncCallManager.IsCallFinished(handle);
            return true;
        }

        public int APICallFailureReason(int handle)
        {
            //// TODO: this function can only really return reasonable results
            //// if we make this an unmapped interface / function
            //// As this is not the case right now, most of the failure reasons that
            //// exist are irrelevent.

            //// TODO: if we never say that any calls have finished this function is never
            //// going to be called anyway...

            //// Invalid jobid handle
            //if (!AsyncCallManager.IsHandleValid(handle)) return 2;

            //// Connection to steam does not exist
            //if (Instance.Connected == false) return 1;

            // No failure
            return -1;
        }

        public bool APICallResult(int handle, IntPtr callback, int callback_size, int callback_expected)
        {
            //var result = AsyncCallManager.GetCallResult(handle);

            //if (result == null)
            //{
            //    return false;
            //}

            //if (callback_size != result.Length)
            //{
            //    Write("callback_size != result.Length for callback {0}", callback_expected);
            //}

            //System.Runtime.InteropServices.Marshal.Copy(result, 0, callback, callback_size);

            return true;
        }
    }
}