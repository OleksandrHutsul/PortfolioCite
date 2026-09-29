using Microsoft.AspNetCore.Components;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Contact;
using PortfolioCite.Contracts.Portfolio;

namespace PortfolioCite.App.Components.Pages.Contact;

public partial class ContactPage : ComponentBase
{
    [Inject] public required PortfolioContentService PortfolioContentService { get; set; }

    protected string Email { get; private set; } = string.Empty;
    protected IReadOnlyList<ContactLinkDto> Links { get; private set; } = [];
    protected CreateContactSubmissionRequest Submission { get; private set; } = new();
    protected bool IsLoading { get; private set; }
    protected bool IsSubmitting { get; private set; }
    protected bool Submitted { get; private set; }
    protected string? LoadError { get; private set; }
    protected string? SubmitError { get; private set; }

    protected override Task OnInitializedAsync()
    {
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        IsLoading = true;
        LoadError = null;

        try
        {
            var snapshot = await PortfolioContentService.GetSnapshotAsync();

            Email = snapshot.Profile.Email;
            Links = snapshot.ContactLinks;
        }
        catch (Exception)
        {
            LoadError = "Contact details are temporarily unavailable.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task SubmitAsync()
    {
        if (IsSubmitting) return;

        IsSubmitting = true;
        SubmitError = null;

        try
        {
            await PortfolioContentService.SubmitContactAsync(Submission);
            Submitted = true;
        }
        catch (HttpRequestException)
        {
            SubmitError = "Your message could not be sent. Please try again shortly.";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    protected void ResetForm()
    {
        Submission = new CreateContactSubmissionRequest();
        Submitted = false;
        SubmitError = null;
    }
}
