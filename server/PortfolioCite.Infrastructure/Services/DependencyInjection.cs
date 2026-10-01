using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Infrastructure.Data;
using PortfolioCite.Infrastructure.Email;
using PortfolioCite.Infrastructure.Repositories;
using Resend;

namespace PortfolioCite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PortfolioDatabase")
            ?? throw new InvalidOperationException("Connection string 'PortfolioDatabase' is not configured.");

        var emailSection = configuration.GetSection(EmailOptions.SectionName);
        var emailOptions = emailSection.Get<EmailOptions>() ?? new EmailOptions();

        services.AddDbContext<PortfolioDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IPortfolioRepository, PortfolioRepository>();

        services.Configure<EmailOptions>(emailSection);

        services.AddResend(options =>
        {
            options.ApiToken = emailOptions.ApiKey;
            options.ThrowExceptions = true;
        });

        services.AddScoped<IEmailService, ResendEmailService>();

        return services;
    }
}
