using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Attestia.ViewModels;

namespace Attestia.App.Pages;

public sealed partial class IntentsPage : Page
{
    public IntentsViewModel ViewModel { get; }

    public IntentsPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<IntentsViewModel>();

        ViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IntentsViewModel.IsBusy))
            {
                LoadingRing.IsActive = ViewModel.IsBusy;
            }
            else if (e.PropertyName == nameof(IntentsViewModel.ErrorMessage))
            {
                ErrorText.Text = ViewModel.ErrorMessage ?? "";
            }
            else if (e.PropertyName == nameof(IntentsViewModel.HasMore))
            {
                LoadMoreBtn.Visibility = ViewModel.HasMore ? Visibility.Visible : Visibility.Collapsed;
            }
        };
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadCommand.ExecuteAsync(null);
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadCommand.ExecuteAsync(null);
    }

    private async void LoadMore_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadMoreCommand.ExecuteAsync(null);
    }

    private async void StatusFilter_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (StatusFilter.SelectedItem is ComboBoxItem item)
        {
            var tag = item.Tag as string;
            ViewModel.StatusFilter = string.IsNullOrEmpty(tag) ? null : tag;
            await ViewModel.LoadCommand.ExecuteAsync(null);
        }
    }
}
