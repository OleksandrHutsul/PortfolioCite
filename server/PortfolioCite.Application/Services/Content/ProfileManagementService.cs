using PortfolioCite.Application.Abstractions;
using PortfolioCite.Contracts.Administration;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Content;

public class ProfileManagementService : IProfileManagementService
{
    private readonly IPortfolioRepository _repository;

    public ProfileManagementService(IPortfolioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProfileAdminDto?> GetAsync(CancellationToken cancellationToken)
    {
        var profile = await _repository.GetProfileAsync(cancellationToken);

        return profile is null ? null : Map(profile);
    }

    public async Task<ProfileAdminDto> SaveAsync(SaveProfileRequest request, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetProfileForUpdateAsync(cancellationToken);

        if (profile is null)
        {
            profile = new Profile();
            await _repository.AddAsync(profile, cancellationToken);
        }

        profile.FullName = request.FullName.Trim();
        profile.Role = request.Role.Trim();
        profile.Location = request.Location.Trim();
        profile.Summary = request.Summary.Trim();
        profile.CurrentFocus = request.CurrentFocus.Trim();
        profile.Languages = request.Languages.Trim();
        profile.Email = request.Email.Trim();
        profile.AvatarUrl = request.AvatarUrl.Trim();
        profile.ResumeUrl = NullIfWhiteSpace(request.ResumeUrl);
        profile.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.SaveChangesAsync(cancellationToken);

        return Map(profile);
    }

    private static ProfileAdminDto Map(Profile profile)
    {
        return new ProfileAdminDto(profile.FullName, profile.Role, profile.Location, profile.Summary, profile.CurrentFocus, profile.Languages, profile.Email, profile.AvatarUrl, profile.ResumeUrl, profile.UpdatedAt);
    }

    private static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
