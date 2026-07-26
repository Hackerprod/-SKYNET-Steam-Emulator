using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using SKYNET.Steamworks.Implementation;

namespace SKYNET.Managers
{
    /// <summary>
    /// Holds presentation metadata delivered with the authenticated app session.
    /// Achievement ownership/progress remains in StateCache; this catalog only
    /// supplies the immutable schema that Steam normally distributes per AppID.
    /// </summary>
    public static class AchievementDefinitionManager
    {
        private static readonly object Gate = new object();
        private static readonly Dictionary<string, APIClient.SkyNetAchievementDefinitionDto> Definitions =
            new Dictionary<string, APIClient.SkyNetAchievementDefinitionDto>(StringComparer.Ordinal);
        private static readonly List<string> OrderedNames = new List<string>();
        private static readonly Dictionary<string, int> IconHandles =
            new Dictionary<string, int>(StringComparer.Ordinal);
        private static uint CurrentAppId;

        public static void Apply(
            uint appId,
            IEnumerable<APIClient.SkyNetAchievementDefinitionDto> definitions)
        {
            lock (Gate)
            {
                CurrentAppId = appId;
                Definitions.Clear();
                OrderedNames.Clear();
                IconHandles.Clear();

                foreach (var definition in definitions ?? Enumerable.Empty<APIClient.SkyNetAchievementDefinitionDto>())
                {
                    var apiName = definition?.ApiName?.Trim();
                    if (string.IsNullOrEmpty(apiName))
                    {
                        continue;
                    }

                    if (!Definitions.ContainsKey(apiName))
                    {
                        OrderedNames.Add(apiName);
                    }

                    Definitions[apiName] = definition;
                }
            }
        }

        public static int Count
        {
            get
            {
                lock (Gate)
                {
                    return OrderedNames.Count;
                }
            }
        }

        public static string GetName(uint index)
        {
            lock (Gate)
            {
                return index < OrderedNames.Count
                    ? OrderedNames[(int)index]
                    : string.Empty;
            }
        }

        public static string GetDisplayAttribute(string apiName, string key)
        {
            if (string.IsNullOrEmpty(apiName) || string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            lock (Gate)
            {
                if (!Definitions.TryGetValue(apiName, out var definition))
                {
                    return string.Equals(key, "name", StringComparison.OrdinalIgnoreCase)
                        ? apiName
                        : string.Equals(key, "hidden", StringComparison.OrdinalIgnoreCase)
                            ? "0"
                            : string.Empty;
                }

                if (string.Equals(key, "name", StringComparison.OrdinalIgnoreCase))
                {
                    return string.IsNullOrWhiteSpace(definition.DisplayName)
                        ? apiName
                        : definition.DisplayName;
                }

                if (string.Equals(key, "desc", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(key, "description", StringComparison.OrdinalIgnoreCase))
                {
                    return definition.Description ?? string.Empty;
                }

                if (string.Equals(key, "hidden", StringComparison.OrdinalIgnoreCase))
                {
                    return definition.Hidden ? "1" : "0";
                }

                return string.Empty;
            }
        }

        public static bool HasDefinition(string apiName)
        {
            if (string.IsNullOrEmpty(apiName))
            {
                return false;
            }

            lock (Gate)
            {
                return Definitions.ContainsKey(apiName);
            }
        }

        public static bool IsKnownOrUnconfigured(string apiName)
        {
            if (string.IsNullOrEmpty(apiName))
            {
                return false;
            }

            lock (Gate)
            {
                return Definitions.Count == 0 || Definitions.ContainsKey(apiName);
            }
        }

        public static int GetIcon(string apiName, bool achieved)
        {
            if (string.IsNullOrEmpty(apiName) || SteamFriends.Instance == null)
            {
                return 0;
            }

            lock (Gate)
            {
                if (!Definitions.TryGetValue(apiName, out var definition))
                {
                    return 0;
                }

                var cacheKey = $"{CurrentAppId}:{apiName}:{achieved}";
                if (IconHandles.TryGetValue(cacheKey, out var cached))
                {
                    return cached;
                }

                using (var image = DecodeImage(
                           achieved ? definition.IconBase64 : definition.LockedIconBase64)
                       ?? CreateFallbackIcon(achieved))
                {
                    var handle = SteamFriends.Instance.RegisterImage(image);
                    if (handle != 0)
                    {
                        IconHandles[cacheKey] = handle;
                    }
                    return handle;
                }
            }
        }

        private static Bitmap DecodeImage(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                return null;
            }

            try
            {
                var bytes = Convert.FromBase64String(base64);
                using (var stream = new MemoryStream(bytes, writable: false))
                using (var image = Image.FromStream(stream, useEmbeddedColorManagement: false, validateImageData: true))
                {
                    return new Bitmap(image);
                }
            }
            catch
            {
                return null;
            }
        }

        private static Bitmap CreateFallbackIcon(bool achieved)
        {
            var bitmap = new Bitmap(64, 64, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
            using (var border = new Pen(achieved ? Color.FromArgb(75, 225, 145) : Color.FromArgb(155, 165, 180), 4))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(Color.FromArgb(24, 27, 32));
                graphics.DrawRectangle(border, 5, 5, 53, 53);

                if (achieved)
                {
                    using (var check = new Pen(Color.FromArgb(75, 225, 145), 7)
                    {
                        StartCap = LineCap.Round,
                        EndCap = LineCap.Round
                    })
                    {
                        graphics.DrawLines(check, new[]
                        {
                            new Point(17, 33),
                            new Point(28, 44),
                            new Point(48, 20)
                        });
                    }
                }
                else
                {
                    using (var lockPen = new Pen(Color.FromArgb(155, 165, 180), 5))
                    using (var lockFill = new SolidBrush(Color.FromArgb(68, 74, 85)))
                    {
                        graphics.DrawArc(lockPen, 20, 13, 24, 28, 180, -180);
                        graphics.FillRectangle(lockFill, 16, 29, 32, 22);
                    }
                }
            }

            return bitmap;
        }
    }
}
