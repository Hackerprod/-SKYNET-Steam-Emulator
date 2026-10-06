using SKYNET.Helpers;
using System;
using System.Collections.Generic;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("SteamBilling002",
        "_unknown_fn_1", "_unknown_fn_2", "_unknown_fn_3", "_unknown_fn_4",
        "_unknown_fn_5", "_unknown_fn_6", "_unknown_fn_7", "_unknown_fn_8",
        "_unknown_fn_9", "_unknown_fn_10", "_unknown_fn_11", "_unknown_fn_12",
        "_unknown_fn_13", "_unknown_fn_14", "_unknown_fn_15", "_unknown_fn_16",
        "_unknown_fn_17", "_unknown_fn_18", "_unknown_fn_19", "_unknown_fn_20",
        "_unknown_fn_21", "_unknown_fn_22", "_unknown_fn_23", "_unknown_fn_24",
        "_unknown_fn_25", "_unknown_fn_26", "_unknown_fn_27", "_unknown_fn_28",
        "_unknown_fn_29", "_unknown_fn_30", "_unknown_fn_31", "_unknown_fn_32",
        "_unknown_fn_33", "_unknown_fn_34", "_unknown_fn_35", "_unknown_fn_36",
        "_unknown_fn_37", "_unknown_fn_38", "_unknown_fn_39", "_unknown_fn_40",
        "_unknown_fn_41", "_unknown_fn_42", "_unknown_fn_43")]
    public class SteamBilling002 : ISteamInterface
    {
        private static readonly HashSet<string> _stubsLogged = new HashSet<string>();

        private static void LogStub(string method)
        {
            lock (_stubsLogged)
            {
                if (!_stubsLogged.Add(method))
                {
                    return;
                }
            }

            SteamEmulator.Write("SteamBilling", method + " not implemented");
        }

        public bool _unknown_fn_1(IntPtr _)
        {
            LogStub("_unknown_fn_1");
            return false;
        }

        public bool _unknown_fn_2(IntPtr _)
        {
            LogStub("_unknown_fn_2");
            return false;
        }

        public bool _unknown_fn_3(IntPtr _)
        {
            LogStub("_unknown_fn_3");
            return false;
        }

        public bool _unknown_fn_4(IntPtr _)
        {
            LogStub("_unknown_fn_4");
            return false;
        }

        public bool _unknown_fn_5(IntPtr _)
        {
            LogStub("_unknown_fn_5");
            return false;
        }

        public bool _unknown_fn_6(IntPtr _)
        {
            LogStub("_unknown_fn_6");
            return false;
        }

        public bool _unknown_fn_7(IntPtr _)
        {
            LogStub("_unknown_fn_7");
            return false;
        }

        public bool _unknown_fn_8(IntPtr _)
        {
            LogStub("_unknown_fn_8");
            return false;
        }

        public bool _unknown_fn_9(IntPtr _)
        {
            LogStub("_unknown_fn_9");
            return false;
        }

        public bool _unknown_fn_10(IntPtr _)
        {
            LogStub("_unknown_fn_10");
            return false;
        }

        public bool _unknown_fn_11(IntPtr _)
        {
            LogStub("_unknown_fn_11");
            return false;
        }

        public bool _unknown_fn_12(IntPtr _)
        {
            LogStub("_unknown_fn_12");
            return false;
        }

        public bool _unknown_fn_13(IntPtr _)
        {
            LogStub("_unknown_fn_13");
            return false;
        }

        public bool _unknown_fn_14(IntPtr _)
        {
            LogStub("_unknown_fn_14");
            return false;
        }

        public bool _unknown_fn_15(IntPtr _)
        {
            LogStub("_unknown_fn_15");
            return false;
        }

        public bool _unknown_fn_16(IntPtr _)
        {
            LogStub("_unknown_fn_16");
            return false;
        }

        public bool _unknown_fn_17(IntPtr _)
        {
            LogStub("_unknown_fn_17");
            return false;
        }

        public bool _unknown_fn_18(IntPtr _)
        {
            LogStub("_unknown_fn_18");
            return false;
        }

        public bool _unknown_fn_19(IntPtr _)
        {
            LogStub("_unknown_fn_19");
            return false;
        }

        public int _unknown_fn_20(IntPtr _)
        {
            LogStub("_unknown_fn_20");
            return 0;
        }

        public int _unknown_fn_21(IntPtr _)
        {
            LogStub("_unknown_fn_21");
            return 0;
        }

        public int _unknown_fn_22(IntPtr _)
        {
            LogStub("_unknown_fn_22");
            return 0;
        }

        public int _unknown_fn_23(IntPtr _)
        {
            LogStub("_unknown_fn_23");
            return 0;
        }

        public int _unknown_fn_24(IntPtr _)
        {
            LogStub("_unknown_fn_24");
            return 0;
        }

        public int _unknown_fn_25(IntPtr _)
        {
            LogStub("_unknown_fn_25");
            return 0;
        }

        public int _unknown_fn_26(IntPtr _)
        {
            LogStub("_unknown_fn_26");
            return 0;
        }

        public IntPtr _unknown_fn_27(IntPtr _)
        {
            LogStub("_unknown_fn_27");
            return NativeStringCache.ToUtf8Ptr(string.Empty);
        }

        public int _unknown_fn_28(IntPtr _)
        {
            LogStub("_unknown_fn_28");
            return 0;
        }

        public int _unknown_fn_29(IntPtr _)
        {
            LogStub("_unknown_fn_29");
            return 0;
        }

        public int _unknown_fn_30(IntPtr _)
        {
            LogStub("_unknown_fn_30");
            return 0;
        }

        public int _unknown_fn_31(IntPtr _)
        {
            LogStub("_unknown_fn_31");
            return 0;
        }

        public int _unknown_fn_32(IntPtr _)
        {
            LogStub("_unknown_fn_32");
            return 0;
        }

        public int _unknown_fn_33(IntPtr _)
        {
            LogStub("_unknown_fn_33");
            return 0;
        }

        public int _unknown_fn_34(IntPtr _)
        {
            LogStub("_unknown_fn_34");
            return 0;
        }

        public int _unknown_fn_35(IntPtr _)
        {
            LogStub("_unknown_fn_35");
            return 0;
        }

        public int _unknown_fn_36(IntPtr _)
        {
            LogStub("_unknown_fn_36");
            return 0;
        }

        public int _unknown_fn_37(IntPtr _)
        {
            LogStub("_unknown_fn_37");
            return 0;
        }

        public IntPtr _unknown_fn_38(IntPtr _)
        {
            LogStub("_unknown_fn_38");
            return NativeStringCache.ToUtf8Ptr(string.Empty);
        }

        public int _unknown_fn_39(IntPtr _)
        {
            LogStub("_unknown_fn_39");
            return 0;
        }

        public int _unknown_fn_40(IntPtr _)
        {
            LogStub("_unknown_fn_40");
            return 0;
        }

        public bool _unknown_fn_41(IntPtr _)
        {
            LogStub("_unknown_fn_41");
            return false;
        }

        public bool _unknown_fn_42(IntPtr _)
        {
            LogStub("_unknown_fn_42");
            return false;
        }

        public bool _unknown_fn_43(IntPtr _)
        {
            LogStub("_unknown_fn_43");
            return false;
        }
    }
}
