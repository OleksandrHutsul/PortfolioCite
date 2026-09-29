using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Services.Content;
using PortfolioCite.Contracts.Administration;

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
    public async Task<ActionResult<ProfileAdminDto>> Get(CancellationToken cancellationToken)
    {
        var profile = await _profileService.GetAsync(cancellationToken);

        return profile is null ? NotFound() : Ok(profile);
    }

    [HttpPut]
    public async Task<ActionResult<ProfileAdminDto>> Update(SaveProfileRequest request, CancellationToken cancellationToken)
    {
        var profile = await _profileService.SaveAsync(request, cancellationToken);

        return Ok(profile);
    }
}
