using Microsoft.EntityFrameworkCore;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Repositories;

public partial class PortfolioRepository
{
    public async Task<IReadOnlyList<ContactSubmission>> GetContactSubmissionsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ContactSubmissions
            .AsNoTracking()
            .OrderBy(submission => submission.IsRead)
            .ThenByDescending(submission => submission.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<ContactSubmission?> GetContactSubmissionAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.ContactSubmissions.FirstOrDefaultAsync(submission => submission.Id == id, cancellationToken);
    }
}
