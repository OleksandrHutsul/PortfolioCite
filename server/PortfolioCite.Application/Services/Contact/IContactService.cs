using PortfolioCite.Contracts.Contact;

namespace PortfolioCite.Application.Services.Contact;

public interface IContactService
{
    Task<Guid> SubmitAsync(CreateContactSubmissionRequest request, CancellationToken cancellationToken);
}
