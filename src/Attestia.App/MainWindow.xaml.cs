using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Attestia.App.Pages;
using Attestia.Sidecar;

namespace Attestia.App;

public sealed partial class MainWindow : Window
{
    private readonly NodeSidecar _sidecar;

    public MainWindow()
    {
        InitializeComponent();

        var appWindow = this.AppWindow;
        appWindow.Title = "Attestia";
        appWindow.Resize(new Windows.Graphics.SizeInt32(1280, 800));

        _sidecar = App.GetService<NodeSidecar>();
        _sidecar.StatusChanged += OnSidecarStatusChanged;

        // Select Dashboard on startup
        NavView.SelectedItem = NavView.MenuItems[0];

        _ = StartSidecarAsync();
    }

    private async Task StartSidecarAsync()
    {
        try
        {
            UpdateStatus(SidecarStatus.Starting);
            await _sidecar.StartAsync();
            UpdateStatus(SidecarStatus.Running);
        }
        catch
        {
            UpdateStatus(SidecarStatus.Error);
        }
    }

    private void OnSidecarStatusChanged(object? sender, SidecarStatus status)
    {
        DispatcherQueue.TryEnqueue(() => UpdateStatus(status));
    }

    private void UpdateStatus(SidecarStatus status)
    {
        StatusText.Text = status switch
        {
            SidecarStatus.Running => "Engine ready",
            SidecarStatus.Starting => "Starting engine...",
            SidecarStatus.Stopping => "Shutting down...",
            SidecarStatus.Degraded => "Engine degraded",
            SidecarStatus.Crashed => "Engine stopped unexpectedly",
            SidecarStatus.Error => "Engine offline",
            SidecarStatus.Stopped => "Engine stopped",
            _ => "Initializing...",
        };

        StatusIndicator.Fill = status switch
        {
            SidecarStatus.Running => new SolidColorBrush(Colors.LimeGreen),
            SidecarStatus.Starting or SidecarStatus.Stopping => new SolidColorBrush(Colors.Orange),
            SidecarStatus.Degraded => new SolidColorBrush(Colors.Gold),
            SidecarStatus.Crashed or SidecarStatus.Error => new SolidColorBrush(Colors.Tomato),
            _ => new SolidColorBrush(Colors.Gray),
        };
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
            return;
        }

        if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag)
        {
            var pageType = tag switch
            {
                "dashboard" => typeof(DashboardPage),
                "intents" => typeof(IntentsPage),
                "reconciliation" => typeof(ReconciliationPage),
                "proofs" => typeof(ProofsPage),
                "compliance" => typeof(CompliancePage),
                "events" => typeof(EventsPage),
                _ => typeof(DashboardPage),
            };
            ContentFrame.Navigate(pageType);
        }
    }
}
