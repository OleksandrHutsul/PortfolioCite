using System.Globalization;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Application.Models;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Contact;

public class ContactOwnerNotification : IContactNotification
{
    private readonly IPortfolioRepository _repository;
    private readonly IEmailService _email;
    private readonly ILogger<ContactOwnerNotification> _logger;

    public ContactOwnerNotification(IPortfolioRepository repository, IEmailService email, ILogger<ContactOwnerNotification> logger)
    {
        _repository = repository;
        _email = email;
        _logger = logger;
    }

    public async Task NotifyOwnerAsync(ContactSubmission submission, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetProfileAsync(cancellationToken);
        var recipient = profile?.Email.Trim();

        if (!MailAddress.TryCreate(recipient, out var owner))
        {
            _logger.LogWarning("Contact message {SubmissionId} was stored, but the profile has no notification email.", submission.Id);
            return;
        }

        var replyTo = MailAddress.TryCreate(submission.Email, out var visitor) ? visitor.Address : null;
        var received = submission.CreatedAt.ToUniversalTime().ToString("dd MMM yyyy, HH:mm 'UTC'", CultureInfo.InvariantCulture);
        var body = $"""
            <div style="font-family:Segoe UI,Arial,sans-serif;color:#1a2332;line-height:1.5;max-width:560px;">
              <h1 style="font-size:18px;font-weight:600;margin:0 0 16px;">New contact message</h1>
              <p style="margin:0 0 8px;"><strong>Name:</strong> {Encode(submission.Name)}</p>
              <p style="margin:0 0 8px;"><strong>Email:</strong> {Encode(submission.Email)}</p>
              <p style="margin:0 0 8px;"><strong>Received:</strong> {Encode(received)}</p>
              <p style="margin:0 0 8px;"><strong>Subject:</strong> {Encode(submission.Subject)}</p>
              <p style="margin:16px 0 8px;"><strong>Message:</strong></p>
              <div style="white-space:pre-wrap;">{Encode(submission.Message)}</div>
            </div>
            """;

        await _email.SendAsync(new EmailMessage(owner.Address, $"New contact message: {OneLine(submission.Subject)}", body, replyTo), cancellationToken);
    }

    private static string OneLine(string value)
    {
        return string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private static string Encode(string value)
    {
        return WebUtility.HtmlEncode(value);
    }
}
