namespace PortfolioCite.Contracts.Portfolio;

public record PortfolioSnapshotDto(ProfileDto Profile, IReadOnlyList<SkillCategoryDto> SkillCategories, IReadOnlyList<ProjectDto> Projects,
    IReadOnlyList<CertificateDto> Certificates, IReadOnlyList<WorkExperienceDto> WorkExperiences, IReadOnlyList<EducationDto> Education,
    IReadOnlyList<ContactLinkDto> ContactLinks);
