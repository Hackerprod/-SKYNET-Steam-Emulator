using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
    private readonly ICollectionView _cardsView;
    private string _search = "";
    private SessionResult? _session;
    private bool _loadingSortUi;

    public MainWindow()
    {
        InitializeComponent();
        _cardsView = CollectionViewSource.GetDefaultView(_cards);
        _cardsView.Filter = FilterCard;
        GamesItems.ItemsSource = _cardsView;
        App.Launcher.GameExited += OnGameExited;
        StateChanged += (_, _) => UpdateMaximizeGlyph();
        UpdateMaximizeGlyph();

        InitSortUi();
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
        AdminView.Visibility = view == "Admin" ? Visibility.Visible : Visibility.Collapsed;

        var accent = (Brush)FindResource("SkynetAccent");
        var muted = (Brush)FindResource("SkynetMuted");
        NavDashboard.Foreground = view == "Dashboard" ? accent : muted;
        NavUsers.Foreground = view == "Users" ? accent : muted;
        NavStats.Foreground = view == "Stats" ? accent : muted;
        NavAdmin.Foreground = view == "Admin" ? accent : muted;

        if (view == "Users") _ = LoadUsersAsync();
        if (view == "Admin") _ = LoadAdminAsync();
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

    private async Task LoadAdminAsync()
    {
        if (_session?.Status != SessionStatus.Authenticated || string.IsNullOrEmpty(_session?.AccessToken))
            await RefreshSessionAsync();

        var overview = await App.Server.GetAdminOverviewAsync(_session?.AccessToken);
        if (overview == null)
        {
            AdminDataPanel.Visibility = Visibility.Collapsed;
            AdminEmpty.Visibility = Visibility.Visible;
            return;
        }

        AdminDataPanel.DataContext = new AdminOverviewVm(overview);
        AdminDataPanel.Visibility = Visibility.Visible;
        AdminEmpty.Visibility = Visibility.Collapsed;
    }

    private void AdminRefresh_Click(object sender, RoutedEventArgs e) => _ = LoadAdminAsync();

    private void AdminOpenPanel_Click(object sender, RoutedEventArgs e) => OpenWeb("admin");

    // ================= data =================

    private void LoadGames()
    {
        _cards.Clear();
        foreach (var g in SortGames(App.Store.Config.Games))
            _cards.Add(new GameCardVm(g));
        SaveAndRefreshCounts();
    }

    private static IEnumerable<GameEntry> SortGames(IEnumerable<GameEntry> games)
    {
        var descending = App.Store.Config.LibrarySortDescending;
        return App.Store.Config.LibrarySortMode switch
        {
            "Name" => descending
                ? games.OrderByDescending(g => g.Name, StringComparer.OrdinalIgnoreCase)
                : games.OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase),
            "DateAdded" => descending
                ? games.OrderByDescending(g => g.AddedUtc)
                : games.OrderBy(g => g.AddedUtc),
            "AppId" => descending
                ? games.OrderByDescending(g => g.AppId)
                : games.OrderBy(g => g.AppId),
            _ => descending
                ? games.OrderByDescending(g => g.LastPlayedUtc ?? g.AddedUtc)
                : games.OrderBy(g => g.LastPlayedUtc ?? g.AddedUtc)
        };
    }

    private void InitSortUi()
    {
        _loadingSortUi = true;
        var mode = App.Store.Config.LibrarySortMode;
        foreach (ComboBoxItem item in SortModeCombo.Items)
        {
            if (Equals(item.Tag, mode))
            {
                SortModeCombo.SelectedItem = item;
                break;
            }
        }
        SortModeCombo.SelectedItem ??= SortModeCombo.Items[0];
        SortDirectionButton.Content = App.Store.Config.LibrarySortDescending ? "↓" : "↑";
        _loadingSortUi = false;
    }

    private void SortModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_loadingSortUi || SortModeCombo.SelectedItem is not ComboBoxItem item) return;
        App.Store.Config.LibrarySortMode = (string)item.Tag;
        App.Store.Save();
        ReorderCards();
    }

    private void SortDirection_Click(object sender, RoutedEventArgs e)
    {
        App.Store.Config.LibrarySortDescending = !App.Store.Config.LibrarySortDescending;
        SortDirectionButton.Content = App.Store.Config.LibrarySortDescending ? "↓" : "↑";
        App.Store.Save();
        ReorderCards();
    }

    /// <summary>Re-sorts the existing card view models in place instead of rebuilding
    /// them from config, so in-memory-only state (IsRunning/RunningProcess) survives
    /// a sort change instead of resetting every card to "not running".</summary>
    private void ReorderCards()
    {
        var byId = _cards.ToDictionary(c => c.Game.Id);
        var orderedIds = SortGames(_cards.Select(c => c.Game)).Select(g => g.Id).ToList();
        _cards.Clear();
        foreach (var id in orderedIds)
            _cards.Add(byId[id]);
    }

    private void SaveAndRefreshCounts()
    {
        LibraryCount.Text = _cards.Count == 1 ? "1 game" : $"{_cards.Count} games";
        UpdateEmptyState();
    }

    private void UpdateEmptyState()
    {
        if (_cards.Count == 0)
        {
            EmptyState.Text = "No games yet.\nClick ADD GAME and pick a game executable to get started.";
            EmptyState.Visibility = Visibility.Visible;
        }
        else if (_cardsView.IsEmpty)
        {
            EmptyState.Text = "No games match your search.";
            EmptyState.Visibility = Visibility.Visible;
        }
        else
        {
            EmptyState.Visibility = Visibility.Collapsed;
        }
    }

    private bool FilterCard(object o) =>
        string.IsNullOrEmpty(_search) || o is not GameCardVm vm ||
        vm.Name.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0 ||
        vm.Game.AppId.ToString().IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0;

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _search = SearchBox.Text.Trim();
        _cardsView.Refresh();
        UpdateEmptyState();
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
        var isAdmin = _session.User?.IsAdmin == true;
        NavAdmin.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        if (!isAdmin && AdminView.Visibility == Visibility.Visible) SwitchView("Dashboard");
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
            PopulateProfileCard(session.User);
            return;
        }

        PlayerProfileCard.Visibility = Visibility.Collapsed;
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

    private void PopulateProfileCard(WebUser user)
    {
        PlayerProfileCard.Visibility = Visibility.Visible;
        ProfileDisplayName.Text = user.DisplayName;

        var isOnline = user.Online;
        ProfileStatusText.Text = isOnline ? "ONLINE" : "OFFLINE";
        ProfileStatusText.Foreground = (Brush)FindResource(isOnline ? "SkynetAccent" : "SkynetMuted");
        ProfileStatusDot.Fill = (Brush)FindResource(isOnline ? "SkynetAccent" : "SkynetMuted");

        var avatarSrc = Images.FromBytes(user.AvatarPng);
        if (avatarSrc != null)
        {
            ProfileAvatarBorder.Background = new ImageBrush(avatarSrc)
            {
                Stretch = Stretch.UniformToFill
            };
            ProfileAvatarInitial.Visibility = Visibility.Collapsed;
        }
        else
        {
            ProfileAvatarBorder.Background = (Brush)FindResource("SkynetDark");
            ProfileAvatarInitial.Text = string.IsNullOrEmpty(user.DisplayName)
                ? "?"
                : user.DisplayName.Substring(0, 1).ToUpperInvariant();
            ProfileAvatarInitial.Visibility = Visibility.Visible;
        }
    }

    // ================= window chrome =================

    private void Header_Drag(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ToggleMaximizeRestore();
            return;
        }

        if (e.ButtonState != MouseButtonState.Pressed) return;

        if (WindowState == WindowState.Maximized)
        {
            // DragMove() on a maximized borderless window doesn't drag it like native
            // chrome does -- it was left stuck maximized with no way to get it back.
            // Restore first, repositioned so the window stays under the cursor, then drag.
            var ratioX = e.GetPosition(this).X / ActualWidth;
            var screenPoint = PointToScreen(e.GetPosition(this));

            WindowState = WindowState.Normal;

            Left = screenPoint.X - (Width * ratioX);
            Top = screenPoint.Y - 10;
        }

        DragMove();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    private void MaximizeRestore_Click(object sender, RoutedEventArgs e) => ToggleMaximizeRestore();
    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void ToggleMaximizeRestore()
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void UpdateMaximizeGlyph()
    {
        var maximized = WindowState == WindowState.Maximized;
        MaximizeRestoreButton.Content = maximized ? "❐" : "□";
        MaximizeRestoreButton.ToolTip = maximized ? "Restore" : "Maximize";
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (App.Store.Config.CloseMinimizesToTray && !App.IsExiting)
        {
            e.Cancel = true;
            Hide();
            return;
        }
        base.OnClosing(e);
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        // Server unreachable -> open options so the user can set/detect the address.
        if (_session?.Status == SessionStatus.ServerUnavailable)
        {
            Options_Click(sender, e);
            return;
        }
        OpenWeb("login");
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

    private async void PlayStop_Click(object sender, RoutedEventArgs e)
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

        await RefreshSessionAsync();
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
