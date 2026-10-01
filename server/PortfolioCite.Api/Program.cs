using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PortfolioCite.Api.Configuration;
using PortfolioCite.Api.Filters;
using PortfolioCite.Api.Services;
using PortfolioCite.Application.Abstractions;
using PortfolioCite.Application.Services;
using PortfolioCite.Domain.Entities;
using PortfolioCite.Infrastructure;
using PortfolioCite.Infrastructure.Data;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ContentValidationExceptionFilter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "JWT issuer is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "JWT audience is required.")
    .Validate(options => options.SigningKey.Length >= 32, "JWT signing key must contain at least 32 characters.")
    .Validate(options => options.AccessTokenMinutes is > 0 and <= 1440, "JWT access token lifetime must be between 1 and 1440 minutes.")
    .ValidateOnStart();

builder.Services.Configure<InitialAdminOptions>(builder.Configuration.GetSection(InitialAdminOptions.SectionName));

builder.Services.AddScoped<IPasswordHasher<Administrator>, PasswordHasher<Administrator>>();
builder.Services.AddScoped<IAccessTokenService, JwtAccessTokenService>();
builder.Services.AddScoped<InitialAdminProvisioner>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? throw new InvalidOperationException("CORS allowed origins are not configured.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("PortfolioApp", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((options, jwtOptions) =>
    {
        var jwt = jwtOptions.Value;

        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PortfolioAdmin", policy => policy.RequireRole("Admin"));
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("ContactForm", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0
            }));

    options.AddPolicy("AdminLogin", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(5),
                QueueLimit = 0
            }));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PortfolioApp");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    await dbContext.Database.MigrateAsync(app.Lifetime.ApplicationStopping);

    var provisioner = scope.ServiceProvider.GetRequiredService<InitialAdminProvisioner>();
    await provisioner.ProvisionAsync(app.Lifetime.ApplicationStopping);
}

app.Run();
