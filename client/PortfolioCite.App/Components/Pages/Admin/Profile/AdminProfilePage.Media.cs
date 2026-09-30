using Microsoft.AspNetCore.Components.Forms;
using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.App.Components.Pages.Admin.Profile;

public partial class AdminProfilePage
{
    protected async Task SelectAvatarAsync(InputFileChangeEventArgs args)
    {
        if (IsSaving) return;

        AvatarInputVersion++;

        var result = await ReadFileAsync(args.File, true);

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

        var result = await ReadFileAsync(args.File, false);

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
                problems.Add("the résumé could not be uploaded");
        }
        else if (ResumeRemovePending && HasStoredResume)
        {
            if (!await RemoveStoredResumeAsync())
                problems.Add("the résumé could not be removed");
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
            AvatarMessage = FriendlyMediaError(result.Error, "The image could not be saved. Try again.");
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
            AvatarMessage = FriendlyMediaError(result.Error, "The image could not be removed. Try again.");
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
            ResumeMessage = FriendlyMediaError(result.Error, "The résumé could not be saved. Try again.");
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
            ResumeMessage = FriendlyMediaError(result.Error, "The résumé could not be removed. Try again.");
            return false;
        }

        ResumeUrl = BlankToNull(result.Value.ResumeUrl);
        ResumeRemovePending = false;

        return true;
    }

    private async Task<(PendingUpload? File, string? Error)> ReadFileAsync(IBrowserFile file, bool avatar)
    {
        var extension = ProfileMediaRules.ExtensionOf(file.Name);
        var contentType = ProfileMediaRules.ContentTypeFor(extension);
        var typeIsValid = avatar ? ProfileMediaRules.IsAvatarExtension(extension) : ProfileMediaRules.IsResumeExtension(extension);
        var maxBytes = avatar ? ProfileMediaRules.AvatarMaxBytes : ProfileMediaRules.ResumeMaxBytes;

        if (contentType is null || !typeIsValid)
            return (null, avatar ? ProfileMediaRules.AvatarTypeError : ProfileMediaRules.ResumeTypeError);

        if (file.Size <= 0)
            return (null, ProfileMediaRules.EmptyFileError);

        if (file.Size > maxBytes)
            return (null, avatar ? ProfileMediaRules.AvatarSizeError : ProfileMediaRules.ResumeSizeError);

        try
        {
            await using var stream = file.OpenReadStream(maxBytes);
            using var memory = new MemoryStream();

            await stream.CopyToAsync(memory, _lifetime.Token);

            var name = Path.GetFileName(file.Name);
            var pending = new PendingUpload(memory.ToArray(), string.IsNullOrWhiteSpace(name) ? "upload" : name, contentType);

            return (pending, null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return (null, avatar ? "The image could not be read." : "The résumé could not be read.");
        }
    }

    private static string FriendlyMediaError(string? error, string fallback)
    {
        if (string.IsNullOrWhiteSpace(error))
            return fallback;

        if (error.Contains("Exception", StringComparison.OrdinalIgnoreCase) || error.Contains("HTTP", StringComparison.OrdinalIgnoreCase)
            || error.Any(char.IsDigit) && error.Contains("status", StringComparison.OrdinalIgnoreCase))
            return fallback;

        return error;
    }

    private static string? FileNameFromUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var path = Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.AbsolutePath : url;
        var name = Path.GetFileName(Uri.UnescapeDataString(path));

        return string.IsNullOrWhiteSpace(name) ? null : name;
    }

    private static string FormatSize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";

        if (bytes < 1024 * 1024)
            return $"{bytes / 1024d:0.#} KB";

        return $"{bytes / (1024d * 1024d):0.#} MB";
    }
}
