using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Services.Content;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.Api.Controllers;

[ApiController]
[Authorize(Policy = "PortfolioAdmin")]
[Route("api/admin/skill-categories")]
public class AdminSkillCategoriesController : ControllerBase
{
    private readonly ISkillsManagementService _skillsService;

    public AdminSkillCategoriesController(ISkillsManagementService skillsService)
    {
        _skillsService = skillsService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SkillCategoryAdminDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _skillsService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<SkillCategoryAdminDto>> Create(SaveSkillCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _skillsService.CreateAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, category);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SkillCategoryAdminDto>> Update(int id, SaveSkillCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _skillsService.UpdateAsync(id, request, cancellationToken);

        return category is null ? NotFound() : Ok(category);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _skillsService.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
