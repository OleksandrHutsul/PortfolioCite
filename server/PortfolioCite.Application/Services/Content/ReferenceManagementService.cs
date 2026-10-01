using PortfolioCite.Application.Abstractions;
using PortfolioCite.Application.Validation;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Portfolio.Rules;
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
        var certificates = await _repository.ListForUpdateAsync<Certificate>(cancellationToken);
        var certificate = new Certificate();

        DisplayOrderEditor.Insert(certificates, certificate, request.DisplayOrder, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
        Apply(certificate, request);

        await _repository.AddAsync(certificate, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(certificate);
    }

    public async Task<CertificateAdminDto?> UpdateCertificateAsync(int id, SaveCertificateRequest request, CancellationToken cancellationToken)
    {
        var certificates = await _repository.ListForUpdateAsync<Certificate>(cancellationToken);
        var certificate = certificates.FirstOrDefault(item => item.Id == id);
        if (certificate is null) return null;

        DisplayOrderEditor.Move(certificates, certificate, request.DisplayOrder, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
        Apply(certificate, request);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(certificate);
    }

    public async Task<bool> DeleteCertificateAsync(int id, CancellationToken cancellationToken)
    {
        var certificates = await _repository.ListForUpdateAsync<Certificate>(cancellationToken);
        var certificate = certificates.FirstOrDefault(item => item.Id == id);
        if (certificate is null) return false;

        _repository.Remove(certificate);
        DisplayOrderEditor.CloseGap(certificates, certificate, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
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
        EnsureIcon(request);
        var links = await _repository.ListForUpdateAsync<ContactLink>(cancellationToken);
        var link = new ContactLink();

        DisplayOrderEditor.Insert(links, link, request.DisplayOrder, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
        Apply(link, request);

        await _repository.AddAsync(link, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(link);
    }

    public async Task<ContactLinkAdminDto?> UpdateContactLinkAsync(int id, SaveContactLinkRequest request, CancellationToken cancellationToken)
    {
        EnsureIcon(request);
        var links = await _repository.ListForUpdateAsync<ContactLink>(cancellationToken);
        var link = links.FirstOrDefault(item => item.Id == id);
        if (link is null) return null;

        DisplayOrderEditor.Move(links, link, request.DisplayOrder, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
        Apply(link, request);
        await _repository.SaveChangesAsync(cancellationToken);

        return Map(link);
    }

    public async Task<bool> DeleteContactLinkAsync(int id, CancellationToken cancellationToken)
    {
        var links = await _repository.ListForUpdateAsync<ContactLink>(cancellationToken);
        var link = links.FirstOrDefault(item => item.Id == id);
        if (link is null) return false;

        _repository.Remove(link);
        DisplayOrderEditor.CloseGap(links, link, item => item.DisplayOrder, (item, order) => item.DisplayOrder = order);
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

    private static void EnsureIcon(SaveContactLinkRequest request)
    {
        var validator = new ContentValidator();
        validator.AddSkillIcon(request.IconName, nameof(SaveContactLinkRequest.IconName));
        validator.ThrowIfInvalid();
    }

    private static void Apply(ContactLink link, SaveContactLinkRequest request)
    {
        link.Label = request.Label.Trim();
        link.Url = request.Url.Trim();
        link.IconName = SkillIcons.Canonical(request.IconName) ?? request.IconName.Trim();
    }

    private static CertificateAdminDto Map(Certificate certificate)
    {
        return new CertificateAdminDto(certificate.Id, certificate.Name, certificate.Issuer, certificate.IssuedOn, certificate.CredentialUrl, certificate.DisplayOrder);
    }

    private static ContactLinkAdminDto Map(ContactLink link)
    {
        return new ContactLinkAdminDto(link.Id, link.Label, link.Url, link.IconName, link.DisplayOrder);
    }
}
