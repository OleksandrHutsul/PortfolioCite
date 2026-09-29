using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Services.Content;
using PortfolioCite.Contracts.Administration;

namespace PortfolioCite.Api.Controllers;

[ApiController]
[Authorize(Policy = "PortfolioAdmin")]
[Route("api/admin/contact-links")]
public class AdminContactLinksController : ControllerBase
{
    private readonly IReferenceManagementService _referenceService;

    public AdminContactLinksController(IReferenceManagementService referenceService)
    {
        _referenceService = referenceService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContactLinkAdminDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _referenceService.GetContactLinksAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ContactLinkAdminDto>> Create(SaveContactLinkRequest request, CancellationToken cancellationToken)
    {
        var link = await _referenceService.CreateContactLinkAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, link);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ContactLinkAdminDto>> Update(int id, SaveContactLinkRequest request, CancellationToken cancellationToken)
    {
        var link = await _referenceService.UpdateContactLinkAsync(id, request, cancellationToken);

        return link is null ? NotFound() : Ok(link);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _referenceService.DeleteContactLinkAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
