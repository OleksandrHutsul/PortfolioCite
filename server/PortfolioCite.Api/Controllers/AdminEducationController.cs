using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Services.Content;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.Api.Controllers;

[ApiController]
[Authorize(Policy = "PortfolioAdmin")]
[Route("api/admin/education")]
public class AdminEducationController : ControllerBase
{
    private readonly ICareerManagementService _careerService;

    public AdminEducationController(ICareerManagementService careerService)
    {
        _careerService = careerService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EducationAdminDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _careerService.GetEducationAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<EducationAdminDto>> Create(SaveEducationRequest request, CancellationToken cancellationToken)
    {
        var education = await _careerService.CreateEducationAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, education);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EducationAdminDto>> Update(int id, SaveEducationRequest request, CancellationToken cancellationToken)
    {
        var education = await _careerService.UpdateEducationAsync(id, request, cancellationToken);

        return education is null ? NotFound() : Ok(education);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _careerService.DeleteEducationAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
