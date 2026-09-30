using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.App.Services;

public partial class AdminApiService
{
    public Task<ApiResult<List<ContactLinkAdminDto>>> GetContactLinksAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<ContactLinkAdminDto>>("api/admin/contact-links", cancellationToken);
    }

    public Task<ApiResult<ContactLinkAdminDto>> CreateContactLinkAsync(SaveContactLinkRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ContactLinkAdminDto>(HttpMethod.Post, "api/admin/contact-links", request, cancellationToken);
    }

    public Task<ApiResult<ContactLinkAdminDto>> UpdateContactLinkAsync(int id, SaveContactLinkRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ContactLinkAdminDto>(HttpMethod.Put, $"api/admin/contact-links/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteContactLinkAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/contact-links/{id}", cancellationToken);
    }
}
