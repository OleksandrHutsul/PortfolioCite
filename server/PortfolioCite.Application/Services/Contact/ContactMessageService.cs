using PortfolioCite.Application.Abstractions;
using PortfolioCite.Contracts.Contact;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Contact;

public class ContactMessageService : IContactMessageService
{
    private readonly IPortfolioRepository _repository;

    public ContactMessageService(IPortfolioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ContactSubmissionDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var messages = await _repository.GetContactSubmissionsAsync(cancellationToken);

        return messages.Select(Map).ToList();
    }

    public async Task<ContactSubmissionDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var message = await _repository.GetContactSubmissionAsync(id, cancellationToken);

        return message is null ? null : Map(message);
    }

    public async Task<ContactSubmissionDto?> SetReadAsync(Guid id, bool isRead, CancellationToken cancellationToken)
    {
        var message = await _repository.GetContactSubmissionAsync(id, cancellationToken);
        if (message is null) return null;

        message.IsRead = isRead;

        await _repository.SaveChangesAsync(cancellationToken);

        return Map(message);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var message = await _repository.GetContactSubmissionAsync(id, cancellationToken);
        if (message is null) return false;

        _repository.Remove(message);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static ContactSubmissionDto Map(ContactSubmission message)
    {
        return new ContactSubmissionDto(message.Id, message.Name, message.Email, message.Subject, message.Message, message.CreatedAt, message.IsRead);
    }
}
