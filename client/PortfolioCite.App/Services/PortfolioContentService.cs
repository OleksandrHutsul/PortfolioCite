using System.Net.Http.Json;
using PortfolioCite.Contracts.Contact;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.App.Services;

public class PortfolioContentService
{
    private readonly HttpClient _httpClient;

    public PortfolioContentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PortfolioSnapshotDto> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = await _httpClient.GetFromJsonAsync<PortfolioSnapshotDto>("api/portfolio", cancellationToken)
            ?? throw new InvalidOperationException("The portfolio API returned an empty response.");

        return ApiResourceUrl.Resolve(_httpClient.BaseAddress, snapshot);
    }

    public async Task<IReadOnlyList<ProjectDto>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await _httpClient.GetFromJsonAsync<IReadOnlyList<ProjectDto>>("api/projects", cancellationToken)
            ?? throw new InvalidOperationException("The projects API returned an empty response.");

        return ApiResourceUrl.Resolve(_httpClient.BaseAddress, projects);
    }

    public async Task SubmitContactAsync(CreateContactSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/contact", request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
