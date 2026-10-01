using Microsoft.Extensions.DependencyInjection;
using PortfolioCite.Application.Services.Contact;
using PortfolioCite.Application.Services.Content;
using PortfolioCite.Application.Services.Authentication;
using PortfolioCite.Application.Services.PortfolioQuery;
using PortfolioCite.Application.Services.Projects;

namespace PortfolioCite.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IContactNotification, ContactOwnerNotification>();
        services.AddScoped<IContactMessageService, ContactMessageService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IPortfolioQueryService, PortfolioQueryService>();
        services.AddScoped<IProfileManagementService, ProfileManagementService>();
        services.AddScoped<ISkillsManagementService, SkillsManagementService>();
        services.AddScoped<ICareerManagementService, CareerManagementService>();
        services.AddScoped<IReferenceManagementService, ReferenceManagementService>();

        return services;
    }
}
