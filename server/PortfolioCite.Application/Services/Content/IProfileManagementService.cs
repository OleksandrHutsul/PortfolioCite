using PortfolioCite.Contracts.Administration;

namespace PortfolioCite.Application.Services.Content;

public interface IProfileManagementService
{
    Task<ProfileAdminDto?> GetAsync(CancellationToken cancellationToken);
    Task<ProfileAdminDto> SaveAsync(SaveProfileRequest request, CancellationToken cancellationToken);
}
