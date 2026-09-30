using System.Net;
using PortfolioCite.App.Services.Models;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.App.Services;

public partial class AdminApiService
{
    public async Task<ApiResult<ProfileAdminDto?>> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync("api/admin/profile", cancellationToken);

            await HandleUnauthorizedAsync(response);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return ApiResult<ProfileAdminDto?>.SuccessWithoutValue();

            return await ReadAsync<ProfileAdminDto?>(response, cancellationToken);
        }
        catch (Exception)
        {
            return ApiResult<ProfileAdminDto?>.Failure("The administration service is temporarily unavailable.");
        }
    }

    public Task<ApiResult<ProfileAdminDto>> UpdateProfileAsync(SaveProfileRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<ProfileAdminDto>(HttpMethod.Put, "api/admin/profile", request, cancellationToken);
    }

    public Task<ApiResult<ProfileAdminDto>> UploadAvatarAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        return UploadAsync<ProfileAdminDto>("api/admin/profile/avatar", content, fileName, contentType, cancellationToken);
    }

    public Task<ApiResult<ProfileAdminDto>> RemoveAvatarAsync(CancellationToken cancellationToken = default)
    {
        return SendWithoutBodyAsync<ProfileAdminDto>(HttpMethod.Delete, "api/admin/profile/avatar", cancellationToken);
    }

    public Task<ApiResult<ProfileAdminDto>> UploadResumeAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        return UploadAsync<ProfileAdminDto>("api/admin/profile/resume", content, fileName, contentType, cancellationToken);
    }

    public Task<ApiResult<ProfileAdminDto>> RemoveResumeAsync(CancellationToken cancellationToken = default)
    {
        return SendWithoutBodyAsync<ProfileAdminDto>(HttpMethod.Delete, "api/admin/profile/resume", cancellationToken);
    }
}
