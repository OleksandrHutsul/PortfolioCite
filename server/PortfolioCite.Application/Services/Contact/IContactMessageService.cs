using PortfolioCite.Contracts.Contact;

namespace PortfolioCite.Application.Services.Contact;

public interface IContactMessageService
{
    Task<IReadOnlyList<ContactSubmissionDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ContactSubmissionDto?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<ContactSubmissionDto?> SetReadAsync(Guid id, bool isRead, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
