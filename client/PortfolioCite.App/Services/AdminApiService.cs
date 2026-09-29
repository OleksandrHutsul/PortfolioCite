using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Administration;
using PortfolioCite.Contracts.Authentication;
using PortfolioCite.Contracts.Contact;
using PortfolioCite.Contracts.Portfolio;
using PortfolioCite.Contracts.Projects;

namespace PortfolioCite.App.Services;

public class AdminApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly AdminAuthenticationStateProvider _authenticationStateProvider;

    public AdminApiService(HttpClient httpClient, AdminAuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = response.StatusCode == HttpStatusCode.Unauthorized
                    ? "The email or password is incorrect."
                    : "Sign-in is temporarily unavailable.";

                return ApiResult<LoginResponse>.Failure(error);
            }

            var login = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions, cancellationToken);

            if (login is null)
                return ApiResult<LoginResponse>.Failure("The server returned an invalid response.");

            await _authenticationStateProvider.SignInAsync(login);

            return ApiResult<LoginResponse>.Success(login);
        }
        catch (Exception)
        {
            return ApiResult<LoginResponse>.Failure("Sign-in is temporarily unavailable.");
        }
    }

    public Task LogoutAsync()
    {
        return _authenticationStateProvider.SignOutAsync();
    }

    public Task<ApiResult<List<ProjectDto>>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<ProjectDto>>("api/admin/projects", cancellationToken);
    }

    public Task<ApiResult<ProjectDto>> GetProjectAsync(int id, CancellationToken cancellationToken = default)
    {
        return GetAsync<ProjectDto>($"api/admin/projects/{id}", cancellationToken);
    }

    public Task<ApiResult<ProjectDto>> CreateProjectAsync(SaveProjectRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ProjectDto>(HttpMethod.Post, "api/admin/projects", request, cancellationToken);
    }

    public Task<ApiResult<ProjectDto>> UpdateProjectAsync(int id, SaveProjectRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ProjectDto>(HttpMethod.Put, $"api/admin/projects/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteProjectAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/projects/{id}", cancellationToken);
    }

    public Task<ApiResult<List<ContactSubmissionDto>>> GetMessagesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<ContactSubmissionDto>>("api/admin/messages", cancellationToken);
    }

    public Task<ApiResult<ContactSubmissionDto>> SetMessageReadAsync(Guid id, bool isRead, CancellationToken cancellationToken = default)
    {
        var request = new UpdateContactReadRequest { IsRead = isRead };

        return SendAsync<ContactSubmissionDto>(HttpMethod.Patch, $"api/admin/messages/{id}/read", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/messages/{id}", cancellationToken);
    }

    public Task<ApiResult<ProfileAdminDto>> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<ProfileAdminDto>("api/admin/profile", cancellationToken);
    }

    public Task<ApiResult<ProfileAdminDto>> UpdateProfileAsync(SaveProfileRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ProfileAdminDto>(HttpMethod.Put, "api/admin/profile", request, cancellationToken);
    }

    public Task<ApiResult<List<SkillCategoryAdminDto>>> GetSkillCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<SkillCategoryAdminDto>>("api/admin/skill-categories", cancellationToken);
    }

    public Task<ApiResult<SkillCategoryAdminDto>> CreateSkillCategoryAsync(SaveSkillCategoryRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<SkillCategoryAdminDto>(HttpMethod.Post, "api/admin/skill-categories", request, cancellationToken);
    }

    public Task<ApiResult<SkillCategoryAdminDto>> UpdateSkillCategoryAsync(int id, SaveSkillCategoryRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<SkillCategoryAdminDto>(HttpMethod.Put, $"api/admin/skill-categories/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteSkillCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/skill-categories/{id}", cancellationToken);
    }

    public Task<ApiResult<List<ExperienceAdminDto>>> GetExperiencesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<ExperienceAdminDto>>("api/admin/experience", cancellationToken);
    }

    public Task<ApiResult<ExperienceAdminDto>> CreateExperienceAsync(SaveExperienceRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ExperienceAdminDto>(HttpMethod.Post, "api/admin/experience", request, cancellationToken);
    }

    public Task<ApiResult<ExperienceAdminDto>> UpdateExperienceAsync(int id, SaveExperienceRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ExperienceAdminDto>(HttpMethod.Put, $"api/admin/experience/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteExperienceAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/experience/{id}", cancellationToken);
    }

    public Task<ApiResult<List<EducationAdminDto>>> GetEducationAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<EducationAdminDto>>("api/admin/education", cancellationToken);
    }

    public Task<ApiResult<EducationAdminDto>> CreateEducationAsync(SaveEducationRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<EducationAdminDto>(HttpMethod.Post, "api/admin/education", request, cancellationToken);
    }

    public Task<ApiResult<EducationAdminDto>> UpdateEducationAsync(int id, SaveEducationRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<EducationAdminDto>(HttpMethod.Put, $"api/admin/education/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteEducationAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/education/{id}", cancellationToken);
    }

    public Task<ApiResult<List<CertificateAdminDto>>> GetCertificatesAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<CertificateAdminDto>>("api/admin/certificates", cancellationToken);
    }

    public Task<ApiResult<CertificateAdminDto>> CreateCertificateAsync(SaveCertificateRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<CertificateAdminDto>(HttpMethod.Post, "api/admin/certificates", request, cancellationToken);
    }

    public Task<ApiResult<CertificateAdminDto>> UpdateCertificateAsync(int id, SaveCertificateRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<CertificateAdminDto>(HttpMethod.Put, $"api/admin/certificates/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteCertificateAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/certificates/{id}", cancellationToken);
    }

    public Task<ApiResult<List<ContactLinkAdminDto>>> GetContactLinksAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<List<ContactLinkAdminDto>>("api/admin/contact-links", cancellationToken);
    }

    public Task<ApiResult<ContactLinkAdminDto>> CreateContactLinkAsync(SaveContactLinkRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ContactLinkAdminDto>(HttpMethod.Post, "api/admin/contact-links", request, cancellationToken);
    }

    public Task<ApiResult<ContactLinkAdminDto>> UpdateContactLinkAsync(int id, SaveContactLinkRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ContactLinkAdminDto>(HttpMethod.Put, $"api/admin/contact-links/{id}", request, cancellationToken);
    }

    public Task<ApiResult<bool>> DeleteContactLinkAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync($"api/admin/contact-links/{id}", cancellationToken);
    }

    private async Task<ApiResult<T>> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            await HandleUnauthorizedAsync(response);

            return await ReadAsync<T>(response, cancellationToken);
        }
        catch (Exception)
        {
            return ApiResult<T>.Failure("The administration service is temporarily unavailable.");
        }
    }

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object request, CancellationToken cancellationToken)
    {
        try
        {
            using var message = new HttpRequestMessage(method, url)
            {
                Content = JsonContent.Create(request)
            };

            using var response = await _httpClient.SendAsync(message, cancellationToken);

            await HandleUnauthorizedAsync(response);

            return await ReadAsync<T>(response, cancellationToken);
        }
        catch (Exception)
        {
            return ApiResult<T>.Failure("The change could not be saved.");
        }
    }

    private async Task<ApiResult<bool>> DeleteAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.DeleteAsync(url, cancellationToken);

            await HandleUnauthorizedAsync(response);

            return response.IsSuccessStatusCode
                ? ApiResult<bool>.Success(true)
                : ApiResult<bool>.Failure(await ReadErrorAsync(response, cancellationToken));
        }
        catch (Exception)
        {
            return ApiResult<bool>.Failure("The item could not be deleted.");
        }
    }

    private async Task HandleUnauthorizedAsync(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            await _authenticationStateProvider.SignOutAsync();
    }

    private static async Task<ApiResult<T>> ReadAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
            return ApiResult<T>.Failure(await ReadErrorAsync(response, cancellationToken));

        var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);

        return value is null
            ? ApiResult<T>.Failure("The server returned an empty response.")
            : ApiResult<T>.Success(value);
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return "Your session has expired. Please sign in again.";

        if (response.StatusCode == HttpStatusCode.Forbidden)
            return "You do not have permission to perform this action.";

        try
        {
            var problem = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

            if (problem.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                var messages = errors.EnumerateObject()
                    .SelectMany(error => error.Value.ValueKind == JsonValueKind.Array
                        ? error.Value.EnumerateArray().Select(value => value.GetString())
                        : [error.Value.GetString()])
                    .Where(message => !string.IsNullOrWhiteSpace(message))
                    .ToList();

                if (messages.Count > 0)
                    return string.Join(" ", messages);
            }

            if (problem.TryGetProperty("title", out var title))
                return title.GetString() ?? "The request failed.";
        }
        catch
        {
        }

        return "The request could not be completed.";
    }
}
