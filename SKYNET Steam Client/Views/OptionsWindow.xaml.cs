using System.Globalization;
using System.Windows;
using System.Windows.Input;
using SKYNET.Client.Models;
using SKYNET.Client.Services;

namespace SKYNET.Client.Views;

public partial class OptionsWindow : Window
{
    private readonly AppConfig _config;

    public OptionsWindow(AppConfig config)
    {
        InitializeComponent();
        _config = config;
        TServerUrl.Text = config.ServerUrl;
        TDiscoveryPort.Text = config.DiscoveryPort.ToString();
        CAutoDiscover.IsChecked = config.AutoDiscoverServer;
    }

    private async void Detect_Click(object sender, RoutedEventArgs e)
    {
        DetectBtn.IsEnabled = false;
        DetectStatus.Text = "Searching for server on the network...";

        int port = int.TryParse(TDiscoveryPort.Text.Trim(), out var p) ? p : 27081;
        var found = await Task.Run(() => ServerDiscovery.Discover(port));

        if (!string.IsNullOrWhiteSpace(found))
        {
            TServerUrl.Text = found;
            DetectStatus.Text = $"Found: {found}";
        }
        else
        {
            DetectStatus.Text = "No server answered the broadcast. Enter the URL manually.";
        }
        DetectBtn.IsEnabled = true;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var url = TServerUrl.Text.Trim();
        if (string.IsNullOrWhiteSpace(url))
        {
            Dialog.Info(this, "Invalid URL", "Server URL cannot be empty.");
            return;
        }
        if (!url.StartsWith("http://", System.StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", System.StringComparison.OrdinalIgnoreCase))
            url = "http://" + url;
        if (!url.EndsWith("/")) url += "/";

        _config.ServerUrl = url;
        if (int.TryParse(TDiscoveryPort.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var port))
            _config.DiscoveryPort = port;
        _config.AutoDiscoverServer = CAutoDiscover.IsChecked == true;

        App.Store.Save();
        App.Server.Configure(_config.ServerUrl);
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void Bar_Drag(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed) DragMove();
    }
}
