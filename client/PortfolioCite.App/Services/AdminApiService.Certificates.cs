using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Administration;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.App.Services;

public partial class AdminApiService
{
    public Task<ApiResult<List<CertificateAdminDto>>> GetCertificatesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<CertificateAdminDto>>("api/admin/certificates", cancellationToken);
    }

    public Task<ApiResult<CertificateAdminDto>> CreateCertificateAsync(SaveCertificateRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<CertificateAdminDto>(HttpMethod.Post, "api/admin/certificates", request, cancellationToken);
    }

    public Task<ApiResult<CertificateAdminDto>> UpdateCertificateAsync(int id, SaveCertificateRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<CertificateAdminDto>(HttpMethod.Put, $"api/admin/certificates/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteCertificateAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/certificates/{id}", cancellationToken);
    }
}
