using Microsoft.Extensions.Logging;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Contracts.Contact;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Contact;

public class ContactService : IContactService
{
    private readonly IPortfolioRepository _repository;
    private readonly IContactNotification _notification;
    private readonly ILogger<ContactService> _logger;

    public ContactService(IPortfolioRepository repository, IContactNotification notification, ILogger<ContactService> logger)
    {
        _repository = repository;
        _notification = notification;
        _logger = logger;
    }

    public async Task<Guid> SubmitAsync(CreateContactSubmissionRequest request, CancellationToken cancellationToken)
    {
        var submission = new ContactSubmission
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddAsync(submission, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        try
        {
            await _notification.NotifyOwnerAsync(submission, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Contact message {SubmissionId} was stored, but the owner notification failed.", submission.Id);
        }

        return submission.Id;
    }
}