using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Api.Helpers;
using PortfolioCite.Application.Models;
using PortfolioCite.Application.Services.Content;
using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Administration.Rules;

namespace PortfolioCite.Api.Controllers;

[ApiController]
[Authorize(Policy = "PortfolioAdmin")]
[Route("api/admin/profile")]
public class AdminProfileController : ControllerBase
{
    private readonly IProfileManagementService _profileService;

    public AdminProfileController(IProfileManagementService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    [ProducesResponseType<ProfileAdminDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileAdminDto>> Get(CancellationToken cancellationToken)
    {
        var profile = await _profileService.GetAsync(cancellationToken);
        return profile is null ? NotFound() : Ok(ProfileMediaLinks.Apply(Request, profile));
    }

    [HttpPut]
    [ProducesResponseType<ProfileAdminDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProfileAdminDto>> Update(SaveProfileRequest request, CancellationToken cancellationToken)
    {
        var profile = await _profileService.SaveAsync(request, cancellationToken);
        return Ok(ProfileMediaLinks.Apply(Request, profile));
    }

    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MediaRules.ImageRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MediaRules.ImageRequestBytes)]
    public Task<ActionResult<ProfileAdminDto>> UploadAvatar(IFormFile? file, CancellationToken cancellationToken)
    {
        return SaveFileAsync(file, MediaRules.ImageMaxBytes, MediaRules.ImageSizeError, _profileService.SaveAvatarAsync, cancellationToken);
    }

    [HttpDelete("avatar")]
    public async Task<ActionResult<ProfileAdminDto>> RemoveAvatar(CancellationToken cancellationToken)
    {
        var result = await _profileService.RemoveAvatarAsync(cancellationToken);
        return ProfileMediaResultMapper.Map(this, result);
    }

    [HttpPost("resume")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(ProfileMediaRules.ResumeRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = ProfileMediaRules.ResumeRequestBytes)]
    public Task<ActionResult<ProfileAdminDto>> UploadResume(IFormFile? file, CancellationToken cancellationToken)
    {
        return SaveFileAsync(file, ProfileMediaRules.ResumeMaxBytes, ProfileMediaRules.ResumeSizeError, _profileService.SaveResumeAsync, cancellationToken);
    }

    [HttpDelete("resume")]
    public async Task<ActionResult<ProfileAdminDto>> RemoveResume(CancellationToken cancellationToken)
    {
        var result = await _profileService.RemoveResumeAsync(cancellationToken);
        return ProfileMediaResultMapper.Map(this, result);
    }

    private async Task<ActionResult<ProfileAdminDto>> SaveFileAsync(IFormFile? file, long maxBytes, string sizeError,
        Func<ProfileFileContent, CancellationToken, Task<ProfileMediaResult>> save, CancellationToken cancellationToken)
    {
        var validationError = ProfileFileValidator.Validate(file, maxBytes, sizeError);

        if (validationError is not null)
            return validationError;

        await using var stream = file!.OpenReadStream();
        var content = new ProfileFileContent(stream, file.FileName, file.ContentType, file.Length);
        var result = await save(content, cancellationToken);

        return ProfileMediaResultMapper.Map(this, result);
    }
}
