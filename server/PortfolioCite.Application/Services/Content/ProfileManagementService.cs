using PortfolioCite.Application.Abstractions;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Content;

public partial class ProfileManagementService : IProfileManagementService
{
    private readonly IPortfolioRepository _repository;

    public ProfileManagementService(IPortfolioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProfileAdminDto?> GetAsync(CancellationToken cancellationToken)
    {
        var profile = await _repository.GetProfileAsync(cancellationToken);

        return profile is null ? null : await MapAsync(profile, cancellationToken);
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
        profile.Email = request.Email.Trim();
        profile.UpdatedAt = DateTimeOffset.UtcNow;
        ReplaceLanguages(profile, request);

        await _repository.SaveChangesAsync(cancellationToken);

        return await MapAsync(profile, cancellationToken);
    }

    private static void ReplaceLanguages(Profile profile, SaveProfileRequest request)
    {
        profile.Languages.Clear();
        profile.Languages.AddRange((request.Languages ?? []).Select(language => new ProfileLanguage
        {
            Name = language.Name.Trim(),
            Proficiency = language.Proficiency,
            DisplayOrder = language.DisplayOrder
        }));
    }

    private async Task<ProfileAdminDto> MapAsync(Profile profile, CancellationToken cancellationToken)
    {
        var languages = profile.Languages
            .OrderBy(language => language.DisplayOrder)
            .ThenBy(language => language.Id)
            .Select(language => new ProfileLanguageAdminDto(language.Id, language.Name, language.Proficiency, language.DisplayOrder))
            .ToList();

        var files = await _repository.GetProfileFileSummariesAsync(profile.Id, cancellationToken);

        return new ProfileAdminDto(profile.FullName, profile.Role, profile.Location, profile.Summary, profile.CurrentFocus, languages, profile.Email,
            ProfileFileLocations.Avatar(files, profile.UpdatedAt), ProfileFileLocations.Resume(files, profile.UpdatedAt), profile.UpdatedAt);
    }
}
