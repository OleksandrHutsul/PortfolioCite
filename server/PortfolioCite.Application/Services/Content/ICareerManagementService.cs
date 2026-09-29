using PortfolioCite.Contracts.Administration;

namespace PortfolioCite.Application.Services.Content;

public interface ICareerManagementService
{
    Task<IReadOnlyList<ExperienceAdminDto>> GetExperiencesAsync(CancellationToken cancellationToken);
    Task<ExperienceAdminDto> CreateExperienceAsync(SaveExperienceRequest request, CancellationToken cancellationToken);
    Task<ExperienceAdminDto?> UpdateExperienceAsync(int id, SaveExperienceRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteExperienceAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<EducationAdminDto>> GetEducationAsync(CancellationToken cancellationToken);
    Task<EducationAdminDto> CreateEducationAsync(SaveEducationRequest request, CancellationToken cancellationToken);
    Task<EducationAdminDto?> UpdateEducationAsync(int id, SaveEducationRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteEducationAsync(int id, CancellationToken cancellationToken);
}
