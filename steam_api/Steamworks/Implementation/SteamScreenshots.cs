using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SKYNET.Callback;
using SKYNET.Helpers.JSON;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;

namespace SKYNET.Steamworks.Implementation
{
    public sealed class SteamScreenshots : ISteamInterface
    {
        private readonly object gate = new object();
        private readonly Dictionary<uint, ScreenshotRecord> screenshots = new Dictionary<uint, ScreenshotRecord>();
        private uint nextHandle = 1;
        private bool hooked;

        public static SteamScreenshots Instance;

        public SteamScreenshots()
        {
            Instance = this;
            InterfaceName = "SteamScreenshots";
            InterfaceVersion = "STEAMSCREENSHOTS_INTERFACE_VERSION003";
            LoadIndex();
        }

        public uint WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
        {
            if (pubRGB == IntPtr.Zero || nWidth <= 0 || nHeight <= 0)
            {
                return 0;
            }

            var required = checked((long)nWidth * nHeight * 3);
            if (required > cubRGB || required > int.MaxValue)
            {
                return 0;
            }

            var rgb = new byte[(int)required];
            Marshal.Copy(pubRGB, rgb, 0, rgb.Length);
            var handle = AllocateHandle();
            WorkQueue.Enqueue("Write screenshot", () =>
            {
                try
                {
                    var path = ScreenshotPath(handle, ".png");
                    SaveRgb(path, rgb, nWidth, nHeight);
                    AddRecord(handle, path, null, nWidth, nHeight, 0);
                    QueueReady(handle, EResult.k_EResultOK);
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("Screenshot write", ex);
                    QueueReady(handle, EResult.k_EResultFail);
                }
            }, highPriority: true);
            return handle;
        }

        public uint AddScreenshotToLibrary(string pchFilename, string pchThumbnailFilename, int nWidth, int nHeight)
        {
            if (string.IsNullOrWhiteSpace(pchFilename) || !File.Exists(pchFilename))
            {
                return 0;
            }

            var handle = AllocateHandle();
            WorkQueue.Enqueue("Import screenshot", () =>
            {
                try
                {
                    var extension = SafeExtension(pchFilename);
                    var destination = ScreenshotPath(handle, extension);
                    File.Copy(Path.GetFullPath(pchFilename), destination, true);

                    string thumbnail = null;
                    if (!string.IsNullOrWhiteSpace(pchThumbnailFilename) && File.Exists(pchThumbnailFilename))
                    {
                        thumbnail = ScreenshotPath(handle, ".thumb" + SafeExtension(pchThumbnailFilename));
                        File.Copy(Path.GetFullPath(pchThumbnailFilename), thumbnail, true);
                    }

                    AddRecord(handle, destination, thumbnail, nWidth, nHeight, 0);
                    QueueReady(handle, EResult.k_EResultOK);
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("Screenshot import", ex);
                    QueueReady(handle, EResult.k_EResultFail);
                }
            }, highPriority: true);
            return handle;
        }

        public uint AddVRScreenshotToLibrary(int eType, string pchFilename, string pchVRFilename)
        {
            if (string.IsNullOrWhiteSpace(pchFilename) || !File.Exists(pchFilename) ||
                string.IsNullOrWhiteSpace(pchVRFilename) || !File.Exists(pchVRFilename))
            {
                return 0;
            }

            var handle = AllocateHandle();
            WorkQueue.Enqueue("Import VR screenshot", () =>
            {
                try
                {
                    var destination = ScreenshotPath(handle, SafeExtension(pchFilename));
                    var vrDestination = ScreenshotPath(handle, ".vr" + SafeExtension(pchVRFilename));
                    File.Copy(Path.GetFullPath(pchFilename), destination, true);
                    File.Copy(Path.GetFullPath(pchVRFilename), vrDestination, true);

                    var dimensions = ReadImageDimensions(destination);
                    AddRecord(
                        handle,
                        destination,
                        null,
                        dimensions.Width,
                        dimensions.Height,
                        eType,
                        vrDestination);
                    QueueReady(handle, EResult.k_EResultOK);
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("VR screenshot import", ex);
                    QueueReady(handle, EResult.k_EResultFail);
                }
            }, highPriority: true);
            return handle;
        }

        public void TriggerScreenshot()
        {
            CallbackManager.AddCallback(new ScreenshotRequested_t());
        }

        public void HookScreenshots(bool bHook)
        {
            hooked = bHook;
        }

        public bool IsScreenshotsHooked()
        {
            return hooked;
        }

        public bool SetLocation(uint hScreenshot, string pchLocation)
        {
            return UpdateRecord(hScreenshot, record => record.Location = pchLocation ?? string.Empty);
        }

        public bool TagUser(uint hScreenshot, ulong steamID)
        {
            if (steamID == 0)
            {
                return false;
            }
            return UpdateRecord(hScreenshot, record =>
            {
                if (!record.TaggedUsers.Contains(steamID))
                {
                    record.TaggedUsers.Add(steamID);
                }
            });
        }

        public bool TagPublishedFile(uint hScreenshot, ulong unPublishedFileID)
        {
            if (unPublishedFileID == 0)
            {
                return false;
            }
            return UpdateRecord(hScreenshot, record =>
            {
                if (!record.PublishedFiles.Contains(unPublishedFileID))
                {
                    record.PublishedFiles.Add(unPublishedFileID);
                }
            });
        }

