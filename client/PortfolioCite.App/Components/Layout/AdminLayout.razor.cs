using Microsoft.AspNetCore.Components;
using PortfolioCite.App.Services;

namespace PortfolioCite.App.Components.Layout;

public partial class AdminLayout
{
    [Inject] public required AdminApiService AdminApi { get; set; }
    [Inject] public required NavigationManager Navigation { get; set; }

    private async Task LogoutAsync()
    {
        await AdminApi.LogoutAsync();
        Navigation.NavigateTo("/admin/login", replace: true);
    }
}
