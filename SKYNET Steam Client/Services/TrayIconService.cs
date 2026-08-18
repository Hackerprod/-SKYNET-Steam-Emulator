using System.Drawing;
using System.Windows.Forms;

namespace SKYNET.Client.Services;

/// <summary>System tray icon with Open/Restore and Exit actions.</summary>
public sealed class TrayIconService : IDisposable
{
    private readonly NotifyIcon _icon;

    public event Action? OpenRequested;
    public event Action? ExitRequested;

    public TrayIconService()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Open / Restore", null, (_, _) => OpenRequested?.Invoke());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => ExitRequested?.Invoke());

        _icon = new NotifyIcon
        {
            Icon = LoadAppIcon(),
            Text = "SKYNET",
            ContextMenuStrip = menu,
            Visible = true
        };
        _icon.DoubleClick += (_, _) => OpenRequested?.Invoke();
    }

    private static Icon LoadAppIcon()
    {
        try
        {
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Icon.ExtractAssociatedIcon(path) ?? SystemIcons.Application;
        }
        catch
        {
            return SystemIcons.Application;
        }
    }

    public void Dispose()
    {
        _icon.Visible = false;
        _icon.Dispose();
    }
}
