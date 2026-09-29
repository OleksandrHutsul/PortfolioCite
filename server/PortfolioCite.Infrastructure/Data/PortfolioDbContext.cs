using Microsoft.EntityFrameworkCore;
using PortfolioCite.Domain.Entities;

namespace PortfolioCite.Infrastructure.Data;

public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
    {
    }

    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<SkillCategory> SkillCategories => Set<SkillCategory>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Technology> Technologies => Set<Technology>();
    public DbSet<ProjectTechnology> ProjectTechnologies => Set<ProjectTechnology>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<WorkExperience> WorkExperiences => Set<WorkExperience>();
    public DbSet<WorkHighlight> WorkHighlights => Set<WorkHighlight>();
    public DbSet<Education> Education => Set<Education>();
    public DbSet<ContactLink> ContactLinks => Set<ContactLink>();
    public DbSet<ContactSubmission> ContactSubmissions => Set<ContactSubmission>();
    public DbSet<Administrator> Administrators => Set<Administrator>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortfolioDbContext).Assembly);
    }
}
