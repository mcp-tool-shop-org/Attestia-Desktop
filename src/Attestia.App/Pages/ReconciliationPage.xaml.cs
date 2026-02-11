using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Attestia.Core.Models;
using Attestia.ViewModels;

namespace Attestia.App.Pages;

public sealed partial class ReconciliationPage : Page
{
    public ReconciliationViewModel ViewModel { get; }

    public ReconciliationPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ReconciliationViewModel>();

        ViewModel.PropertyChanged += (_, e) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (e.PropertyName == nameof(ReconciliationViewModel.IsBusy))
                {
                    LoadingRing.IsActive = ViewModel.IsBusy;
                    if (!ViewModel.IsBusy)
                    {
                        var hasItems = ViewModel.Attestations.Count > 0;
                        var hasError = !string.IsNullOrEmpty(ViewModel.ErrorMessage);
                        EmptyState.Visibility = !hasItems && !hasError ? Visibility.Visible : Visibility.Collapsed;
                        AttestationsListPanel.Visibility = hasItems ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
                else if (e.PropertyName == nameof(ReconciliationViewModel.ErrorMessage))
                {
                    ErrorText.Text = ViewModel.ErrorMessage ?? "";
                    RetryBtn.Visibility = string.IsNullOrEmpty(ViewModel.ErrorMessage)
                        ? Visibility.Collapsed : Visibility.Visible;
                }
            });
        };
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadAttestationsCommand.ExecuteAsync(null);
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadAttestationsCommand.ExecuteAsync(null);
    }

    private void Attestation_Selected(object sender, SelectionChangedEventArgs e)
    {
        if (AttestationsList.SelectedItem is AttestationRecord record)
        {
            SummaryPanel.Visibility = Visibility.Visible;
            var summary = record.Summary;
            SumIntents.Text = summary.TotalIntents.ToString();
            SumMatched.Text = summary.MatchedCount.ToString();
            SumMismatched.Text = summary.MismatchCount.ToString();
            SumMissing.Text = summary.MissingCount.ToString();

            if (summary.Discrepancies.Count > 0)
            {
                DiscrepanciesPanel.Visibility = Visibility.Visible;
                DiscrepanciesList.ItemsSource = summary.Discrepancies;
            }
            else
            {
                DiscrepanciesPanel.Visibility = Visibility.Collapsed;
            }
        }
        else
        {
            SummaryPanel.Visibility = Visibility.Collapsed;
        }
    }
}
