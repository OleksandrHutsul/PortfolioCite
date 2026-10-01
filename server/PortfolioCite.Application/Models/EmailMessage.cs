namespace PortfolioCite.Application.Models;

public record EmailMessage(string To, string Subject, string Body, string? ReplyTo);
