using System.Windows;
using System.Windows.Input;

namespace SKYNET.Client.Views;

/// <summary>Dark themed replacement for the native (white) MessageBox.</summary>
public partial class Dialog : Window
{
    private Dialog() => InitializeComponent();

    /// <summary>Yes/No confirmation. Returns true if the user confirmed.</summary>
    public static bool Confirm(Window? owner, string title, string message, string okText = "CONFIRM")
    {
        var d = new Dialog { Owner = owner };
        d.TitleText.Text = title;
        d.MessageText.Text = message;
        d.OkBtn.Content = okText;
        d.CancelBtn.Visibility = Visibility.Visible;
        return d.ShowDialog() == true;
    }

    /// <summary>Single-button information dialog.</summary>
    public static void Info(Window? owner, string title, string message)
    {
        var d = new Dialog { Owner = owner };
        d.TitleText.Text = title;
        d.MessageText.Text = message;
        d.OkBtn.Content = "OK";
        d.CancelBtn.Visibility = Visibility.Collapsed;
        d.ShowDialog();
    }

    private void Ok_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void Root_Drag(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed) DragMove();
    }
}
