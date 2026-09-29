using Microsoft.AspNetCore.Components;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration;

namespace PortfolioCite.App.Components.Pages.Admin.Profile;

public partial class AdminProfilePage : ComponentBase
{
    [Inject] public required AdminApiService AdminApi { get; set; }

    protected SaveProfileRequest Request { get; private set; } = new();
    protected bool IsLoading { get; private set; }
    protected bool IsSaving { get; private set; }
    protected bool SaveSucceeded { get; private set; }
    protected string? LoadError { get; private set; }
    protected string? SaveMessage { get; private set; }

    protected override Task OnInitializedAsync()
    {
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        IsLoading = true;
        LoadError = null;

        var result = await AdminApi.GetProfileAsync();

        if (result.Value is not null)
            Request = ToRequest(result.Value);

        LoadError = result.Error;
        IsLoading = false;
    }

    protected async Task SaveAsync()
    {
        if (IsSaving) return;

        IsSaving = true;
        SaveMessage = null;

        var result = await AdminApi.UpdateProfileAsync(Request);

        IsSaving = false;
        SaveSucceeded = result.IsSuccess;
        SaveMessage = result.IsSuccess ? "Profile saved." : result.Error;
    }

    private static SaveProfileRequest ToRequest(ProfileAdminDto profile)
    {
        return new SaveProfileRequest
        {
            FullName = profile.FullName,
            Role = profile.Role,
            Location = profile.Location,
            Summary = profile.Summary,
            CurrentFocus = profile.CurrentFocus,
            Languages = profile.Languages,
            Email = profile.Email,
            AvatarUrl = profile.AvatarUrl,
            ResumeUrl = profile.ResumeUrl
        };
    }
}
