using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Api.Helpers;
using PortfolioCite.Application.Models;
using PortfolioCite.Application.Services.Projects;
using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Contracts.Portfolio.Models;
using PortfolioCite.Contracts.Projects.Models;

namespace PortfolioCite.Api.Controllers;

[ApiController]
[Authorize(Policy = "PortfolioAdmin")]
[Route("api/admin/projects")]
public class AdminProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public AdminProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(ProjectMediaLinks.Apply(Request, await _projectService.GetAllForAdminAsync(cancellationToken)));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProjectDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> Get(int id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetForAdminAsync(id, cancellationToken);

        return project is null ? NotFound() : Ok(ProjectMediaLinks.Apply(Request, project));
    }

    [HttpPost]
    [ProducesResponseType<ProjectDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<ProjectDto>> Create(SaveProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await _projectService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = project.Id }, ProjectMediaLinks.Apply(Request, project));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<ProjectDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> Update(int id, SaveProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await _projectService.UpdateAsync(id, request, cancellationToken);

        return project is null ? NotFound() : Ok(ProjectMediaLinks.Apply(Request, project));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _projectService.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MediaRules.ImageRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MediaRules.ImageRequestBytes)]
    public Task<ActionResult<ProjectDto>> UploadImage(int id, IFormFile? file, CancellationToken cancellationToken)
    {
        return SaveImageAsync(id, file, cancellationToken);
    }

    [HttpDelete("{id:int}/image")]
    public async Task<ActionResult<ProjectDto>> RemoveImage(int id, CancellationToken cancellationToken)
    {
        var result = await _projectService.RemoveImageAsync(id, cancellationToken);

        return ProjectImageResultMapper.Map(this, result);
    }

    private async Task<ActionResult<ProjectDto>> SaveImageAsync(int id, IFormFile? file, CancellationToken cancellationToken)
    {
        var validationError = ProfileFileValidator.Validate(file, MediaRules.ImageMaxBytes, MediaRules.ImageSizeError);

        if (validationError is not null)
            return validationError;

        await using var stream = file!.OpenReadStream();
        var content = new ProfileFileContent(stream, file.FileName, file.ContentType, file.Length);
        var result = await _projectService.SaveImageAsync(id, content, cancellationToken);

        return ProjectImageResultMapper.Map(this, result);
    }
}
