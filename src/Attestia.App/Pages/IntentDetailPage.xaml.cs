using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Attestia.Core.Models;
using Attestia.ViewModels;

namespace Attestia.App.Pages;

public sealed partial class IntentDetailPage : Page
{
    private readonly IntentDetailViewModel _vm;

    public IntentDetailPage()
    {
        InitializeComponent();
        _vm = App.GetService<IntentDetailViewModel>();

        _vm.PropertyChanged += (_, e) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (e.PropertyName == nameof(IntentDetailViewModel.Intent))
                    UpdateDisplay();
                else if (e.PropertyName == nameof(IntentDetailViewModel.IsBusy))
                    LoadingRing.IsActive = _vm.IsBusy;
                else if (e.PropertyName == nameof(IntentDetailViewModel.ErrorMessage))
                {
                    ErrorText.Text = _vm.ErrorMessage ?? "";
                    RetryBtn.Visibility = string.IsNullOrEmpty(_vm.ErrorMessage)
                        ? Visibility.Collapsed : Visibility.Visible;
                }
            });
        };
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is string intentId)
        {
            await _vm.LoadCommand.ExecuteAsync(intentId);
        }
    }

    private void UpdateDisplay()
    {
        var intent = _vm.Intent;
        if (intent is null) return;

        IntentIdText.Text = intent.Id;
        StatusText.Text = intent.Status.ToString();
        KindText.Text = intent.Kind;
        DeclaredByText.Text = intent.DeclaredBy;
        DeclaredAtText.Text = intent.DeclaredAt;
        DescriptionText.Text = intent.Description;

        ParamsText.Text = intent.Params.Count > 0
            ? JsonSerializer.Serialize(intent.Params, new JsonSerializerOptions { WriteIndented = true })
            : "(none)";

        // Show contextual action panels
        ApproveRejectPanel.Visibility = intent.Status == IntentStatus.Declared
            ? Visibility.Visible : Visibility.Collapsed;
        ExecutePanel.Visibility = intent.Status == IntentStatus.Approved
            ? Visibility.Visible : Visibility.Collapsed;
        VerifyPanel.Visibility = intent.Status == IntentStatus.Executed
            ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
    }

    private async void Approve_Click(object sender, RoutedEventArgs e)
    {
        await _vm.ApproveCommand.ExecuteAsync(null);
    }

    private async void Reject_Click(object sender, RoutedEventArgs e)
    {
        _vm.RejectReason = RejectReasonBox.Text?.Trim();
        await _vm.RejectCommand.ExecuteAsync(null);
    }

    private async void Execute_Click(object sender, RoutedEventArgs e)
    {
        _vm.ChainId = ChainIdBox.Text?.Trim();
        _vm.TxHash = TxHashBox.Text?.Trim();
        await _vm.ExecuteCommand.ExecuteAsync(null);
    }

    private async void Verify_Click(object sender, RoutedEventArgs e)
    {
        _vm.VerifyMatched = MatchedCheck.IsChecked == true;
        await _vm.VerifyCommand.ExecuteAsync(null);
    }

    private async void Retry_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.Intent is not null)
            await _vm.LoadCommand.ExecuteAsync(_vm.Intent.Id);
    }
}
