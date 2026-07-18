using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using SKYNET.Client.Models;
using SKYNET.Client.Services;

namespace SKYNET.Client.Views;

public partial class GameSettingsWindow : Window
{
    private readonly GameEntry _game;

    public GameSettingsWindow(GameEntry game)
    {
        InitializeComponent();
        _game = game;

        TArch.ItemsSource = new[] { "Auto-detect", "x86 (32-bit)", "x64 (64-bit)" };
        Load();
    }

    private void Load()
    {
        TName.Text = _game.Name;
        TExe.Text = _game.ExecutablePath;
        TAppId.Text = _game.AppId.ToString();
        TArgs.Text = _game.LaunchArguments;
        TArch.SelectedIndex = _game.Arch switch { GameArch.X86 => 1, GameArch.X64 => 2, _ => 0 };

        var s = _game.Ini;
        TServerUrl.Text = s.ServerUrl;
        TLanguage.Text = s.Language;
        CSecure.IsChecked = s.SecureNetworking;
        CServerApi.IsChecked = s.UseServerApi;
        CActiveWeb.IsChecked = s.UseActiveWebUser;
        CUnlockDlc.IsChecked = s.UnlockAllDlc;
        CInventory.IsChecked = s.InventoryEnabled;
        CLogFile.IsChecked = s.LogToFile;
    }

    private bool Apply()
    {
        _game.Name = string.IsNullOrWhiteSpace(TName.Text) ? _game.Name : TName.Text.Trim();
        _game.ExecutablePath = TExe.Text.Trim();
        _game.LaunchArguments = TArgs.Text.Trim();

        if (uint.TryParse(TAppId.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var appId))
            _game.AppId = appId;
        else if (!string.IsNullOrWhiteSpace(TAppId.Text))
        {
            Dialog.Info(this, "Invalid App ID", "App ID must be a number.");
            return false;
        }

        _game.Arch = TArch.SelectedIndex switch
        {
            1 => GameArch.X86,
            2 => GameArch.X64,
            _ => PeArch.Detect(_game.ExecutablePath)
        };

        var s = _game.Ini;
        s.ServerUrl = string.IsNullOrWhiteSpace(TServerUrl.Text) ? s.ServerUrl : TServerUrl.Text.Trim();
        s.Language = string.IsNullOrWhiteSpace(TLanguage.Text) ? s.Language : TLanguage.Text.Trim();
        s.SecureNetworking = CSecure.IsChecked == true;
        s.UseServerApi = CServerApi.IsChecked == true;
        s.UseActiveWebUser = CActiveWeb.IsChecked == true;
        s.UnlockAllDlc = CUnlockDlc.IsChecked == true;
        s.InventoryEnabled = CInventory.IsChecked == true;
        s.LogToFile = CLogFile.IsChecked == true;
        return true;
    }

    private void ChangeExe_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Select the game executable",
            Filter = "Game executable (*.exe)|*.exe|All files (*.*)|*.*",
            CheckFileExists = true
        };
        if (dlg.ShowDialog(this) != true) return;
        TExe.Text = dlg.FileName;
        _game.IconPng = IconExtractor.ExtractPng(dlg.FileName);
        TArch.SelectedIndex = PeArch.Detect(dlg.FileName) switch { GameArch.X86 => 1, GameArch.X64 => 2, _ => 0 };
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!Apply()) return;
        DialogResult = true;
    }

    private async void Launch_Click(object sender, RoutedEventArgs e)
    {
        if (!Apply()) return;
        App.Store.Save();

        WebUser? user = null;
        try { user = (await App.Server.ResolveSessionAsync(App.Store.Config)).User; }
        catch { }

        var result = App.Launcher.Launch(_game, App.Store.Config, user);
        if (!result.Success)
        {
            Dialog.Info(this, "Launch failed", result.Error ?? "Unknown error");
            return;
        }
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void Bar_Drag(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed) DragMove();
    }
}
