using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Services.Content;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.Api.Controllers;

[ApiController]
[Authorize(Policy = "PortfolioAdmin")]
[Route("api/admin/experience")]
public class AdminExperiencesController : ControllerBase
{
    private readonly ICareerManagementService _careerService;

    public AdminExperiencesController(ICareerManagementService careerService)
    {
        _careerService = careerService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExperienceAdminDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _careerService.GetExperiencesAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ExperienceAdminDto>> Create(SaveExperienceRequest request, CancellationToken cancellationToken)
    {
        var experience = await _careerService.CreateExperienceAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, experience);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ExperienceAdminDto>> Update(int id, SaveExperienceRequest request, CancellationToken cancellationToken)
    {
        var experience = await _careerService.UpdateExperienceAsync(id, request, cancellationToken);

        return experience is null ? NotFound() : Ok(experience);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _careerService.DeleteExperienceAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
