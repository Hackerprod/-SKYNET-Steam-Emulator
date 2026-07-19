using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using SKYNET.Client.Models;
using SKYNET.Client.Services;
using SKYNET.Client.ViewModels;

namespace SKYNET.Client;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<GameCardVm> _cards = new();
    private SessionResult? _session;

    public MainWindow()
    {
        InitializeComponent();
        GamesItems.ItemsSource = _cards;
        App.Launcher.GameExited += OnGameExited;

        LoadGames();
        Loaded += async (_, _) => await RefreshSessionAsync();
    }

    // ================= views =================

    private void Nav_Click(object sender, MouseButtonEventArgs e)
    {
        // Handle here so the click does not bubble to the header's DragMove, which
        // would otherwise swallow the interaction and block navigation.
        e.Handled = true;
        if (sender is FrameworkElement fe && fe.Tag is string view) SwitchView(view);
    }

    private void SwitchView(string view)
    {
        DashboardView.Visibility = view == "Dashboard" ? Visibility.Visible : Visibility.Collapsed;
        UsersView.Visibility = view == "Users" ? Visibility.Visible : Visibility.Collapsed;
        StatsView.Visibility = view == "Stats" ? Visibility.Visible : Visibility.Collapsed;

        var accent = (Brush)FindResource("SkynetAccent");
        var muted = (Brush)FindResource("SkynetMuted");
        NavDashboard.Foreground = view == "Dashboard" ? accent : muted;
        NavUsers.Foreground = view == "Users" ? accent : muted;
        NavStats.Foreground = view == "Stats" ? accent : muted;

        if (view == "Users") _ = LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        // The session token (resolved at startup) can go stale by the time the user
        // opens this view, which made the list intermittently come back empty. Make
        // sure we hold a fresh session, and retry once with a refreshed token if the
        // first fetch returns nothing.
        if (_session?.Status != SessionStatus.Authenticated || string.IsNullOrEmpty(_session?.AccessToken))
            await RefreshSessionAsync();

        var users = await App.Server.GetUsersAsync(_session?.AccessToken);
        if (users.Count == 0 && _session?.Status == SessionStatus.Authenticated)
        {
            await RefreshSessionAsync();
            users = await App.Server.GetUsersAsync(_session?.AccessToken);
        }

        UsersItems.ItemsSource = users.Select(u => new UserItemVm(u)).ToList();
        UsersCount.Text = users.Count == 1 ? "1 user" : $"{users.Count} users";
        UsersEmpty.Visibility = users.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    // ================= data =================

    private void LoadGames()
    {
        _cards.Clear();
        foreach (var g in App.Store.Config.Games.OrderByDescending(g => g.LastPlayedUtc ?? g.AddedUtc))
            _cards.Add(new GameCardVm(g));
        SaveAndRefreshCounts();
    }

    private void SaveAndRefreshCounts()
    {
        LibraryCount.Text = _cards.Count == 1 ? "1 game" : $"{_cards.Count} games";
        EmptyState.Visibility = _cards.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnGameExited(GameEntry game)
    {
        Dispatcher.Invoke(() =>
        {
            var vm = _cards.FirstOrDefault(c => c.Game.Id == game.Id);
            if (vm != null) { vm.IsRunning = false; vm.RunningProcess = null; }
        });
    }

    private async Task RefreshSessionAsync()
    {
        _session = await App.Server.ResolveSessionAsync(App.Store.Config);
        // Persist a server URL that discovery may have updated.
        App.Store.Save();
        BuildUserPanel(_session);
    }

    private void Options_Click(object sender, RoutedEventArgs e)
    {
        var win = new Views.OptionsWindow(App.Store.Config) { Owner = this };
        if (win.ShowDialog() == true)
        {
            _ = RefreshSessionAsync();
        }
    }

    private void BuildUserPanel(SessionResult session)
    {
        if (session.Status == SessionStatus.Authenticated && session.User != null)
        {
            LoginBanner.Visibility = Visibility.Collapsed;
            UserPanelHost.Content = BuildUserBadge(session.User);
            return;
        }

        var login = new Button { Content = "LOGIN", Style = (Style)FindResource("AccentButton") };
        login.Click += Login_Click;
        UserPanelHost.Content = login;

        LoginBanner.Visibility = Visibility.Visible;
        if (session.Status == SessionStatus.ServerUnavailable)
        {
            BannerTitle.Text = "Server unavailable";
            BannerText.Text = $"Could not reach the SKYNET server at {App.Store.Config.ServerUrl}. Set the server address or auto-detect it.";
            BannerButton.Content = "SERVER OPTIONS";
        }
        else
        {
            BannerTitle.Text = "Not signed in";
            BannerText.Text = "Sign in on the SKYNET web to link this launcher to your account.";
            BannerButton.Content = "OPEN WEB LOGIN";
        }
    }

    private FrameworkElement BuildUserBadge(WebUser user)
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

        var name = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 10, 0) };
        name.Children.Add(new TextBlock
        {
            Text = user.DisplayName,
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            HorizontalAlignment = HorizontalAlignment.Right,
            Foreground = (Brush)FindResource("SkynetText")
        });
        name.Children.Add(new TextBlock
        {
            Text = user.Online ? "ONLINE" : "OFFLINE",
            FontSize = 9,
            HorizontalAlignment = HorizontalAlignment.Right,
            Foreground = (Brush)FindResource(user.Online ? "SkynetAccent" : "SkynetMuted")
        });
        panel.Children.Add(name);

        var avatarSrc = Images.FromBytes(user.AvatarPng);
        var avatar = new Border
        {
            Width = 34,
            Height = 34,
            CornerRadius = new CornerRadius(17),
            Background = (Brush)FindResource("SkynetDark"),
            BorderBrush = (Brush)FindResource("SkynetBorder"),
            BorderThickness = new Thickness(1),
            ClipToBounds = true
        };
        if (avatarSrc != null)
            avatar.Child = new Image { Source = avatarSrc, Stretch = Stretch.UniformToFill };
        else
            avatar.Child = new TextBlock
            {
                Text = string.IsNullOrEmpty(user.DisplayName) ? "?" : user.DisplayName.Substring(0, 1).ToUpperInvariant(),
                FontWeight = FontWeights.Bold,
                Foreground = (Brush)FindResource("SkynetMuted"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        panel.Children.Add(avatar);
        return panel;
    }

    // ================= window chrome =================

    private void Header_Drag(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed) DragMove();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        // Server unreachable -> open options so the user can set/detect the address.
        if (_session?.Status == SessionStatus.ServerUnavailable)
        {
            Options_Click(sender, e);
            return;
        }
        OpenWeb("Auth/Login");
        _ = RecheckAfterDelay();
    }

    private async Task RecheckAfterDelay()
    {
        await Task.Delay(4000);
        await RefreshSessionAsync();
    }

    private void OpenWeb(string relative)
    {
        try
        {
            var baseUrl = App.Store.Config.ServerUrl.EndsWith("/") ? App.Store.Config.ServerUrl : App.Store.Config.ServerUrl + "/";
            Process.Start(new ProcessStartInfo(baseUrl + relative) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Views.Dialog.Info(this, "Open web", ex.Message);
        }
    }

    // ================= library actions =================

    private void AddGame_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Select the game executable",
            Filter = "Game executable (*.exe)|*.exe|All files (*.*)|*.*",
            CheckFileExists = true
        };
        if (dlg.ShowDialog(this) != true) return;

        var path = dlg.FileName;
        var game = new GameEntry
        {
            Name = Path.GetFileNameWithoutExtension(path),
            ExecutablePath = path,
            Arch = PeArch.Detect(path),
            AppId = TryReadAppId(path),
            IconPng = IconExtractor.ExtractPng(path)
        };

        App.Store.Config.Games.Add(game);
        App.Store.Save();
        LoadGames();
        OpenSettings(game);
    }

    private static uint TryReadAppId(string exePath)
    {
        try
        {
            var txt = Path.Combine(Path.GetDirectoryName(exePath) ?? "", "steam_appid.txt");
            if (File.Exists(txt) && uint.TryParse(File.ReadAllText(txt).Trim(), out var id)) return id;
        }
        catch { }
        return 0;
    }

    private void PlayStop_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not GameCardVm vm) return;

        // Running -> STOP: terminate the process. Its Exited event restores the DLL
        // and flips the card back to PLAY (via OnGameExited).
        if (vm.IsRunning)
        {
            try
            {
                var p = vm.RunningProcess;
                if (p != null && !p.HasExited)
                {
                    p.CloseMainWindow();
                    if (!p.WaitForExit(1500)) p.Kill();
                }
            }
            catch { /* already gone */ }
            return;
        }

        var result = App.Launcher.Launch(vm.Game, App.Store.Config, _session?.User);
        App.Store.Save();
        if (!result.Success)
        {
            Views.Dialog.Info(this, "Launch failed", result.Error ?? "Unknown error");
            return;
        }
        vm.RunningProcess = result.Process;
        vm.IsRunning = true;
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is GameCardVm vm) OpenSettings(vm.Game);
    }

    private void OpenSettings(GameEntry game)
    {
        var win = new Views.GameSettingsWindow(game) { Owner = this };
        if (win.ShowDialog() == true)
        {
            App.Store.Save();
            LoadGames();
        }
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not GameCardVm vm) return;
        if (!Views.Dialog.Confirm(this, "Remove game",
                $"Remove \"{vm.Game.Name}\" from the library?", "REMOVE")) return;

        App.Store.Config.Games.RemoveAll(g => g.Id == vm.Game.Id);
        App.Store.Save();
        LoadGames();
    }
}
