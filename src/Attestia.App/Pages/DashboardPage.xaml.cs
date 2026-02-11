using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Attestia.Sidecar;
using Attestia.ViewModels;

namespace Attestia.App.Pages;

public sealed partial class DashboardPage : Page
{
    private readonly DashboardViewModel _vm;

    public DashboardPage()
    {
        InitializeComponent();
        _vm = App.GetService<DashboardViewModel>();

        _vm.PropertyChanged += (_, e) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                LoadingRing.IsActive = _vm.IsBusy;
                IntentCountText.Text = _vm.TotalIntents.ToString();
                LeafCountText.Text = _vm.MerkleLeafCount.ToString();
                AttestationText.Text = _vm.LatestAttestationId ?? "None yet";
                MerkleRootText.Text = _vm.MerkleRoot ?? "Not yet computed";
                UpdateEngineStatus();

                // Show offline banner when engine is unreachable
                OfflineBanner.Visibility = _vm.IsEngineOffline
                    ? Visibility.Visible : Visibility.Collapsed;

                // Show getting started when engine is online but no data
                var isFirstRun = !_vm.IsEngineOffline && !_vm.IsBusy
                    && _vm.TotalIntents == 0 && _vm.MerkleLeafCount == 0;
                GettingStartedCard.Visibility = isFirstRun
                    ? Visibility.Visible : Visibility.Collapsed;
            });
        };
    }

    private void UpdateEngineStatus()
    {
        var status = _vm.SidecarStatus;
        EngineStatusText.Text = status switch
        {
            SidecarStatus.Running => "Healthy",
            SidecarStatus.Starting => "Starting...",
            SidecarStatus.Degraded => "Degraded",
            SidecarStatus.Crashed or SidecarStatus.Error => "Offline",
            SidecarStatus.Stopped => "Stopped",
            _ => "...",
        };

        var color = status switch
        {
            SidecarStatus.Running => Colors.LimeGreen,
            SidecarStatus.Starting => Colors.Orange,
            SidecarStatus.Degraded => Colors.Gold,
            _ => Colors.Gray,
        };
        EngineIndicator.Fill = new SolidColorBrush(color);
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await _vm.RefreshCommand.ExecuteAsync(null);
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await _vm.RefreshCommand.ExecuteAsync(null);
    }
}
