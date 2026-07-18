using System.ComponentModel;
using System.Windows.Media;
using SKYNET.Client.Models;
using SKYNET.Client.Services;

namespace SKYNET.Client.ViewModels;

/// <summary>View model for one game card in the dashboard grid.</summary>
public sealed class GameCardVm : INotifyPropertyChanged
{
    public GameEntry Game { get; }

    public GameCardVm(GameEntry game)
    {
        Game = game;
        Icon = Images.FromBytes(game.IconPng);
    }

    public string Name => string.IsNullOrWhiteSpace(Game.Name) ? "Unnamed game" : Game.Name;

    public string Subtitle
    {
        get
        {
            var arch = Game.Arch switch { GameArch.X64 => "x64", GameArch.X86 => "x86", _ => "?" };
            return Game.AppId > 0 ? $"AppID {Game.AppId}  •  {arch}" : $"No AppID  •  {arch}";
        }
    }

    private ImageSource? _icon;
    public ImageSource? Icon
    {
        get => _icon;
        set { _icon = value; Raise(nameof(Icon)); Raise(nameof(HasIcon)); Raise(nameof(NoIcon)); }
    }

    public bool HasIcon => _icon != null;
    public bool NoIcon => _icon == null;

    /// <summary>The live process while running, so STOP can terminate it.</summary>
    public System.Diagnostics.Process? RunningProcess { get; set; }

    private bool _isRunning;
    /// <summary>True while a launched instance of this game is alive.</summary>
    public bool IsRunning
    {
        get => _isRunning;
        set { _isRunning = value; Raise(nameof(IsRunning)); Raise(nameof(NotRunning)); Raise(nameof(PlayLabel)); }
    }
    public bool NotRunning => !_isRunning;

    /// <summary>Primary button label: PLAY when idle, STOP while running.</summary>
    public string PlayLabel => _isRunning ? "■  STOP" : "▶  PLAY";

    /// <summary>First letter fallback when no icon is available.</summary>
    public string Initial => string.IsNullOrWhiteSpace(Name) ? "?" : Name.Substring(0, 1).ToUpperInvariant();

    public void Refresh()
    {
        Icon = Images.FromBytes(Game.IconPng);
        Raise(nameof(Name));
        Raise(nameof(Subtitle));
        Raise(nameof(Initial));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Raise(string p) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
}
