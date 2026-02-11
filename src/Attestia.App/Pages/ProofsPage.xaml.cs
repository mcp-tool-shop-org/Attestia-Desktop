using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Attestia.ViewModels;

namespace Attestia.App.Pages;

public sealed partial class ProofsPage : Page
{
    private readonly ProofsViewModel _vm;

    public ProofsPage()
    {
        InitializeComponent();
        _vm = App.GetService<ProofsViewModel>();

        _vm.PropertyChanged += (_, e) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (e.PropertyName == nameof(ProofsViewModel.MerkleRoot))
                    MerkleRootText.Text = _vm.MerkleRoot ?? "—";
                else if (e.PropertyName == nameof(ProofsViewModel.LeafCount))
                    LeafCountText.Text = _vm.LeafCount.ToString();
                else if (e.PropertyName == nameof(ProofsViewModel.IsBusy))
                    LoadingRing.IsActive = _vm.IsBusy;
                else if (e.PropertyName == nameof(ProofsViewModel.ErrorMessage))
                    ErrorText.Text = _vm.ErrorMessage ?? "";
                else if (e.PropertyName == nameof(ProofsViewModel.CurrentProof))
                    UpdateProofDisplay();
                else if (e.PropertyName == nameof(ProofsViewModel.ProofVerificationResult))
                    UpdateVerifyResult();
            });
        };
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await _vm.LoadMerkleRootCommand.ExecuteAsync(null);
    }

    private async void RefreshRoot_Click(object sender, RoutedEventArgs e)
    {
        await _vm.LoadMerkleRootCommand.ExecuteAsync(null);
    }

    private async void Lookup_Click(object sender, RoutedEventArgs e)
    {
        _vm.LookupId = LookupIdBox.Text?.Trim();
        await _vm.LookupProofCommand.ExecuteAsync(null);
    }

    private async void Verify_Click(object sender, RoutedEventArgs e)
    {
        await _vm.VerifyCurrentProofCommand.ExecuteAsync(null);
    }

    private void UpdateProofDisplay()
    {
        var proof = _vm.CurrentProof;
        if (proof is null)
        {
            ProofPanel.Visibility = Visibility.Collapsed;
            return;
        }

        ProofPanel.Visibility = Visibility.Visible;
        ProofLeafHash.Text = proof.InclusionProof.LeafHash;
        ProofLeafIndex.Text = proof.InclusionProof.LeafIndex.ToString();
        ProofRoot.Text = proof.InclusionProof.Root;

        var siblings = proof.InclusionProof.Siblings;
        ProofSiblings.Text = siblings.Count == 0
            ? "(none)"
            : string.Join("\n", siblings.Select(s => $"{s.Direction}: {s.Hash}"));

        VerifyResultText.Text = "";
    }

    private void UpdateVerifyResult()
    {
        if (_vm.ProofVerificationResult is true)
        {
            VerifyResultText.Text = "VALID";
            VerifyResultText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                Microsoft.UI.Colors.Green);
        }
        else if (_vm.ProofVerificationResult is false)
        {
            VerifyResultText.Text = "INVALID";
            VerifyResultText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                Microsoft.UI.Colors.Red);
        }
        else
        {
            VerifyResultText.Text = "";
        }
    }
}
