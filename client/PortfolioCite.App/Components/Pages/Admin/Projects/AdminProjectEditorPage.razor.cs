using Microsoft.AspNetCore.Components;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Contracts.Portfolio.Models;
using PortfolioCite.Contracts.Projects.Models;

namespace PortfolioCite.App.Components.Pages.Admin.Projects;

public partial class AdminProjectEditorPage : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _lifetime = new();
    private bool _disposed;
    private bool _hasLoadedForCurrentId;
    private int? _loadedForId;
    private int? _persistedId;
    private PendingUpload? _pendingImage;

    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }

    [Parameter] public int? Id { get; set; }

    protected SaveProjectRequest Request { get; private set; } = new();
    protected string TechnologyNames { get; set; } = string.Empty;
    protected bool IsLoading { get; private set; }
    protected bool IsSaving { get; private set; }
    protected string? LoadError { get; private set; }
    protected string? SaveError { get; private set; }
    protected int OrderItemCount { get; private set; } = 1;
    protected string? ImageUrl { get; private set; }
    protected string? ImagePreviewUrl { get; private set; }
    protected bool ImageRemovePending { get; private set; }
    protected string? ImageMessage { get; private set; }
    protected int ImageInputVersion { get; private set; }

    protected bool HasStoredImage => !string.IsNullOrWhiteSpace(ImageUrl);
    protected bool HasPendingImage => _pendingImage is not null;
    protected bool ShowProjectImage => !ImageRemovePending && !string.IsNullOrWhiteSpace(ImagePreviewUrl);
    protected string ImageHint => $"JPG, PNG or WebP · Max {MediaRules.ImageMaxBytes / (1024 * 1024)} MB";
    protected string ImageActionLabel => HasPendingImage ? "Change image" : HasStoredImage && !ImageRemovePending ? "Replace image" : "Choose image";
    protected string ImageStatus => ImageRemovePending ? "Will be removed when you save." : HasPendingImage ? _pendingImage!.Name
        : HasStoredImage ? "Current project image" : "No project image";
    protected string? ImageDetail => HasPendingImage ? MediaFileReader.FormatSize(_pendingImage!.Content.Length) : null;
    protected string ImageAlt => HasPendingImage ? $"Preview of {_pendingImage!.Name}" : "Current project image";

    private int? ProjectId => Id ?? _persistedId;

    protected override Task OnParametersSetAsync()
    {
        if (_hasLoadedForCurrentId && _loadedForId == Id)
            return Task.CompletedTask;

        _loadedForId = Id;
        _hasLoadedForCurrentId = true;
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        LoadError = null;
        SaveError = null;
        IsLoading = true;

        try
        {
            var projects = await AdminApi.GetProjectsAsync(_lifetime.Token);

            if (!projects.IsSuccess)
            {
                LoadError = projects.Error;
                return;
            }

            var existingCount = projects.Value?.Count ?? 0;

            if (Id is null)
            {
                OrderItemCount = DisplayOrderRules.ItemCount(existingCount, isCreating: true);
                Request = new SaveProjectRequest
                {
                    DisplayOrder = DisplayOrderRules.Next(existingCount)
                };
                TechnologyNames = string.Empty;
                ResetImageState();
                return;
            }

            var result = await AdminApi.GetProjectAsync(Id.Value, _lifetime.Token);

            if (result.Value is null)
            {
                LoadError = result.Error ?? "The project was not found.";
                return;
            }

            OrderItemCount = DisplayOrderRules.ItemCount(existingCount, isCreating: false);
            Request = ToRequest(result.Value);
            TechnologyNames = string.Join(", ", result.Value.Technologies);
            ApplyStoredImage(result.Value.ImageUrl);
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
        SaveError = null;
        ImageMessage = null;
        Request.Technologies = TechnologyNames.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        try
        {
            var projectId = ProjectId;

            var result = projectId is int id
                ? await AdminApi.UpdateProjectAsync(id, Request, _lifetime.Token)
                : await AdminApi.CreateProjectAsync(Request, _lifetime.Token);

            if (!result.IsSuccess || result.Value is null)
            {
                SaveError = string.IsNullOrWhiteSpace(result.Error) ? "The project could not be saved." : result.Error;
                return;
            }

            projectId = result.Value.Id;
            _persistedId = projectId;

            if (_pendingImage is not null)
            {
                if (!await UploadImageAsync(projectId.Value))
                {
                    SaveError = "The project was saved, but the image could not be uploaded. Try saving again.";
                    return;
                }
            }
            else if (ImageRemovePending && HasStoredImage)
            {
                if (!await RemoveImageAsync(projectId.Value))
                {
                    SaveError = "The project was saved, but the image could not be removed. Try saving again.";
                    return;
                }
            }

            Navigation.NavigateTo("/admin/projects");
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

    private static SaveProjectRequest ToRequest(ProjectDto project)
    {
        return new SaveProjectRequest
        {
            Name = project.Name,
            ShortDescription = project.ShortDescription,
            Description = project.Description,
            GitHubUrl = project.GitHubUrl,
            LiveUrl = project.LiveUrl,
            DisplayOrder = project.DisplayOrder,
            IsFeatured = project.IsFeatured,
            IsPublished = project.IsPublished
        };
    }
}
