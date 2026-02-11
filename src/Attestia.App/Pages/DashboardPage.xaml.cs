using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
                IntentCountText.Text = _vm.TotalIntents.ToString();
                LeafCountText.Text = _vm.MerkleLeafCount.ToString();
                AttestationText.Text = _vm.LatestAttestationId ?? "None";
                MerkleRootText.Text = _vm.MerkleRoot ?? "—";
                SidecarText.Text = _vm.SidecarStatus.ToString();
                LoadingRing.IsActive = _vm.IsBusy;
                ErrorText.Text = _vm.ErrorMessage ?? "";
            });
        };
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
