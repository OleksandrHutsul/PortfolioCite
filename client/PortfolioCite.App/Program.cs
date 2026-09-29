using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PortfolioCite.App.Components;
using PortfolioCite.App.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("PortfolioAdmin", policy => policy.RequireRole("Admin"));
});

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthTokenStore>();
builder.Services.AddScoped<AuthorizedHttpHandler>();
builder.Services.AddScoped<AdminAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<AdminAuthenticationStateProvider>());

var apiBaseUrl = builder.Configuration["PortfolioApi:BaseUrl"];

builder.Services.AddHttpClient("PortfolioApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthorizedHttpHandler>();

builder.Services.AddScoped(provider =>
    provider.GetRequiredService<IHttpClientFactory>().CreateClient("PortfolioApi"));

builder.Services.AddScoped<PortfolioContentService>();
builder.Services.AddScoped<AdminApiService>();

await builder.Build().RunAsync();
