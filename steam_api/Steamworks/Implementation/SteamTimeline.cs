using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using SKYNET.Callback;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using Overlay.Core;

using SteamAPICall_t = System.UInt64;
using TimelineEventHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public sealed class SteamTimeline : ISteamInterface
    {
        private const int MaxPhaseIdBytes = 64;
        private readonly object gate = new object();
        private readonly Dictionary<TimelineEventHandle_t, TimelineEventRecord> events =
            new Dictionary<TimelineEventHandle_t, TimelineEventRecord>();
        private readonly List<GamePhaseRecord> phases = new List<GamePhaseRecord>();
        private readonly string sessionId = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff");
        private TimelineEventHandle_t nextEventHandle = 1;
        private GamePhaseRecord activePhase;
        private string tooltip = string.Empty;
        private float tooltipOffset;
        private int gameMode;

        public SteamTimeline()
        {
            InterfaceName = "SteamTimeline";
            InterfaceVersion = "STEAMTIMELINE_INTERFACE_V004";
        }

        public void SetTimelineTooltip(string pchDescription, float flTimeDelta)
        {
            lock (gate)
            {
                tooltip = pchDescription ?? string.Empty;
                tooltipOffset = flTimeDelta;
                Persist();
            }
        }

        public void ClearTimelineTooltip(float flTimeDelta)
        {
            lock (gate)
            {
                tooltip = string.Empty;
                tooltipOffset = flTimeDelta;
                Persist();
            }
        }

        public void SetTimelineGameMode(int eMode)
        {
            lock (gate)
            {
                gameMode = eMode;
                Persist();
            }
        }

        public TimelineEventHandle_t AddInstantaneousTimelineEvent(
            string pchTitle,
            string pchDescription,
            string pchIcon,
            uint unIconPriority,
            float flStartOffsetSeconds,
            int ePossibleClip)
        {
            return AddEvent(pchTitle, pchDescription, pchIcon, unIconPriority, flStartOffsetSeconds, 0, ePossibleClip, false);
        }

        public TimelineEventHandle_t AddRangeTimelineEvent(
            string pchTitle,
            string pchDescription,
            string pchIcon,
            uint unIconPriority,
            float flStartOffsetSeconds,
            float flDuration,
            int ePossibleClip)
        {
            return AddEvent(pchTitle, pchDescription, pchIcon, unIconPriority, flStartOffsetSeconds, flDuration, ePossibleClip, false);
        }

        public TimelineEventHandle_t StartRangeTimelineEvent(
            string pchTitle,
            string pchDescription,
            string pchIcon,
            uint unPriority,
            float flStartOffsetSeconds,
            int ePossibleClip)
        {
            return AddEvent(pchTitle, pchDescription, pchIcon, unPriority, flStartOffsetSeconds, null, ePossibleClip, true);
        }

        public void UpdateRangeTimelineEvent(
            TimelineEventHandle_t ulEvent,
            string pchTitle,
            string pchDescription,
            string pchIcon,
            uint unPriority,
            int ePossibleClip)
        {
            lock (gate)
            {
                if (!events.TryGetValue(ulEvent, out var record))
                {
                    return;
                }
                if (pchTitle != null) record.Title = pchTitle;
                if (pchDescription != null) record.Description = pchDescription;
                if (pchIcon != null) record.Icon = pchIcon;
                record.Priority = unPriority;
                record.ClipPriority = ePossibleClip;
                record.UpdatedAtUnixMs = NowUnixMs();
                Persist();
            }
        }

        public void EndRangeTimelineEvent(TimelineEventHandle_t ulEvent, float flEndOffsetSeconds)
        {
            lock (gate)
            {
                if (!events.TryGetValue(ulEvent, out var record) || record.EndedAtUnixMs.HasValue)
                {
                    return;
                }
                record.EndOffsetSeconds = flEndOffsetSeconds;
                record.EndedAtUnixMs = NowUnixMs();
                record.DurationSeconds = Math.Max(0, (record.EndedAtUnixMs.Value - record.CreatedAtUnixMs) / 1000d);
                Persist();
            }
        }

        public void RemoveTimelineEvent(TimelineEventHandle_t ulEvent)
        {
            lock (gate)
            {
                if (events.Remove(ulEvent))
                {
                    Persist();
                }
            }
        }

        public SteamAPICall_t DoesEventRecordingExist(TimelineEventHandle_t ulEvent)
        {
            // Timeline metadata and Steam Game Recording are separate systems.
            // This emulator currently stores metadata but has no video recorder.
            return CallbackManager.AddCallbackResult(new SteamTimelineEventRecordingExists_t
            {
                EventId = ulEvent,
                RecordingExists = false
            });
        }

        public void StartGamePhase()
        {
            lock (gate)
            {
                if (activePhase != null)
                {
                    EndActivePhase();
                }
                activePhase = new GamePhaseRecord
                {
                    Sequence = phases.Count + 1,
                    StartedAtUnixMs = NowUnixMs()
                };
                phases.Add(activePhase);
                Persist();
            }
        }

        public void EndGamePhase()
        {
            lock (gate)
            {
                EndActivePhase();
                Persist();
            }
        }

        public void SetGamePhaseID(string pchPhaseID)
        {
            lock (gate)
            {
                EnsureActivePhase();
                activePhase.PhaseId = TruncateUtf8(pchPhaseID ?? string.Empty, MaxPhaseIdBytes - 1);
                Persist();
            }
        }

        public SteamAPICall_t DoesGamePhaseRecordingExist(string pchPhaseID)
        {
            return CallbackManager.AddCallbackResult(new SteamTimelineGamePhaseRecordingExists_t
            {
                PhaseId = Utf8Buffer(pchPhaseID, MaxPhaseIdBytes),
                RecordingMilliseconds = 0,
                LongestClipMilliseconds = 0,
                ClipCount = 0,
                ScreenshotCount = 0
            });
        }

        public void AddGamePhaseTag(string pchTagName, string pchTagIcon, string pchTagGroup, uint unPriority)
        {
            lock (gate)
            {
                EnsureActivePhase();
                activePhase.Tags.Add(new TimelineTag
                {
                    Name = pchTagName ?? string.Empty,
                    Icon = pchTagIcon ?? string.Empty,
                    Group = pchTagGroup ?? string.Empty,
                    Priority = unPriority
                });
                Persist();
            }
        }

        public void SetGamePhaseAttribute(string pchAttributeGroup, string pchAttributeValue, uint unPriority)
        {
            lock (gate)
            {
                EnsureActivePhase();
                activePhase.Attributes[pchAttributeGroup ?? string.Empty] = new TimelineAttribute
                {
                    Value = pchAttributeValue ?? string.Empty,
                    Priority = unPriority
                };
                Persist();
            }
        }

        public void OpenOverlayToGamePhase(string pchPhaseID)
        {
            GamePhaseRecord phase;
            lock (gate)
            {
                phase = phases.LastOrDefault(current =>
                    string.Equals(current.PhaseId, pchPhaseID ?? string.Empty, StringComparison.Ordinal))?.Clone();
            }

            if (phase == null)
            {
                OverlayManager.ShowTimeline(
                    "Game Phase",
                    "The requested phase is not available in this session.",
                    Array.Empty<OverlaySummaryItem>(),
                    Array.Empty<OverlayActivityItem>());
                return;
            }

            var end = phase.EndedAtUnixMs ?? NowUnixMs();
            var duration = TimeSpan.FromMilliseconds(Math.Max(0, end - phase.StartedAtUnixMs));
            var details = phase.Attributes
                .OrderByDescending(pair => pair.Value.Priority)
                .Select(pair => new OverlayActivityItem
                {
                    Title = pair.Key,
                    Detail = pair.Value.Value,
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(phase.StartedAtUnixMs).LocalDateTime
                })
                .Concat(phase.Tags
                    .OrderByDescending(tag => tag.Priority)
                    .Select(tag => new OverlayActivityItem
                    {
                        Title = string.IsNullOrWhiteSpace(tag.Group) ? "Tag" : tag.Group,
                        Detail = tag.Name,
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(phase.StartedAtUnixMs).LocalDateTime
                    }))
                .ToList();
            OverlayManager.ShowTimeline(
                string.IsNullOrWhiteSpace(phase.PhaseId) ? "Game Phase" : phase.PhaseId,
                "Timeline metadata for the selected game phase.",
                new[]
                {
                    Summary("DURATION", FormatDuration(duration)),
                    Summary("ATTRIBUTES", phase.Attributes.Count.ToString()),
                    Summary("TAGS", phase.Tags.Count.ToString())
                },
                details);
        }

        public void OpenOverlayToTimelineEvent(TimelineEventHandle_t ulEvent)
        {
            TimelineEventRecord record;
            lock (gate)
            {
                record = events.TryGetValue(ulEvent, out var current) ? current.Clone() : null;
            }

            if (record == null)
            {
                OverlayManager.ShowTimeline(
                    "Timeline Event",
                    "The requested event is not available in this session.",
                    Array.Empty<OverlaySummaryItem>(),
                    Array.Empty<OverlayActivityItem>());
                return;
            }

            var details = new List<OverlayActivityItem>
            {
                new OverlayActivityItem
                {
                    Title = string.IsNullOrWhiteSpace(record.Title) ? "Timeline event" : record.Title,
                    Detail = record.Description ?? string.Empty,
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(record.CreatedAtUnixMs).LocalDateTime
                }
            };
            OverlayManager.ShowTimeline(
                string.IsNullOrWhiteSpace(record.Title) ? "Timeline Event" : record.Title,
                record.Description ?? string.Empty,
                new[]
                {
                    Summary("EVENT ID", record.Handle.ToString()),
                    Summary("DURATION", FormatDuration(TimeSpan.FromSeconds(record.DurationSeconds ?? 0))),
                    Summary("PRIORITY", record.Priority.ToString())
                },
                details);
        }

        private TimelineEventHandle_t AddEvent(
            string title,
            string description,
            string icon,
            uint priority,
            float startOffset,
            double? duration,
            int clipPriority,
            bool open)
        {
            lock (gate)
            {
                var handle = nextEventHandle++;
                var now = NowUnixMs();
                events[handle] = new TimelineEventRecord
                {
                    Handle = handle,
                    Title = title ?? string.Empty,
                    Description = description ?? string.Empty,
                    Icon = icon ?? string.Empty,
                    Priority = priority,
                    StartOffsetSeconds = startOffset,
                    DurationSeconds = duration,
                    ClipPriority = clipPriority,
                    CreatedAtUnixMs = now,
                    UpdatedAtUnixMs = now,
                    EndedAtUnixMs = open ? (long?)null : now + (long)((duration ?? 0) * 1000)
                };
                Persist();
                return handle;
            }
        }

        private void EnsureActivePhase()
        {
            if (activePhase != null)
            {
                return;
            }
            activePhase = new GamePhaseRecord
            {
                Sequence = phases.Count + 1,
                StartedAtUnixMs = NowUnixMs()
            };
            phases.Add(activePhase);
        }

        private void EndActivePhase()
        {
            if (activePhase == null)
            {
                return;
            }
            activePhase.EndedAtUnixMs = NowUnixMs();
            activePhase = null;
        }

        private void Persist()
        {
            var snapshot = new TimelineSession
            {
                AppId = SteamEmulator.AppID,
                SessionId = sessionId,
                GameMode = gameMode,
                Tooltip = tooltip,
                TooltipOffsetSeconds = tooltipOffset,
                Events = events.Values.OrderBy(record => record.Handle).Select(record => record.Clone()).ToList(),
                Phases = phases.Select(phase => phase.Clone()).ToList()
            };
            WorkQueue.Enqueue("Persist timeline", () =>
            {
                try
                {
                    var root = Path.Combine(Common.GetPath(), "SKYNET", "Timeline", SteamEmulator.AppID.ToString());
                    Directory.CreateDirectory(root);
                    var path = Path.Combine(root, sessionId + ".json");
                    var temporary = path + ".tmp";
                    File.WriteAllText(temporary, new JavaScriptSerializer().Serialize(snapshot));
                    if (File.Exists(path))
                    {
                        try
                        {
                            File.Replace(temporary, path, null);
                        }
                        catch (PlatformNotSupportedException)
                        {
                            ReplaceByCopy(temporary, path);
                        }
                        catch (IOException)
                        {
                            ReplaceByCopy(temporary, path);
                        }
                    }
                    else
                    {
                        File.Move(temporary, path);
                    }
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("Timeline persistence", ex);
                }
            }, $"timeline:{SteamEmulator.AppID}:{sessionId}");
        }

        private static long NowUnixMs()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private static void ReplaceByCopy(string temporary, string path)
        {
            File.Copy(temporary, path, true);
            File.Delete(temporary);
        }

        private static OverlaySummaryItem Summary(string label, string value)
        {
            return new OverlaySummaryItem
            {
                Label = label ?? string.Empty,
                Value = value ?? string.Empty,
                Tone = "accent"
            };
        }

        private static string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalHours >= 1)
            {
                return string.Format("{0}:{1:00}:{2:00}", (int)duration.TotalHours, duration.Minutes, duration.Seconds);
            }
            return string.Format("{0}:{1:00}", (int)duration.TotalMinutes, duration.Seconds);
        }

        private static byte[] Utf8Buffer(string value, int size)
        {
            var result = new byte[size];
            var bytes = Encoding.UTF8.GetBytes(TruncateUtf8(value ?? string.Empty, size - 1));
            Buffer.BlockCopy(bytes, 0, result, 0, Math.Min(bytes.Length, size - 1));
            return result;
        }

        private static string TruncateUtf8(string value, int maximumBytes)
        {
            if (Encoding.UTF8.GetByteCount(value) <= maximumBytes)
            {
                return value;
            }
            while (value.Length > 0 && Encoding.UTF8.GetByteCount(value) > maximumBytes)
            {
                value = value.Substring(0, value.Length - 1);
            }
            return value;
        }

        private sealed class TimelineSession
        {
            public uint AppId { get; set; }
            public string SessionId { get; set; }
            public int GameMode { get; set; }
            public string Tooltip { get; set; }
            public float TooltipOffsetSeconds { get; set; }
            public List<TimelineEventRecord> Events { get; set; }
            public List<GamePhaseRecord> Phases { get; set; }
        }

        private sealed class TimelineEventRecord
        {
            public ulong Handle { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Icon { get; set; }
            public uint Priority { get; set; }
            public float StartOffsetSeconds { get; set; }
            public float EndOffsetSeconds { get; set; }
            public double? DurationSeconds { get; set; }
            public int ClipPriority { get; set; }
            public long CreatedAtUnixMs { get; set; }
            public long UpdatedAtUnixMs { get; set; }
            public long? EndedAtUnixMs { get; set; }

            public TimelineEventRecord Clone()
            {
                return (TimelineEventRecord)MemberwiseClone();
            }
        }

        private sealed class GamePhaseRecord
        {
            public int Sequence { get; set; }
            public string PhaseId { get; set; } = string.Empty;
            public long StartedAtUnixMs { get; set; }
            public long? EndedAtUnixMs { get; set; }
            public List<TimelineTag> Tags { get; set; } = new List<TimelineTag>();
            public Dictionary<string, TimelineAttribute> Attributes { get; set; } =
                new Dictionary<string, TimelineAttribute>(StringComparer.Ordinal);

            public GamePhaseRecord Clone()
            {
                return new GamePhaseRecord
                {
                    Sequence = Sequence,
                    PhaseId = PhaseId,
                    StartedAtUnixMs = StartedAtUnixMs,
                    EndedAtUnixMs = EndedAtUnixMs,
                    Tags = Tags.Select(tag => tag.Clone()).ToList(),
                    Attributes = Attributes.ToDictionary(pair => pair.Key, pair => pair.Value.Clone(), StringComparer.Ordinal)
                };
            }
        }

        private sealed class TimelineTag
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public string Group { get; set; }
            public uint Priority { get; set; }

            public TimelineTag Clone()
            {
                return (TimelineTag)MemberwiseClone();
            }
        }

        private sealed class TimelineAttribute
        {
            public string Value { get; set; }
            public uint Priority { get; set; }

            public TimelineAttribute Clone()
            {
                return (TimelineAttribute)MemberwiseClone();
            }
        }
    }
}
