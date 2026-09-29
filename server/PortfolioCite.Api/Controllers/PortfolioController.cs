using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Services.PortfolioQuery;
using PortfolioCite.Contracts.Portfolio;

namespace PortfolioCite.Api.Controllers;

[ApiController]
[Route("api/portfolio")]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioQueryService _queryService;

    public PortfolioController(IPortfolioQueryService queryService)
    {
        _queryService = queryService;
    }

    [HttpGet]
    [ProducesResponseType<PortfolioSnapshotDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PortfolioSnapshotDto>> Get(CancellationToken cancellationToken)
    {
        var portfolio = await _queryService.GetSnapshotAsync(cancellationToken);

        return Ok(portfolio);
    }
}
