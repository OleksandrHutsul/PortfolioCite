using System.Net.Mail;
using Microsoft.Extensions.Options;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Application.Models;

namespace PortfolioCite.Infrastructure.Email;

public class ResendEmailService : IEmailService
{
    private readonly Resend.IResend _resend;
    private readonly EmailOptions _options;

    public ResendEmailService(Resend.IResend resend, IOptions<EmailOptions> options)
    {
        _resend = resend;
        _options = options.Value;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        if (!MailAddress.TryCreate(_options.FromAddress, out var from))
            throw new InvalidOperationException("Email delivery is not configured.");

        if (!MailAddress.TryCreate(message.To, out var to))
            throw new InvalidOperationException("The notification recipient is not a valid email address.");

        var mail = new Resend.EmailMessage
        {
            From = new Resend.EmailAddress
            {
                Email = from.Address,
                DisplayName = string.IsNullOrWhiteSpace(_options.FromName) ? null : _options.FromName.Trim()
            },
            Subject = message.Subject,
            HtmlBody = message.Body
        };
        mail.To.Add(to.Address);

        if (MailAddress.TryCreate(message.ReplyTo, out var replyTo))
            mail.ReplyTo = replyTo.Address;

        await _resend.EmailSendAsync(mail, cancellationToken);
    }
}
