using Microsoft.EntityFrameworkCore;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Repositories;

public partial class PortfolioRepository
{
    public Task<Profile?> GetProfileAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Profiles
            .AsNoTracking()
            .Include(profile => profile.Languages)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Profile?> GetProfileForUpdateAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Profiles
            .Include(profile => profile.Languages)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProfileFile>> GetProfileFileSummariesAsync(int profileId, CancellationToken cancellationToken)
    {
        return await _dbContext.ProfileFiles
            .AsNoTracking()
            .Where(file => file.ProfileId == profileId)
            .Select(file => new ProfileFile
            {
                Id = file.Id,
                ProfileId = file.ProfileId,
                Kind = file.Kind,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Size
            })
            .ToListAsync(cancellationToken);
    }

    public Task<ProfileFile?> GetProfileFileForUpdateAsync(int profileId, ProfileFileKind kind, CancellationToken cancellationToken)
    {
        return _dbContext.ProfileFiles.FirstOrDefaultAsync(file => file.ProfileId == profileId && file.Kind == kind, cancellationToken);
    }

    public Task<ProfileFile?> GetProfileFileAsync(ProfileFileKind kind, CancellationToken cancellationToken)
    {
        var profileId = _dbContext.Profiles
            .OrderBy(profile => profile.Id)
            .Select(profile => profile.Id)
            .Take(1);

        return _dbContext.ProfileFiles
            .AsNoTracking()
            .Where(file => file.Kind == kind && profileId.Contains(file.ProfileId))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
