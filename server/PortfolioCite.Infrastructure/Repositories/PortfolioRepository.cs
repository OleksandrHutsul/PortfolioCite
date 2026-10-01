using Microsoft.EntityFrameworkCore;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Domain.Entities;
using PortfolioCite.Infrastructure.Data;

namespace PortfolioCite.Infrastructure.Repositories;

public partial class PortfolioRepository : IPortfolioRepository
{
    private readonly PortfolioDbContext _dbContext;

    public PortfolioRepository(PortfolioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<T?> GetForUpdateAsync<T>(int id, CancellationToken cancellationToken) where T : class
    {
        return _dbContext.Set<T>().FindAsync([id], cancellationToken).AsTask();
    }

    public Task<List<T>> ListForUpdateAsync<T>(CancellationToken cancellationToken) where T : class
    {
        return _dbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public Task AddAsync<T>(T entity, CancellationToken cancellationToken) where T : class
    {
        return _dbContext.Set<T>().AddAsync(entity, cancellationToken).AsTask();
    }

    public void Remove<T>(T entity) where T : class
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public Task<Administrator?> GetAdministratorByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return _dbContext.Administrators.FirstOrDefaultAsync(administrator => administrator.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public Task<bool> HasAdministratorAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Administrators.AnyAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
