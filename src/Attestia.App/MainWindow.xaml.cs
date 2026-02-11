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
        appWindow.Title = "Attestia — Financial Truth Infrastructure";
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
        catch (Exception ex)
        {
            UpdateStatus(SidecarStatus.Error);
            System.Diagnostics.Debug.WriteLine($"Sidecar start failed: {ex.Message}");
        }
    }

    private void OnSidecarStatusChanged(object? sender, SidecarStatus status)
    {
        DispatcherQueue.TryEnqueue(() => UpdateStatus(status));
    }

    private void UpdateStatus(SidecarStatus status)
    {
        StatusText.Text = $"Sidecar: {status.ToString().ToLowerInvariant()}";
        PortText.Text = _sidecar.Port > 0 ? $"port {_sidecar.Port}" : "";

        StatusIndicator.Fill = status switch
        {
            SidecarStatus.Running => new SolidColorBrush(Colors.LimeGreen),
            SidecarStatus.Starting or SidecarStatus.Stopping => new SolidColorBrush(Colors.Orange),
            SidecarStatus.Degraded => new SolidColorBrush(Colors.Yellow),
            SidecarStatus.Crashed or SidecarStatus.Error => new SolidColorBrush(Colors.Red),
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
