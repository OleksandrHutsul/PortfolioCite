using PortfolioCite.Application.Models;

namespace PortfolioCite.Application.Abstractions;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken);
}
