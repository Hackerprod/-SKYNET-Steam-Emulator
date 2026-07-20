using System.Windows.Media;
using SKYNET.Client.Models;
using SKYNET.Client.Services;

namespace SKYNET.Client.ViewModels;

/// <summary>View model for one user card in the Users view.</summary>
public sealed class UserItemVm
{
    private static readonly SolidColorBrush Online = new((Color)ColorConverter.ConvertFromString("#3B82F6"));
    private static readonly SolidColorBrush Offline = new((Color)ColorConverter.ConvertFromString("#888888"));

    public UserItemVm(WebUser user)
    {
        DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? "Player" : user.DisplayName;
        IsOnline = user.Online;
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
    public string StatusText => IsOnline ? "ONLINE" : "OFFLINE";
    public Brush StatusBrush => IsOnline ? Online : Offline;
}
