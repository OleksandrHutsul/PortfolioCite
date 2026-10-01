using Microsoft.AspNetCore.Components.Forms;
using PortfolioCite.App.Services;

namespace PortfolioCite.App.Components.Pages.Admin.Projects;

public partial class AdminProjectEditorPage
{
    protected async Task SelectImageAsync(InputFileChangeEventArgs args)
    {
        if (IsSaving) return;

        ImageInputVersion++;

        var result = await MediaFileReader.ReadImageAsync(args.File, _lifetime.Token);

        if (result.File is null)
        {
            ImageMessage = result.Error;
            return;
        }

        _pendingImage = result.File;
        ImagePreviewUrl = $"data:{_pendingImage.ContentType};base64,{Convert.ToBase64String(_pendingImage.Content)}";
        ImageRemovePending = false;
        ImageMessage = null;
    }

    protected void ClearImageSelection()
    {
        if (IsSaving) return;

        _pendingImage = null;
        ImagePreviewUrl = ImageUrl;
        ImageMessage = null;
    }

    protected void MarkImageForRemoval()
    {
        if (IsSaving || !HasStoredImage) return;

        _pendingImage = null;
        ImagePreviewUrl = null;
        ImageRemovePending = true;
        ImageMessage = null;
    }

    protected void UndoImageRemoval()
    {
        if (IsSaving) return;

        ImageRemovePending = false;
        ImagePreviewUrl = ImageUrl;
        ImageMessage = null;
    }

    private async Task<bool> UploadImageAsync(int projectId)
    {
        var pending = _pendingImage!;
        using var stream = new MemoryStream(pending.Content, writable: false);
        var result = await AdminApi.UploadProjectImageAsync(projectId, stream, pending.Name, pending.ContentType, _lifetime.Token);

        if (!result.IsSuccess || result.Value is null)
        {
            ImageMessage = MediaFileReader.FriendlyError(result.Error, "The image could not be saved. Try again.");
            return false;
        }

        ApplyStoredImage(result.Value.ImageUrl);
        return true;
    }

    private async Task<bool> RemoveImageAsync(int projectId)
    {
        var result = await AdminApi.RemoveProjectImageAsync(projectId, _lifetime.Token);

        if (!result.IsSuccess || result.Value is null)
        {
            ImageMessage = MediaFileReader.FriendlyError(result.Error, "The image could not be removed. Try again.");
            return false;
        }

        ApplyStoredImage(result.Value.ImageUrl);
        return true;
    }

    private async Task RollBackCreatedProjectAsync(int projectId)
    {
        var deleted = await AdminApi.DeleteProjectAsync(projectId, _lifetime.Token);

        if (deleted.IsSuccess)
            return;

        _persistedId = projectId;
        SaveError = "The project was created, but the image could not be saved. Save again to retry the image.";
    }

    private void ResetImageState()
    {
        _persistedId = null;
        _pendingImage = null;
        ImageUrl = null;
        ImagePreviewUrl = null;
        ImageRemovePending = false;
        ImageMessage = null;
        ImageInputVersion++;
    }

    private void ApplyStoredImage(string? imageUrl)
    {
        _pendingImage = null;
        ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl;
        ImagePreviewUrl = ImageUrl;
        ImageRemovePending = false;
        ImageMessage = null;
    }
}
