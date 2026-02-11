using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Attestia.ViewModels;

namespace Attestia.App.Pages;

public sealed partial class DeclareIntentPage : Page
{
    private readonly DeclareIntentViewModel _vm;

    public DeclareIntentPage()
    {
        InitializeComponent();
        _vm = App.GetService<DeclareIntentViewModel>();

        _vm.PropertyChanged += (_, e) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (e.PropertyName == nameof(DeclareIntentViewModel.IsBusy))
                    LoadingRing.IsActive = _vm.IsBusy;
                else if (e.PropertyName == nameof(DeclareIntentViewModel.ErrorMessage))
                    ErrorText.Text = _vm.ErrorMessage ?? "";
                else if (e.PropertyName == nameof(DeclareIntentViewModel.DeclaredIntent))
                    OnDeclared();
            });
        };
    }

    private void OnDeclared()
    {
        if (_vm.DeclaredIntent is null) return;

        SuccessPanel.Visibility = Visibility.Visible;
        DeclaredIdText.Text = _vm.DeclaredIntent.Id;
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
    }

    private async void Declare_Click(object sender, RoutedEventArgs e)
    {
        _vm.IntentId = IntentIdBox.Text?.Trim();
        _vm.Kind = (KindCombo.SelectedItem as ComboBoxItem)?.Content as string ?? "transfer";
        _vm.Description = DescriptionBox.Text?.Trim();
        _vm.FromAddress = FromBox.Text?.Trim();
        _vm.ToAddress = ToBox.Text?.Trim();
        _vm.Amount = AmountBox.Text?.Trim();
        _vm.Currency = (CurrencyCombo.SelectedItem as ComboBoxItem)?.Content as string ?? "USD";
        _vm.ChainId = ChainIdBox.Text?.Trim();

        await _vm.DeclareCommand.ExecuteAsync(null);
    }

    private void ViewDeclared_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.DeclaredIntent is not null)
        {
            Frame.Navigate(typeof(IntentDetailPage), _vm.DeclaredIntent.Id);
        }
    }
}