        private uint AllocateHandle()
        {
            lock (gate)
            {
                while (nextHandle == 0 || screenshots.ContainsKey(nextHandle))
                {
                    nextHandle++;
                }
                return nextHandle++;
            }
        }

        private void AddRecord(
            uint handle,
            string path,
            string thumbnail,
            int width,
            int height,
            int vrType,
            string vrPath = null)
        {
            lock (gate)
            {
                screenshots[handle] = new ScreenshotRecord
                {
                    Handle = handle,
                    AppId = SteamEmulator.InternalAppId,
                    Path = path,
                    ThumbnailPath = thumbnail,
                    Width = width,
                    Height = height,
                    CreatedAtUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    VrType = vrType,
                    VrPath = vrPath
                };
                PersistIndex();
            }
        }

        private bool UpdateRecord(uint handle, Action<ScreenshotRecord> update)
        {
            lock (gate)
            {
                if (!screenshots.TryGetValue(handle, out var record))
                {
                    return false;
                }
                update(record);
                PersistIndex();
                return true;
            }
        }

        private void LoadIndex()
        {
            try
            {
                var path = IndexPath();
                if (!File.Exists(path))
                {
                    return;
                }

                var records = File.ReadAllText(path).FromJson<List<ScreenshotRecord>>();
                if (records == null)
                {
                    return;
                }
                foreach (var record in records.Where(record => record != null && record.Handle != 0))
                {
                    screenshots[record.Handle] = record;
                }
                if (screenshots.Count > 0)
                {
                    nextHandle = screenshots.Keys.Max() + 1;
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("Screenshot index load", ex);
            }
        }

        private void PersistIndex()
        {
            var snapshot = screenshots.Values.OrderBy(record => record.Handle).ToArray();
            WorkQueue.Enqueue("Persist screenshot index", () =>
            {
                try
                {
                    var path = IndexPath();
                    var temporary = path + ".tmp";
                    File.WriteAllText(temporary, snapshot.ToJson());
                    if (File.Exists(path))
                    {
                        File.Replace(temporary, path, null);
                    }
                    else
                    {
                        File.Move(temporary, path);
                    }
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("Screenshot index save", ex);
                }
            }, $"screenshots:{SteamEmulator.InternalAppId}");
        }

        private static void SaveRgb(string path, byte[] rgb, int width, int height)
        {
            var sourceStride = checked(width * 3);
            var targetStride = checked((sourceStride + 3) & ~3);
            var bgr = new byte[checked(targetStride * height)];
            for (var y = 0; y < height; y++)
            {
                var sourceRow = y * sourceStride;
                var targetRow = y * targetStride;
                for (var x = 0; x < width; x++)
                {
                    var sourcePixel = sourceRow + x * 3;
                    var targetPixel = targetRow + x * 3;
                    bgr[targetPixel] = rgb[sourcePixel + 2];
                    bgr[targetPixel + 1] = rgb[sourcePixel + 1];
                    bgr[targetPixel + 2] = rgb[sourcePixel];
                }
            }

            var buffer = GCHandle.Alloc(bgr, GCHandleType.Pinned);
            try
            {
                using (var bitmap = new Bitmap(
                    width,
                    height,
                    targetStride,
                    PixelFormat.Format24bppRgb,
                    buffer.AddrOfPinnedObject()))
                {
                    bitmap.Save(path, ImageFormat.Png);
                }
            }
            finally
            {
                buffer.Free();
            }
        }

        private static Size ReadImageDimensions(string path)
        {
            using (var image = Image.FromFile(path))
            {
                return image.Size;
            }
        }

        private static void QueueReady(uint handle, EResult result)
        {
            NativeCallbackQueue.Enqueue(() => CallbackManager.AddCallback(new ScreenshotReady_t
            {
                Local = handle,
                Result = result
            }));
        }

        private static string ScreenshotPath(uint handle, string extension)
        {
            var root = ScreenshotRoot();
            return Path.Combine(root, $"{handle:D10}{extension}");
        }

        private static string IndexPath()
        {
            return Path.Combine(ScreenshotRoot(), "screenshots.json");
        }

        private static string ScreenshotRoot()
        {
            var root = Path.Combine(Common.GetPath(), "SKYNET", "Screenshots", SteamEmulator.InternalAppId.ToString());
            Directory.CreateDirectory(root);
            return root;
        }

        private static string SafeExtension(string path)
        {
            var extension = Path.GetExtension(path);
            return string.IsNullOrWhiteSpace(extension) || extension.Length > 10 ? ".png" : extension;
        }

        private sealed class ScreenshotRecord
        {
            public uint Handle { get; set; }
            public uint AppId { get; set; }
            public string Path { get; set; }
            public string ThumbnailPath { get; set; }
            public string VrPath { get; set; }
            public int VrType { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public long CreatedAtUnix { get; set; }
            public string Location { get; set; } = string.Empty;
            public List<ulong> TaggedUsers { get; set; } = new List<ulong>();
            public List<ulong> PublishedFiles { get; set; } = new List<ulong>();
        }
    }
}
