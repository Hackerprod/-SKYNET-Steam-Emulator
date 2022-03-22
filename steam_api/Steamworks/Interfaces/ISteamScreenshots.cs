using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamScreenshots
    {
        // Writes a screenshot to the user's screenshot library given the raw image data, which must be in RGB format.
        // The return value is a handle that is valid for the duration of the game process and can be used to apply tags.
        uint WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight);

        // Adds a screenshot to the user's screenshot library from disk.  If a thumbnail is provided, it must be 200 pixels wide and the same aspect ratio
        // as the screenshot, otherwise a thumbnail will be generated if the user uploads the screenshot.  The screenshots must be in either JPEG or TGA format.
        // The return value is a handle that is valid for the duration of the game process and can be used to apply tags.
        // JPEG, TGA, and PNG formats are supported.
        uint AddScreenshotToLibrary(char pchFilename, char pchThumbnailFilename, int nWidth, int nHeight);

        // Causes the Steam overlay to take a screenshot.  If screenshots are being hooked by the game then a ScreenshotRequested_t callback is sent back to the game instead. 
        void TriggerScreenshot(IntPtr _);

        // Toggles whether the overlay handles screenshots when the user presses the screenshot hotkey, or the game handles them.  If the game is hooking screenshots,
        // then the ScreenshotRequested_t callback will be sent if the user presses the hotkey, and the game is expected to call WriteScreenshot or AddScreenshotToLibrary
        // in response.
        void HookScreenshots(bool bHook);

        // Sets metadata about a screenshot's location (for example, the name of the map)
        bool SetLocation(uint hScreenshot, char pchLocation);

        // Tags a user as being visible in the screenshot
        bool TagUser(uint hScreenshot, IntPtr steamID);

        // Tags a published file as being visible in the screenshot
        bool TagPublishedFile(uint hScreenshot, uint unPublishedFileID);

        // Returns true if the app has hooked the screenshot
        bool IsScreenshotsHooked(IntPtr _);

        // Adds a VR screenshot to the user's screenshot library from disk in the supported type.
        // pchFilename should be the normal 2D image used in the library view
        // pchVRFilename should contain the image that matches the correct type
        // The return value is a handle that is valid for the duration of the game process and can be used to apply tags.
        // JPEG, TGA, and PNG formats are supported.
        uint AddVRScreenshotToLibrary(EVRScreenshotType eType, char pchFilename, char pchVRFilename);

    }
    public enum EVRScreenshotType : int
    {
        k_EVRScreenshotType_None = 0,
        k_EVRScreenshotType_Mono = 1,
        k_EVRScreenshotType_Stereo = 2,
        k_EVRScreenshotType_MonoCubemap = 3,
        k_EVRScreenshotType_MonoPanorama = 4,
        k_EVRScreenshotType_StereoPanorama = 5
    };
}
