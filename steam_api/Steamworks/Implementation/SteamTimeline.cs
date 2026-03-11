using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;
using TimelineEventHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamTimeline : ISteamInterface
    {
        private TimelineEventHandle_t nextEventHandle = 1;

        public SteamTimeline()
        {
            InterfaceName = "SteamTimeline";
            InterfaceVersion = "STEAMTIMELINE_INTERFACE_V004";
        }

        public void SetTimelineTooltip(string pchDescription, float flTimeDelta)
        {
            Write("SetTimelineTooltip");
        }

        public void ClearTimelineTooltip(float flTimeDelta)
        {
            Write("ClearTimelineTooltip");
        }

        public void SetTimelineGameMode(int eMode)
        {
            Write("SetTimelineGameMode");
        }

        public TimelineEventHandle_t AddInstantaneousTimelineEvent(string pchTitle, string pchDescription, string pchIcon, uint unIconPriority, float flStartOffsetSeconds, int ePossibleClip)
        {
            Write("AddInstantaneousTimelineEvent");
            return nextEventHandle++;
        }

        public TimelineEventHandle_t AddRangeTimelineEvent(string pchTitle, string pchDescription, string pchIcon, uint unIconPriority, float flStartOffsetSeconds, float flDuration, int ePossibleClip)
        {
            Write("AddRangeTimelineEvent");
            return nextEventHandle++;
        }

        public TimelineEventHandle_t StartRangeTimelineEvent(string pchTitle, string pchDescription, string pchIcon, uint unPriority, float flStartOffsetSeconds, int ePossibleClip)
        {
            Write("StartRangeTimelineEvent");
            return nextEventHandle++;
        }

        public void UpdateRangeTimelineEvent(TimelineEventHandle_t ulEvent, string pchTitle, string pchDescription, string pchIcon, uint unPriority, int ePossibleClip)
        {
            Write("UpdateRangeTimelineEvent");
        }

        public void EndRangeTimelineEvent(TimelineEventHandle_t ulEvent, float flEndOffsetSeconds)
        {
            Write("EndRangeTimelineEvent");
        }

        public void RemoveTimelineEvent(TimelineEventHandle_t ulEvent)
        {
            Write("RemoveTimelineEvent");
        }

        public SteamAPICall_t DoesEventRecordingExist(TimelineEventHandle_t ulEvent)
        {
            Write("DoesEventRecordingExist");
            return k_uAPICallInvalid;
        }

        public void StartGamePhase()
        {
            Write("StartGamePhase");
        }

        public void EndGamePhase()
        {
            Write("EndGamePhase");
        }

        public void SetGamePhaseID(string pchPhaseID)
        {
            Write("SetGamePhaseID");
        }

        public SteamAPICall_t DoesGamePhaseRecordingExist(string pchPhaseID)
        {
            Write("DoesGamePhaseRecordingExist");
            return k_uAPICallInvalid;
        }

        public void AddGamePhaseTag(string pchTagName, string pchTagIcon, string pchTagGroup, uint unPriority)
        {
            Write("AddGamePhaseTag");
        }

        public void SetGamePhaseAttribute(string pchAttributeGroup, string pchAttributeValue, uint unPriority)
        {
            Write("SetGamePhaseAttribute");
        }

        public void OpenOverlayToGamePhase(string pchPhaseID)
        {
            Write("OpenOverlayToGamePhase");
        }

        public void OpenOverlayToTimelineEvent(TimelineEventHandle_t ulEvent)
        {
            Write("OpenOverlayToTimelineEvent");
        }
    }
}
