using Microsoft.UI.Xaml.Controls;
using Attestia.Sidecar;
using Attestia.ViewModels;

namespace Attestia.App.Pages;

public sealed partial class SettingsPage : Page
{
    private readonly SettingsViewModel _vm;

    public SettingsPage()
    {
        InitializeComponent();

        _vm = App.GetService<SettingsViewModel>();
        UpdateSidecarInfo();

        var sidecar = App.GetService<NodeSidecar>();
        sidecar.StatusChanged += (_, _) => DispatcherQueue.TryEnqueue(UpdateSidecarInfo);
    }

    private void UpdateSidecarInfo()
    {
        SidecarStatusText.Text = _vm.SidecarStatus.ToString();
        PortText.Text = _vm.ServerPort > 0 ? _vm.ServerPort.ToString() : "—";
        UrlText.Text = _vm.ServerPort > 0 ? $"http://localhost:{_vm.ServerPort}" : "—";
    }
}
