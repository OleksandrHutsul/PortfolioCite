using PortfolioCite.Application.Abstractions;
using PortfolioCite.Contracts.Contact;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Contact;

public class ContactService : IContactService
{
    private readonly IPortfolioRepository _repository;

    public ContactService(IPortfolioRepository repository)
    {
        _repository = repository;
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

        return submission.Id;
    }
}