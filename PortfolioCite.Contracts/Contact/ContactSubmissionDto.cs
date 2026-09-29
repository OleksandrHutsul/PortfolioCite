namespace PortfolioCite.Contracts.Contact;

public record ContactSubmissionDto(Guid Id, string Name, string Email, string Subject, string Message, DateTimeOffset CreatedAt, bool IsRead);
