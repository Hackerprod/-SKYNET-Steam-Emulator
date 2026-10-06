using System;
using System.Collections.Generic;

using SteamAPICall_t = System.UInt64;
using TimelineEventHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("STEAMTIMELINE_INTERFACE_V001",
        "SetTimelineStateDescription", "ClearTimelineStateDescription", "AddTimelineEvent", "SetTimelineGameMode")]
    [InterfaceLayout("STEAMTIMELINE_INTERFACE_V002",
        "SetTimelineTooltip", "ClearTimelineTooltip", "SetTimelineGameMode", "AddTimelineEvent__V002",
        "unknown_ret0_1", "unknown_ret0_2", "unknown_nop_3", "RemoveTimelineEvent",
        "unknown_nop_4", "unknown_nop_5", "unknown_nop_6", "DoesEventRecordingExist",
        "StartGamePhase", "unknown_nop_7", "EndGamePhase", "SetGamePhaseID",
        "DoesGamePhaseRecordingExist", "AddGamePhaseTag", "OpenOverlayToGamePhase", "OpenOverlayToTimelineEvent")]
    [InterfaceLayout("STEAMTIMELINE_INTERFACE_V003",
        "SetTimelineTooltip", "ClearTimelineTooltip", "SetTimelineGameMode", "AddTimelineEvent__V002",
        "unknown_ret0_1", "unknown_ret0_2", "unknown_nop_3", "RemoveTimelineEvent",
        "unknown_nop_4", "unknown_nop_5", "unknown_nop_6", "DoesEventRecordingExist",
        "StartGamePhase", "EndGamePhase", "SetGamePhaseID", "DoesGamePhaseRecordingExist",
        "AddGamePhaseTag", "SetGamePhaseAttribute", "OpenOverlayToGamePhase", "OpenOverlayToTimelineEvent")]
    [InterfaceLayout("STEAMTIMELINE_INTERFACE_V004",
        "SetTimelineTooltip", "ClearTimelineTooltip", "SetTimelineGameMode", "AddInstantaneousTimelineEvent",
        "AddRangeTimelineEvent", "StartRangeTimelineEvent", "UpdateRangeTimelineEvent", "EndRangeTimelineEvent",
        "RemoveTimelineEvent", "DoesEventRecordingExist", "StartGamePhase", "EndGamePhase",
        "SetGamePhaseID", "DoesGamePhaseRecordingExist", "AddGamePhaseTag", "SetGamePhaseAttribute",
        "OpenOverlayToGamePhase", "OpenOverlayToTimelineEvent")]
    public class SteamTimeline004 : ISteamInterface
    {
        public void SetTimelineTooltip(IntPtr _, string pchDescription, float flTimeDelta)
        {
            SteamEmulator.SteamTimeline.SetTimelineTooltip(pchDescription, flTimeDelta);
        }

        public void ClearTimelineTooltip(IntPtr _, float flTimeDelta)
        {
            SteamEmulator.SteamTimeline.ClearTimelineTooltip(flTimeDelta);
        }

        public void SetTimelineGameMode(IntPtr _, int eMode)
        {
            SteamEmulator.SteamTimeline.SetTimelineGameMode(eMode);
        }

        public TimelineEventHandle_t AddInstantaneousTimelineEvent(IntPtr _, string pchTitle, string pchDescription, string pchIcon, uint unIconPriority, float flStartOffsetSeconds, int ePossibleClip)
        {
            return SteamEmulator.SteamTimeline.AddInstantaneousTimelineEvent(pchTitle, pchDescription, pchIcon, unIconPriority, flStartOffsetSeconds, ePossibleClip);
        }

        public TimelineEventHandle_t AddRangeTimelineEvent(IntPtr _, string pchTitle, string pchDescription, string pchIcon, uint unIconPriority, float flStartOffsetSeconds, float flDuration, int ePossibleClip)
        {
            return SteamEmulator.SteamTimeline.AddRangeTimelineEvent(pchTitle, pchDescription, pchIcon, unIconPriority, flStartOffsetSeconds, flDuration, ePossibleClip);
        }

        public TimelineEventHandle_t StartRangeTimelineEvent(IntPtr _, string pchTitle, string pchDescription, string pchIcon, uint unPriority, float flStartOffsetSeconds, int ePossibleClip)
        {
            return SteamEmulator.SteamTimeline.StartRangeTimelineEvent(pchTitle, pchDescription, pchIcon, unPriority, flStartOffsetSeconds, ePossibleClip);
        }

        public void UpdateRangeTimelineEvent(IntPtr _, TimelineEventHandle_t ulEvent, string pchTitle, string pchDescription, string pchIcon, uint unPriority, int ePossibleClip)
        {
            SteamEmulator.SteamTimeline.UpdateRangeTimelineEvent(ulEvent, pchTitle, pchDescription, pchIcon, unPriority, ePossibleClip);
        }

        public void EndRangeTimelineEvent(IntPtr _, TimelineEventHandle_t ulEvent, float flEndOffsetSeconds)
        {
            SteamEmulator.SteamTimeline.EndRangeTimelineEvent(ulEvent, flEndOffsetSeconds);
        }

        public void RemoveTimelineEvent(IntPtr _, TimelineEventHandle_t ulEvent)
        {
            SteamEmulator.SteamTimeline.RemoveTimelineEvent(ulEvent);
        }

        public SteamAPICall_t DoesEventRecordingExist(IntPtr _, TimelineEventHandle_t ulEvent)
        {
            return SteamEmulator.SteamTimeline.DoesEventRecordingExist(ulEvent);
        }

        public void StartGamePhase(IntPtr _)
        {
            SteamEmulator.SteamTimeline.StartGamePhase();
        }

        public void EndGamePhase(IntPtr _)
        {
            SteamEmulator.SteamTimeline.EndGamePhase();
        }

        public void SetGamePhaseID(IntPtr _, string pchPhaseID)
        {
            SteamEmulator.SteamTimeline.SetGamePhaseID(pchPhaseID);
        }

        public SteamAPICall_t DoesGamePhaseRecordingExist(IntPtr _, string pchPhaseID)
        {
            return SteamEmulator.SteamTimeline.DoesGamePhaseRecordingExist(pchPhaseID);
        }

        public void AddGamePhaseTag(IntPtr _, string pchTagName, string pchTagIcon, string pchTagGroup, uint unPriority)
        {
            SteamEmulator.SteamTimeline.AddGamePhaseTag(pchTagName, pchTagIcon, pchTagGroup, unPriority);
        }

        public void SetGamePhaseAttribute(IntPtr _, string pchAttributeGroup, string pchAttributeValue, uint unPriority)
        {
            SteamEmulator.SteamTimeline.SetGamePhaseAttribute(pchAttributeGroup, pchAttributeValue, unPriority);
        }

        public void OpenOverlayToGamePhase(IntPtr _, string pchPhaseID)
        {
            SteamEmulator.SteamTimeline.OpenOverlayToGamePhase(pchPhaseID);
        }

        public void OpenOverlayToTimelineEvent(IntPtr _, TimelineEventHandle_t ulEvent)
        {
            SteamEmulator.SteamTimeline.OpenOverlayToTimelineEvent(ulEvent);
        }

        public void SetTimelineStateDescription(IntPtr _, string pchDescription, float flTimeDelta)
        {
            SteamEmulator.SteamTimeline.SetTimelineTooltip(pchDescription, flTimeDelta);
        }

        public void ClearTimelineStateDescription(IntPtr _, float flTimeDelta)
        {
            SteamEmulator.SteamTimeline.ClearTimelineTooltip(flTimeDelta);
        }

        public void AddTimelineEvent(IntPtr _, string pchIcon, string pchTitle, string pchDescription, uint unPriority, float flStartOffsetSeconds, float flDurationSeconds, int ePossibleClip)
        {
            if (flDurationSeconds > 0f)
            {
                SteamEmulator.SteamTimeline.AddRangeTimelineEvent(pchTitle, pchDescription, pchIcon, unPriority, flStartOffsetSeconds, flDurationSeconds, ePossibleClip);
            }
            else
            {
                SteamEmulator.SteamTimeline.AddInstantaneousTimelineEvent(pchTitle, pchDescription, pchIcon, unPriority, flStartOffsetSeconds, ePossibleClip);
            }
        }

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

            SteamEmulator.Write("SteamTimeline", method + " not implemented");
        }

        public TimelineEventHandle_t AddTimelineEvent__V002(IntPtr _, string pchTitle, string pchDescription, string pchIcon, uint unIconPriority, float flStartOffsetSeconds, float flDurationSeconds, int ePossibleClip)
        {
            if (flDurationSeconds > 0f)
            {
                return SteamEmulator.SteamTimeline.AddRangeTimelineEvent(pchTitle, pchDescription, pchIcon, unIconPriority, flStartOffsetSeconds, flDurationSeconds, ePossibleClip);
            }

            return SteamEmulator.SteamTimeline.AddInstantaneousTimelineEvent(pchTitle, pchDescription, pchIcon, unIconPriority, flStartOffsetSeconds, ePossibleClip);
        }

        public uint unknown_ret0_1(IntPtr _)
        {
            LogStub("unknown_ret0_1");
            return 0;
        }

        public uint unknown_ret0_2(IntPtr _)
        {
            LogStub("unknown_ret0_2");
            return 0;
        }

        public void unknown_nop_3(IntPtr _) => LogStub("unknown_nop_3");
        public void unknown_nop_4(IntPtr _) => LogStub("unknown_nop_4");
        public void unknown_nop_5(IntPtr _) => LogStub("unknown_nop_5");
        public void unknown_nop_6(IntPtr _) => LogStub("unknown_nop_6");
        public void unknown_nop_7(IntPtr _) => LogStub("unknown_nop_7");
    }
}
