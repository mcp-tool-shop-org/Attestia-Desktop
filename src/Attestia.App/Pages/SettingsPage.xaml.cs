using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Attestia.Sidecar;
using Attestia.ViewModels;

namespace Attestia.App.Pages;

public sealed partial class SettingsPage : Page
{
    private readonly SettingsViewModel _vm;
    private readonly NodeSidecar _sidecar;

    public SettingsPage()
    {
        InitializeComponent();

        _vm = App.GetService<SettingsViewModel>();
        _sidecar = App.GetService<NodeSidecar>();
        UpdateSidecarInfo();

        _sidecar.StatusChanged += (_, _) => DispatcherQueue.TryEnqueue(UpdateSidecarInfo);
    }

    private void UpdateSidecarInfo()
    {
        SidecarStatusText.Text = _vm.SidecarStatus.ToString();
        PortText.Text = _vm.ServerPort > 0 ? _vm.ServerPort.ToString() : "—";
        UrlText.Text = _vm.ServerPort > 0 ? $"http://localhost:{_vm.ServerPort}" : "—";
    }

    private async void RestartSidecar_Click(object sender, RoutedEventArgs e)
    {
        SidecarLoading.IsActive = true;
        try
        {
            await _sidecar.StopAsync();
            await _sidecar.StartAsync();
        }
        catch (Exception ex)
        {
            SidecarStatusText.Text = $"Error: {ex.Message}";
        }
        finally
        {
            SidecarLoading.IsActive = false;
        }
    }
}
