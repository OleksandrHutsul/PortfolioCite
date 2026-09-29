using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Infrastructure.Data;
using PortfolioCite.Infrastructure.Repositories;

namespace PortfolioCite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PortfolioDatabase")
            ?? throw new InvalidOperationException("Connection string 'PortfolioDatabase' is not configured.");

        services.AddDbContext<PortfolioDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IPortfolioRepository, PortfolioRepository>();

        return services;
    }
}
