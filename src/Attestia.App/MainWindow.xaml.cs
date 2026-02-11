using Microsoft.UI.Xaml;

namespace Attestia.App;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var appWindow = this.AppWindow;
        appWindow.Title = "Attestia — Financial Truth Infrastructure";
        appWindow.Resize(new Windows.Graphics.SizeInt32(1280, 800));
    }
}
