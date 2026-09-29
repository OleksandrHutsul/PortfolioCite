using Microsoft.AspNetCore.Components;
using PortfolioCite.App.Services;
using PortfolioCite.Contracts.Authentication;

namespace PortfolioCite.App.Components.Pages.Admin.Login;

public partial class AdminLoginPage : ComponentBase
{
    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }

    [SupplyParameterFromQuery] public string? ReturnUrl { get; set; }

    protected LoginRequest Request { get; } = new();
    protected bool IsSubmitting { get; private set; }
    protected string? Error { get; private set; }

    protected async Task LoginAsync()
    {
        if (IsSubmitting) return;

        IsSubmitting = true;
        Error = null;

        var result = await AdminApi.LoginAsync(Request);

        IsSubmitting = false;

        if (!result.IsSuccess)
        {
            Error = result.Error;
            return;
        }

        var destination = !string.IsNullOrWhiteSpace(ReturnUrl) && ReturnUrl.StartsWith("/admin", StringComparison.OrdinalIgnoreCase) ? ReturnUrl : "/admin";

        Navigation.NavigateTo(destination, replace: true);
    }
}
