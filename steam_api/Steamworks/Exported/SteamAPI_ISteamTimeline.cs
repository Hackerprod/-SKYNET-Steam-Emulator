using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;
using TimelineEventHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamTimeline
    {
        static SteamAPI_ISteamTimeline()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_SetTimelineTooltip(IntPtr _, string pchDescription, float flTimeDelta)
        {
            Write("SteamAPI_ISteamTimeline_SetTimelineTooltip");
            SteamEmulator.SteamTimeline.SetTimelineTooltip(pchDescription, flTimeDelta);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_ClearTimelineTooltip(IntPtr _, float flTimeDelta)
        {
            Write("SteamAPI_ISteamTimeline_ClearTimelineTooltip");
            SteamEmulator.SteamTimeline.ClearTimelineTooltip(flTimeDelta);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_SetTimelineGameMode(IntPtr _, int eMode)
        {
            Write("SteamAPI_ISteamTimeline_SetTimelineGameMode");
            SteamEmulator.SteamTimeline.SetTimelineGameMode(eMode);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static TimelineEventHandle_t SteamAPI_ISteamTimeline_AddInstantaneousTimelineEvent(IntPtr _, string pchTitle, string pchDescription, string pchIcon, uint unIconPriority, float flStartOffsetSeconds, int ePossibleClip)
        {
            Write("SteamAPI_ISteamTimeline_AddInstantaneousTimelineEvent");
            return SteamEmulator.SteamTimeline.AddInstantaneousTimelineEvent(pchTitle, pchDescription, pchIcon, unIconPriority, flStartOffsetSeconds, ePossibleClip);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static TimelineEventHandle_t SteamAPI_ISteamTimeline_AddRangeTimelineEvent(IntPtr _, string pchTitle, string pchDescription, string pchIcon, uint unIconPriority, float flStartOffsetSeconds, float flDuration, int ePossibleClip)
        {
            Write("SteamAPI_ISteamTimeline_AddRangeTimelineEvent");
            return SteamEmulator.SteamTimeline.AddRangeTimelineEvent(pchTitle, pchDescription, pchIcon, unIconPriority, flStartOffsetSeconds, flDuration, ePossibleClip);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static TimelineEventHandle_t SteamAPI_ISteamTimeline_StartRangeTimelineEvent(IntPtr _, string pchTitle, string pchDescription, string pchIcon, uint unPriority, float flStartOffsetSeconds, int ePossibleClip)
        {
            Write("SteamAPI_ISteamTimeline_StartRangeTimelineEvent");
            return SteamEmulator.SteamTimeline.StartRangeTimelineEvent(pchTitle, pchDescription, pchIcon, unPriority, flStartOffsetSeconds, ePossibleClip);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_UpdateRangeTimelineEvent(IntPtr _, TimelineEventHandle_t ulEvent, string pchTitle, string pchDescription, string pchIcon, uint unPriority, int ePossibleClip)
        {
            Write("SteamAPI_ISteamTimeline_UpdateRangeTimelineEvent");
            SteamEmulator.SteamTimeline.UpdateRangeTimelineEvent(ulEvent, pchTitle, pchDescription, pchIcon, unPriority, ePossibleClip);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_EndRangeTimelineEvent(IntPtr _, TimelineEventHandle_t ulEvent, float flEndOffsetSeconds)
        {
            Write("SteamAPI_ISteamTimeline_EndRangeTimelineEvent");
            SteamEmulator.SteamTimeline.EndRangeTimelineEvent(ulEvent, flEndOffsetSeconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_RemoveTimelineEvent(IntPtr _, TimelineEventHandle_t ulEvent)
        {
            Write("SteamAPI_ISteamTimeline_RemoveTimelineEvent");
            SteamEmulator.SteamTimeline.RemoveTimelineEvent(ulEvent);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamTimeline_DoesEventRecordingExist(IntPtr _, TimelineEventHandle_t ulEvent)
        {
            Write("SteamAPI_ISteamTimeline_DoesEventRecordingExist");
            return SteamEmulator.SteamTimeline.DoesEventRecordingExist(ulEvent);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_StartGamePhase(IntPtr _)
        {
            Write("SteamAPI_ISteamTimeline_StartGamePhase");
            SteamEmulator.SteamTimeline.StartGamePhase();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_EndGamePhase(IntPtr _)
        {
            Write("SteamAPI_ISteamTimeline_EndGamePhase");
            SteamEmulator.SteamTimeline.EndGamePhase();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_SetGamePhaseID(IntPtr _, string pchPhaseID)
        {
            Write("SteamAPI_ISteamTimeline_SetGamePhaseID");
            SteamEmulator.SteamTimeline.SetGamePhaseID(pchPhaseID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamTimeline_DoesGamePhaseRecordingExist(IntPtr _, string pchPhaseID)
        {
            Write("SteamAPI_ISteamTimeline_DoesGamePhaseRecordingExist");
            return SteamEmulator.SteamTimeline.DoesGamePhaseRecordingExist(pchPhaseID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_AddGamePhaseTag(IntPtr _, string pchTagName, string pchTagIcon, string pchTagGroup, uint unPriority)
        {
            Write("SteamAPI_ISteamTimeline_AddGamePhaseTag");
            SteamEmulator.SteamTimeline.AddGamePhaseTag(pchTagName, pchTagIcon, pchTagGroup, unPriority);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_SetGamePhaseAttribute(IntPtr _, string pchAttributeGroup, string pchAttributeValue, uint unPriority)
        {
            Write("SteamAPI_ISteamTimeline_SetGamePhaseAttribute");
            SteamEmulator.SteamTimeline.SetGamePhaseAttribute(pchAttributeGroup, pchAttributeValue, unPriority);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_OpenOverlayToGamePhase(IntPtr _, string pchPhaseID)
        {
            Write("SteamAPI_ISteamTimeline_OpenOverlayToGamePhase");
            SteamEmulator.SteamTimeline.OpenOverlayToGamePhase(pchPhaseID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamTimeline_OpenOverlayToTimelineEvent(IntPtr _, TimelineEventHandle_t ulEvent)
        {
            Write("SteamAPI_ISteamTimeline_OpenOverlayToTimelineEvent");
            SteamEmulator.SteamTimeline.OpenOverlayToTimelineEvent(ulEvent);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
