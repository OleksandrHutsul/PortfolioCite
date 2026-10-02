using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.App.Services;

public partial class AdminApiService
{
    //TO-DO: HttpService and replace HttpClient with it.

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

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

    private async Task<ApiResult<T>> UploadAsync<T>(string url, Stream content, string fileName, string contentType, CancellationToken cancellationToken)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            using var fileContent = new StreamContent(content);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            form.Add(fileContent, "file", SafeFileName(fileName));

            using var response = await _httpClient.PostAsync(url, form, cancellationToken);

            await HandleUnauthorizedAsync(response);

            return await ReadAsync<T>(response, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return ApiResult<T>.Failure("The file could not be uploaded.");
        }
    }

    private async Task<ApiResult<T>> SendWithoutBodyAsync<T>(HttpMethod method, string url, CancellationToken cancellationToken)
    {
        try
        {
            using var message = new HttpRequestMessage(method, url);
            using var response = await _httpClient.SendAsync(message, cancellationToken);

            await HandleUnauthorizedAsync(response);

            return await ReadAsync<T>(response, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return ApiResult<T>.Failure("The file could not be removed.");
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

    private async Task<ApiResult<T>> ReadAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
            return ApiResult<T>.Failure(await ReadErrorAsync(response, cancellationToken));

        var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);

        return value is null ? ApiResult<T>.Failure("The server returned an empty response.") : ApiResult<T>.Success(ResolveResources(value));
    }

    private T ResolveResources<T>(T value)
    {
        var apiBase = _httpClient.BaseAddress;

        object resolved = value switch
        {
            ProfileAdminDto profile => ApiResourceUrl.Resolve(apiBase, profile),
            ProfileDto profile => ApiResourceUrl.Resolve(apiBase, profile),
            ProjectDto project => ApiResourceUrl.Resolve(apiBase, project),
            IReadOnlyList<ProjectDto> projects => ApiResourceUrl.Resolve(apiBase, projects),
            _ => value!
        };

        return (T)resolved;
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

            if (problem.TryGetProperty("detail", out var detail) && !string.IsNullOrWhiteSpace(detail.GetString()))
                return detail.GetString()!;

            if (problem.TryGetProperty("title", out var title) && !string.IsNullOrWhiteSpace(title.GetString()))
                return title.GetString()!;
        }
        catch
        {
        }

        if (response.StatusCode == HttpStatusCode.RequestEntityTooLarge)
            return "The file is too large.";

        return "The request could not be completed.";
    }

    private static string SafeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName);
        return string.IsNullOrWhiteSpace(name) ? "upload" : name;
    }
}
