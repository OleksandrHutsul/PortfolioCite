using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Api.Helpers;
using PortfolioCite.Application.Models;
using PortfolioCite.Application.Services.PortfolioQuery;
using PortfolioCite.Contracts.Portfolio.Models;

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

        return Ok(ProfileMediaLinks.Apply(Request, portfolio));
    }

    [HttpGet("avatar/{fileName}")]
    public Task<IActionResult> Avatar(string fileName, CancellationToken cancellationToken)
    {
        return SendFileAsync(_queryService.GetAvatarAsync, cancellationToken);
    }

    [HttpGet("resume/{fileName}")]
    public Task<IActionResult> Resume(string fileName, CancellationToken cancellationToken)
    {
        return SendFileAsync(_queryService.GetResumeAsync, cancellationToken);
    }

    private async Task<IActionResult> SendFileAsync(Func<CancellationToken, Task<ProfileFileDownload?>> load, CancellationToken cancellationToken)
    {
        var file = await load(cancellationToken);

        if (file is null)
            return NotFound();

        Response.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
        {
            FileNameStar = file.FileName
        }.ToString();
        Response.Headers.XContentTypeOptions = "nosniff";

        return File(file.Content, file.ContentType);
    }
}
