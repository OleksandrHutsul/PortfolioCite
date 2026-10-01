using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Contact;

public interface IContactNotification
{
    Task NotifyOwnerAsync(ContactSubmission submission, CancellationToken cancellationToken);
}
