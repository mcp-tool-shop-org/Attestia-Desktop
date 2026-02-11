using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Attestia.Sidecar;
using Attestia.ViewModels;

namespace Attestia.App.Pages;

public sealed partial class DashboardPage : Page
{
    private readonly DashboardViewModel _vm;

    // Muted institutional colors
    private static readonly Windows.UI.Color HealthyColor = Windows.UI.Color.FromArgb(255, 76, 122, 91);
    private static readonly Windows.UI.Color WarningColor = Windows.UI.Color.FromArgb(255, 194, 161, 74);
    private static readonly Windows.UI.Color ErrorColor = Windows.UI.Color.FromArgb(255, 168, 90, 90);
    private static readonly Windows.UI.Color NeutralColor = Windows.UI.Color.FromArgb(255, 74, 90, 111);

    public DashboardPage()
    {
        InitializeComponent();
        _vm = App.GetService<DashboardViewModel>();

        _vm.PropertyChanged += (_, e) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                LoadingRing.IsActive = _vm.IsBusy;
                UpdatePipeline();
                UpdateIntegrityAnchor();
                UpdateEngineStatus();

                OfflineBanner.Visibility = _vm.IsEngineOffline
                    ? Visibility.Visible : Visibility.Collapsed;

                var isFirstRun = !_vm.IsEngineOffline && !_vm.IsBusy
                    && _vm.TotalIntents == 0 && _vm.MerkleLeafCount == 0;
                GettingStartedCard.Visibility = isFirstRun
                    ? Visibility.Visible : Visibility.Collapsed;
            });
        };
    }

    private void UpdatePipeline()
    {
        PipelineIntentCount.Text = _vm.TotalIntents.ToString();
        PipelineAttestedCount.Text = _vm.LatestAttestationId is not null ? "1+" : "0";
        PipelineReconciledText.Text = _vm.LatestAttestationId ?? "None";
        PipelineProofCount.Text = _vm.MerkleLeafCount.ToString();
        PipelineCompliantText.Text = "—";
    }

    private void UpdateIntegrityAnchor()
    {
        MerkleRootText.Text = _vm.MerkleRoot ?? "Not yet computed";
        IntegrityLeafCount.Text = _vm.MerkleLeafCount.ToString();

        var hasRoot = !string.IsNullOrEmpty(_vm.MerkleRoot);
        IntegrityBadge.Visibility = hasRoot ? Visibility.Visible : Visibility.Collapsed;
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

        EngineIndicator.Fill = new SolidColorBrush(status switch
        {
            SidecarStatus.Running => HealthyColor,
            SidecarStatus.Starting => WarningColor,
            SidecarStatus.Degraded => WarningColor,
            SidecarStatus.Crashed or SidecarStatus.Error => ErrorColor,
            _ => NeutralColor,
        });
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await _vm.RefreshCommand.ExecuteAsync(null);
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await _vm.RefreshCommand.ExecuteAsync(null);
    }

    private void DeclareIntent_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(DeclareIntentPage));
    }
}
