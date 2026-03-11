using System;

using SteamAPICall_t = System.UInt64;
using TimelineEventHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMTIMELINE_INTERFACE_V004")]
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
    }
}
