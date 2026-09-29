using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Contact;

namespace PortfolioCite.App.Components.Pages.Admin.Messages;

public partial class AdminMessagesPage : ComponentBase
{
    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required IJSRuntime JavaScript { get; set; }

    protected List<ContactSubmissionDto> Messages { get; private set; } = [];
    protected ContactSubmissionDto? Selected { get; private set; }
    protected bool IsLoading { get; private set; }
    protected string? Error { get; private set; }
    protected int UnreadCount => Messages.Count(message => !message.IsRead);

    protected override Task OnInitializedAsync()
    {
        return LoadAsync();
    }

    protected async Task LoadAsync()
    {
        IsLoading = true;
        Error = null;

        var result = await AdminApi.GetMessagesAsync();

        Messages = result.Value ?? [];
        Selected = null;
        Error = result.Error;
        IsLoading = false;
    }

    protected async Task OpenAsync(ContactSubmissionDto message)
    {
        Selected = message;

        if (!message.IsRead)
            await SetReadAsync(true);
    }

    protected Task ToggleReadAsync()
    {
        return Selected is null
            ? Task.CompletedTask
            : SetReadAsync(!Selected.IsRead);
    }

    protected async Task DeleteAsync()
    {
        if (Selected is null) return;

        var confirmed = await JavaScript.InvokeAsync<bool>("confirm", "Delete this message permanently?");
        if (!confirmed) return;

        var result = await AdminApi.DeleteMessageAsync(Selected.Id);

        if (!result.IsSuccess)
        {
            Error = result.Error;
            return;
        }

        Messages.RemoveAll(message => message.Id == Selected.Id);
        Selected = null;
    }

    private async Task SetReadAsync(bool isRead)
    {
        if (Selected is null) return;

        var result = await AdminApi.SetMessageReadAsync(Selected.Id, isRead);

        if (result.Value is null)
        {
            Error = result.Error;
            return;
        }

        var index = Messages.FindIndex(message => message.Id == result.Value.Id);

        if (index >= 0)
            Messages[index] = result.Value;

        Selected = result.Value;
    }
}
