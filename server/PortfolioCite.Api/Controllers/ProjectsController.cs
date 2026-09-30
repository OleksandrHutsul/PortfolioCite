using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Services.PortfolioQuery;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IPortfolioQueryService _queryService;

    public ProjectsController(IPortfolioQueryService queryService)
    {
        _queryService = queryService;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProjectDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _queryService.GetProjectsAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProjectDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> Get(int id, CancellationToken cancellationToken)
    {
        var project = await _queryService.GetProjectAsync(id, cancellationToken);

        return project is null ? NotFound() : Ok(project);
    }
}
