using PortfolioCite.Application.Abstractions;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Content;

public class ReferenceManagementService : IReferenceManagementService
{
    private readonly IPortfolioRepository _repository;

    public ReferenceManagementService(IPortfolioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CertificateAdminDto>> GetCertificatesAsync(CancellationToken cancellationToken)
    {
        var certificates = await _repository.GetCertificatesAsync(cancellationToken);

        return certificates.Select(Map).ToList();
    }

    public async Task<CertificateAdminDto> CreateCertificateAsync(SaveCertificateRequest request, CancellationToken cancellationToken)
    {
        var certificate = new Certificate();
        Apply(certificate, request);

        await _repository.AddAsync(certificate, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(certificate);
    }

    public async Task<CertificateAdminDto?> UpdateCertificateAsync(int id, SaveCertificateRequest request, CancellationToken cancellationToken)
    {
        var certificate = await _repository.GetForUpdateAsync<Certificate>(id, cancellationToken);
        if (certificate is null) return null;

        Apply(certificate, request);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(certificate);
    }

    public async Task<bool> DeleteCertificateAsync(int id, CancellationToken cancellationToken)
    {
        var certificate = await _repository.GetForUpdateAsync<Certificate>(id, cancellationToken);
        if (certificate is null) return false;

        _repository.Remove(certificate);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<ContactLinkAdminDto>> GetContactLinksAsync(CancellationToken cancellationToken)
    {
        var links = await _repository.GetContactLinksAsync(cancellationToken);

        return links.Select(Map).ToList();
    }

    public async Task<ContactLinkAdminDto> CreateContactLinkAsync(SaveContactLinkRequest request, CancellationToken cancellationToken)
    {
        var link = new ContactLink();
        Apply(link, request);

        await _repository.AddAsync(link, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(link);
    }

    public async Task<ContactLinkAdminDto?> UpdateContactLinkAsync(int id, SaveContactLinkRequest request, CancellationToken cancellationToken)
    {
        var link = await _repository.GetForUpdateAsync<ContactLink>(id, cancellationToken);
        if (link is null) return null;

        Apply(link, request);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(link);
    }

    public async Task<bool> DeleteContactLinkAsync(int id, CancellationToken cancellationToken)
    {
        var link = await _repository.GetForUpdateAsync<ContactLink>(id, cancellationToken);
        if (link is null) return false;

        _repository.Remove(link);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void Apply(Certificate certificate, SaveCertificateRequest request)
    {
        certificate.Name = request.Name.Trim();
        certificate.Issuer = request.Issuer.Trim();
        certificate.IssuedOn = request.IssuedOn;
        certificate.CredentialUrl = request.CredentialUrl.Trim();
    }

    private static void Apply(ContactLink link, SaveContactLinkRequest request)
    {
        link.Label = request.Label.Trim();
        link.Url = request.Url.Trim();
        link.IconName = request.IconName.Trim();
        link.DisplayOrder = request.DisplayOrder;
    }

    private static CertificateAdminDto Map(Certificate certificate)
    {
        return new CertificateAdminDto(certificate.Id, certificate.Name, certificate.Issuer, certificate.IssuedOn, certificate.CredentialUrl);
    }

    private static ContactLinkAdminDto Map(ContactLink link)
    {
        return new ContactLinkAdminDto(link.Id, link.Label, link.Url, link.IconName, link.DisplayOrder);
    }
}
