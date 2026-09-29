using PortfolioCite.Contracts.Administration;

namespace PortfolioCite.Application.Services.Content;

public interface IReferenceManagementService
{
    Task<IReadOnlyList<CertificateAdminDto>> GetCertificatesAsync(CancellationToken cancellationToken);
    Task<CertificateAdminDto> CreateCertificateAsync(SaveCertificateRequest request, CancellationToken cancellationToken);
    Task<CertificateAdminDto?> UpdateCertificateAsync(int id, SaveCertificateRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteCertificateAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ContactLinkAdminDto>> GetContactLinksAsync(CancellationToken cancellationToken);
    Task<ContactLinkAdminDto> CreateContactLinkAsync(SaveContactLinkRequest request, CancellationToken cancellationToken);
    Task<ContactLinkAdminDto?> UpdateContactLinkAsync(int id, SaveContactLinkRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteContactLinkAsync(int id, CancellationToken cancellationToken);
}
