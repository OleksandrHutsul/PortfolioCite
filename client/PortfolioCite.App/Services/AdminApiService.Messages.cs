using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Contact;

namespace PortfolioCite.App.Services;

public partial class AdminApiService
{
    public Task<ApiResult<List<ContactSubmissionDto>>> GetMessagesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<ContactSubmissionDto>>("api/admin/messages", cancellationToken);
    }

    public Task<ApiResult<ContactSubmissionDto>> SetMessageReadAsync(Guid id, bool isRead, CancellationToken cancellationToken = default)
    {
        var request = new UpdateContactReadRequest { IsRead = isRead };
        return SendAsync<ContactSubmissionDto>(HttpMethod.Patch, $"api/admin/messages/{id}/read", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/messages/{id}", cancellationToken);
    }
}
