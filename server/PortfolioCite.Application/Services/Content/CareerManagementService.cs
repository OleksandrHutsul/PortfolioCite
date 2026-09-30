using PortfolioCite.Application.Abstractions;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Application.Services.Content;

public class CareerManagementService : ICareerManagementService
{
    private readonly IPortfolioRepository _repository;

    public CareerManagementService(IPortfolioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ExperienceAdminDto>> GetExperiencesAsync(CancellationToken cancellationToken)
    {
        var experiences = await _repository.GetWorkExperiencesAsync(cancellationToken);

        return experiences.Select(MapExperience).ToList();
    }

    public async Task<ExperienceAdminDto> CreateExperienceAsync(SaveExperienceRequest request, CancellationToken cancellationToken)
    {
        var experience = new WorkExperience();
        Apply(experience, request);

        await _repository.AddAsync(experience, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapExperience(experience);
    }

    public async Task<ExperienceAdminDto?> UpdateExperienceAsync(int id, SaveExperienceRequest request, CancellationToken cancellationToken)
    {
        var experience = await _repository.GetWorkExperienceForUpdateAsync(id, cancellationToken);
        if (experience is null) return null;

        Apply(experience, request);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapExperience(experience);
    }

    public async Task<bool> DeleteExperienceAsync(int id, CancellationToken cancellationToken)
    {
        var experience = await _repository.GetWorkExperienceForUpdateAsync(id, cancellationToken);
        if (experience is null) return false;

        _repository.Remove(experience);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<EducationAdminDto>> GetEducationAsync(CancellationToken cancellationToken)
    {
        var education = await _repository.GetEducationAsync(cancellationToken);

        return education.Select(MapEducation).ToList();
    }

    public async Task<EducationAdminDto> CreateEducationAsync(SaveEducationRequest request, CancellationToken cancellationToken)
    {
        var education = new Education();
        Apply(education, request);

        await _repository.AddAsync(education, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapEducation(education);
    }

    public async Task<EducationAdminDto?> UpdateEducationAsync(int id, SaveEducationRequest request, CancellationToken cancellationToken)
    {
        var education = await _repository.GetForUpdateAsync<Education>(id, cancellationToken);
        if (education is null) return null;

        Apply(education, request);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapEducation(education);
    }

    public async Task<bool> DeleteEducationAsync(int id, CancellationToken cancellationToken)
    {
        var education = await _repository.GetForUpdateAsync<Education>(id, cancellationToken);
        if (education is null) return false;

        _repository.Remove(education);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void Apply(WorkExperience experience, SaveExperienceRequest request)
    {
        experience.Company = request.Company.Trim();
        experience.Position = request.Position.Trim();
        experience.StartedOn = request.StartedOn;
        experience.EndedOn = request.EndedOn;
        experience.Summary = request.Summary.Trim();
        experience.DisplayOrder = request.DisplayOrder;

        experience.Highlights.Clear();
        experience.Highlights.AddRange(request.Highlights.Select((text, index) => new WorkHighlight
        {
            Text = text.Trim(),
            DisplayOrder = index
        }));
    }

    private static void Apply(Education education, SaveEducationRequest request)
    {
        education.Institution = request.Institution.Trim();
        education.Degree = request.Degree.Trim();
        education.FieldOfStudy = NullIfWhiteSpace(request.FieldOfStudy);
        education.StartedOn = request.StartedOn;
        education.EndedOn = request.EndedOn;
        education.Description = NullIfWhiteSpace(request.Description);
        education.DisplayOrder = request.DisplayOrder;
    }

    private static ExperienceAdminDto MapExperience(WorkExperience experience)
    {
        var highlights = experience.Highlights
            .OrderBy(highlight => highlight.DisplayOrder)
            .Select(highlight => highlight.Text)
            .ToList();

        return new ExperienceAdminDto(experience.Id, experience.Company, experience.Position, experience.StartedOn, experience.EndedOn, experience.Summary, experience.DisplayOrder, highlights);
    }

    private static EducationAdminDto MapEducation(Education education)
    {
        return new EducationAdminDto(education.Id, education.Institution, education.Degree, education.FieldOfStudy, education.StartedOn, education.EndedOn, education.Description, education.DisplayOrder);
    }

    private static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
