using Microsoft.AspNetCore.Components;

namespace PortfolioCite.App.Components.Shared;

public partial class RedirectToLoginComponent : ComponentBase
{
    [Inject] public required NavigationManager Navigation { get; set; }

    protected override void OnInitialized()
    {
        var relativeUrl = Navigation.ToBaseRelativePath(Navigation.Uri);
        var returnUrl = $"/{relativeUrl}";

        Navigation.NavigateTo($"/admin/login?returnUrl={Uri.EscapeDataString(returnUrl)}", replace: true);
    }
}