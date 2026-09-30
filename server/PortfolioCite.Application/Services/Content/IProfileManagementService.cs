using PortfolioCite.Application.Models;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.Application.Services.Content;

public interface IProfileManagementService
{
    Task<ProfileAdminDto?> GetAsync(CancellationToken cancellationToken);
    Task<ProfileAdminDto> SaveAsync(SaveProfileRequest request, CancellationToken cancellationToken);
    Task<ProfileMediaResult> SaveAvatarAsync(ProfileFileContent file, CancellationToken cancellationToken);
    Task<ProfileMediaResult> RemoveAvatarAsync(CancellationToken cancellationToken);
    Task<ProfileMediaResult> SaveResumeAsync(ProfileFileContent file, CancellationToken cancellationToken);
    Task<ProfileMediaResult> RemoveResumeAsync(CancellationToken cancellationToken);
}
