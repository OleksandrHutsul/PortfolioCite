using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Contracts.Portfolio.Models;
using PortfolioCite.Contracts.Portfolio.Rules;

namespace PortfolioCite.App.Components.Pages.Admin.Profile;

public partial class AdminProfilePage : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _lifetime = new();
    private bool _disposed;
    private PendingUpload? _pendingAvatar;
    private PendingUpload? _pendingResume;

    [Inject] public required AdminApiService AdminApi { get; set; }

    protected SaveProfileRequest Request { get; private set; } = new();
    protected EditContext ProfileEditContext { get; private set; } = null!;
    protected bool IsLoading { get; private set; }
    protected bool IsSaving { get; private set; }
    protected bool HasProfile { get; private set; }
    protected bool SaveSucceeded { get; private set; }
    protected string? LoadError { get; private set; }
    protected string? SaveMessage { get; private set; }
    protected string? AvatarUrl { get; private set; }
    protected string? AvatarPreviewUrl { get; private set; }
    protected string? ResumeUrl { get; private set; }
    protected bool AvatarRemovePending { get; private set; }
    protected bool ResumeRemovePending { get; private set; }
    protected string? AvatarMessage { get; private set; }
    protected string? ResumeMessage { get; private set; }
    protected int AvatarInputVersion { get; private set; }
    protected int ResumeInputVersion { get; private set; }

    protected bool HasStoredAvatar => !string.IsNullOrWhiteSpace(AvatarUrl);
    protected bool HasStoredResume => !string.IsNullOrWhiteSpace(ResumeUrl);
    protected bool HasPendingAvatar => _pendingAvatar is not null;
    protected bool HasPendingResume => _pendingResume is not null;
    protected bool ShowAvatarImage => !AvatarRemovePending && !string.IsNullOrWhiteSpace(AvatarPreviewUrl);
    protected bool CanViewResume => HasStoredResume && !ResumeRemovePending && !HasPendingResume;
    protected bool CanAddLanguage => !IsSaving && Request.Languages.Count < LanguageProficiencies.MaxCount;
    protected string AvatarHint => $"JPG, PNG or WebP · Max {ProfileMediaRules.AvatarMaxBytes / (1024 * 1024)} MB";
    protected string ResumeHint => $"PDF only · Max {ProfileMediaRules.ResumeMaxBytes / (1024 * 1024)} MB";
    protected string AvatarActionLabel => HasPendingAvatar ? "Change image" : HasStoredAvatar && !AvatarRemovePending ? "Replace image" : "Choose image";
    protected string ResumeActionLabel => HasPendingResume ? "Change PDF" : HasStoredResume && !ResumeRemovePending ? "Replace" : "Choose PDF";
    protected string AvatarStatus => AvatarRemovePending ? "Will be removed when you save." : HasPendingAvatar ? _pendingAvatar!.Name
        : HasStoredAvatar ? "Current profile image" : "No profile image uploaded";
    protected string ResumeStatus => ResumeRemovePending ? "Will be removed when you save." : HasPendingResume ? _pendingResume!.Name
        : HasStoredResume ? FileNameFromUrl(ResumeUrl) ?? "Résumé uploaded" : "No résumé uploaded";
    protected string? AvatarDetail => HasPendingAvatar ? FormatSize(_pendingAvatar!.Content.Length) : null;
    protected string? ResumeDetail => HasPendingResume ? FormatSize(_pendingResume!.Content.Length) : null;
    protected string AvatarAlt => HasPendingAvatar ? $"Preview of {_pendingAvatar!.Name}" : "Current profile image";
    protected string Description => !IsLoading && LoadError is null && !HasProfile
        ? "No profile exists yet. Enter the details below to create it."
        : "Edit the identity and introduction shown across the portfolio.";

    protected override void OnInitialized()
    {
        UseRequest(Request);
    }

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
            var result = await AdminApi.GetProfileAsync(_lifetime.Token);

            if (!result.IsSuccess)
            {
                HasProfile = false;
                LoadError = result.Error;
                return;
            }

            if (result.Value is null)
            {
                UseRequest(new SaveProfileRequest());
                ClearStoredMedia();
                HasProfile = false;
                return;
            }

            ApplyProfile(result.Value);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task SaveAsync()
    {
        if (IsSaving) return;

        IsSaving = true;
        SaveMessage = null;
        AvatarMessage = null;
        ResumeMessage = null;
        SaveSucceeded = false;

        try
        {
            var result = await AdminApi.UpdateProfileAsync(Request, _lifetime.Token);

            if (!result.IsSuccess || result.Value is null)
            {
                SaveMessage = string.IsNullOrWhiteSpace(result.Error) ? "The profile could not be saved." : result.Error;
                return;
            }

            UseRequest(ToRequest(result.Value));
            HasProfile = true;
            AvatarUrl = BlankToNull(result.Value.AvatarUrl);
            ResumeUrl = BlankToNull(result.Value.ResumeUrl);

            var problems = await SavePendingMediaAsync();

            SaveSucceeded = problems.Count == 0;
            SaveMessage = SaveSucceeded
                ? "Profile saved."
                : $"The profile was saved, but {string.Join(" and ", problems)}.";
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsSaving = false;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _lifetime.Cancel();
        _lifetime.Dispose();
    }

    private void ApplyProfile(ProfileAdminDto profile)
    {
        UseRequest(ToRequest(profile));
        ClearStoredMedia();

        HasProfile = true;
        AvatarUrl = BlankToNull(profile.AvatarUrl);
        AvatarPreviewUrl = AvatarUrl;
        ResumeUrl = BlankToNull(profile.ResumeUrl);
    }

    private void ClearStoredMedia()
    {
        AvatarUrl = null;
        AvatarPreviewUrl = null;
        ResumeUrl = null;
        _pendingAvatar = null;
        _pendingResume = null;
        AvatarRemovePending = false;
        ResumeRemovePending = false;
        AvatarMessage = null;
        ResumeMessage = null;
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
            Languages = profile.Languages
                .Select(language => new SaveProfileLanguageRequest
                {
                    Name = language.Name,
                    Proficiency = language.Proficiency,
                    DisplayOrder = language.DisplayOrder
                })
                .ToList(),
            Email = profile.Email
        };
    }

    private static string? BlankToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
