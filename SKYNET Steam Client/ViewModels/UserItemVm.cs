using System.Windows.Media;
using SKYNET.Client.Models;
using SKYNET.Client.Services;

namespace SKYNET.Client.ViewModels;

/// <summary>View model for one user card in the Users view.</summary>
public sealed class UserItemVm
{
    private static readonly SolidColorBrush Online = new((Color)ColorConverter.ConvertFromString("#3B82F6"));
    private static readonly SolidColorBrush Offline = new((Color)ColorConverter.ConvertFromString("#888888"));
    private static readonly SolidColorBrush InGame = new((Color)ColorConverter.ConvertFromString("#4ADE80"));

    private readonly PresenceLevel _level;

    public UserItemVm(WebUser user, Func<uint, string?>? gameNameResolver = null)
    {
        DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? "Player" : user.DisplayName;
        _level = user.Level;
        IsOnline = _level != PresenceLevel.Offline;
        StatusText = BuildStatusText(user, gameNameResolver);
        Avatar = Images.FromBytes(user.AvatarPng);
        AvatarBrush = Avatar != null
            ? new ImageBrush(Avatar) { Stretch = Stretch.UniformToFill }
            : null;
    }

    public string DisplayName { get; }
    public bool IsOnline { get; }
    public ImageSource? Avatar { get; }
    public ImageBrush? AvatarBrush { get; }
    public bool HasAvatar => Avatar != null;
    public bool NoAvatar => Avatar == null;
    public string Initial => DisplayName.Substring(0, 1).ToUpperInvariant();
    public string StatusText { get; }
    public Brush StatusBrush => _level == PresenceLevel.InGame ? InGame : _level == PresenceLevel.Online ? Online : Offline;
    public Brush DotBrush => StatusBrush;

    private static string BuildStatusText(WebUser user, Func<uint, string?>? gameNameResolver)
    {
        switch (user.Level)
        {
            case PresenceLevel.Offline:
                return "OFFLINE";
            case PresenceLevel.Online:
                return "ONLINE";
        }

        var name = user.AppId != 0 ? gameNameResolver?.Invoke(user.AppId) : null;
        if (string.IsNullOrWhiteSpace(name))
            name = user.AppId != 0 ? $"App {user.AppId}" : "In game";
        var text = "PLAYING " + name;
        return string.IsNullOrWhiteSpace(user.PresenceDetail) ? text : text + " - " + user.PresenceDetail;
    }
}
