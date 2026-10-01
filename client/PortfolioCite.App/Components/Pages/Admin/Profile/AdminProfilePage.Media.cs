using Microsoft.AspNetCore.Components.Forms;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration.Rules;

namespace PortfolioCite.App.Components.Pages.Admin.Profile;

public partial class AdminProfilePage
{
    protected async Task SelectAvatarAsync(InputFileChangeEventArgs args)
    {
        if (IsSaving) return;

        AvatarInputVersion++;

        var result = await MediaFileReader.ReadImageAsync(args.File, _lifetime.Token);

        if (result.File is null)
        {
            AvatarMessage = result.Error;
            return;
        }

        _pendingAvatar = result.File;
        AvatarPreviewUrl = $"data:{_pendingAvatar.ContentType};base64,{Convert.ToBase64String(_pendingAvatar.Content)}";
        AvatarRemovePending = false;
        AvatarMessage = null;
    }

    protected void ClearAvatarSelection()
    {
        if (IsSaving) return;

        _pendingAvatar = null;
        AvatarPreviewUrl = AvatarUrl;
        AvatarMessage = null;
    }

    protected void MarkAvatarForRemoval()
    {
        if (IsSaving || !HasStoredAvatar) return;

        _pendingAvatar = null;
        AvatarPreviewUrl = null;
        AvatarRemovePending = true;
        AvatarMessage = null;
    }

    protected void UndoAvatarRemoval()
    {
        if (IsSaving) return;

        AvatarRemovePending = false;
        AvatarPreviewUrl = AvatarUrl;
        AvatarMessage = null;
    }

    protected async Task SelectResumeAsync(InputFileChangeEventArgs args)
    {
        if (IsSaving) return;

        ResumeInputVersion++;

        var result = await MediaFileReader.ReadResumeAsync(args.File, _lifetime.Token);

        if (result.File is null)
        {
            ResumeMessage = result.Error;
            return;
        }

        _pendingResume = result.File;
        ResumeRemovePending = false;
        ResumeMessage = null;
    }

    protected void ClearResumeSelection()
    {
        if (IsSaving) return;

        _pendingResume = null;
        ResumeMessage = null;
    }

    protected void MarkResumeForRemoval()
    {
        if (IsSaving || !HasStoredResume) return;

        _pendingResume = null;
        ResumeRemovePending = true;
        ResumeMessage = null;
    }

    protected void UndoResumeRemoval()
    {
        if (IsSaving) return;

        ResumeRemovePending = false;
        ResumeMessage = null;
    }

    private async Task<List<string>> SavePendingMediaAsync()
    {
        var problems = new List<string>();

        if (_pendingAvatar is not null)
        {
            if (!await UploadAvatarAsync())
                problems.Add("the avatar could not be uploaded");
        }
        else if (AvatarRemovePending && HasStoredAvatar)
        {
            if (!await RemoveStoredAvatarAsync())
                problems.Add("the avatar could not be removed");
        }

        if (_pendingResume is not null)
        {
            if (!await UploadResumeAsync())
                problems.Add("the resume could not be uploaded");
        }
        else if (ResumeRemovePending && HasStoredResume)
        {
            if (!await RemoveStoredResumeAsync())
                problems.Add("the resume could not be removed");
        }

        return problems;
    }

    private async Task<bool> UploadAvatarAsync()
    {
        var pending = _pendingAvatar!;
        using var stream = new MemoryStream(pending.Content, writable: false);

        var result = await AdminApi.UploadAvatarAsync(stream, pending.Name, pending.ContentType, _lifetime.Token);

        if (!result.IsSuccess || result.Value is null)
        {
            AvatarMessage = MediaFileReader.FriendlyError(result.Error, "The image could not be saved. Try again.");
            return false;
        }

        AvatarUrl = BlankToNull(result.Value.AvatarUrl);
        AvatarPreviewUrl = AvatarUrl;
        _pendingAvatar = null;
        AvatarRemovePending = false;

        return true;
    }

    private async Task<bool> RemoveStoredAvatarAsync()
    {
        var result = await AdminApi.RemoveAvatarAsync(_lifetime.Token);

        if (!result.IsSuccess || result.Value is null)
        {
            AvatarMessage = MediaFileReader.FriendlyError(result.Error, "The image could not be removed. Try again.");
            return false;
        }

        AvatarUrl = BlankToNull(result.Value.AvatarUrl);
        AvatarPreviewUrl = AvatarUrl;
        AvatarRemovePending = false;

        return true;
    }

    private async Task<bool> UploadResumeAsync()
    {
        var pending = _pendingResume!;
        using var stream = new MemoryStream(pending.Content, writable: false);

        var result = await AdminApi.UploadResumeAsync(stream, pending.Name, pending.ContentType, _lifetime.Token);

        if (!result.IsSuccess || result.Value is null)
        {
            ResumeMessage = MediaFileReader.FriendlyError(result.Error, "The resume could not be saved. Try again.");
            return false;
        }

        ResumeUrl = BlankToNull(result.Value.ResumeUrl);
        _pendingResume = null;
        ResumeRemovePending = false;

        return true;
    }

    private async Task<bool> RemoveStoredResumeAsync()
    {
        var result = await AdminApi.RemoveResumeAsync(_lifetime.Token);

        if (!result.IsSuccess || result.Value is null)
        {
            ResumeMessage = MediaFileReader.FriendlyError(result.Error, "The resume could not be removed. Try again.");
            return false;
        }

        ResumeUrl = BlankToNull(result.Value.ResumeUrl);
        ResumeRemovePending = false;

        return true;
    }

    private static string? FileNameFromUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var path = Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.AbsolutePath : url;
        var name = Path.GetFileName(Uri.UnescapeDataString(path));

        return string.IsNullOrWhiteSpace(name) ? null : name;
    }
}
