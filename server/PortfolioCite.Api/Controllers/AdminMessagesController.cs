using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Services.Contact;
using PortfolioCite.Contracts.Contact;

namespace PortfolioCite.Api.Controllers;

[ApiController]
[Authorize(Policy = "PortfolioAdmin")]
[Route("api/admin/messages")]
public class AdminMessagesController : ControllerBase
{
    private readonly IContactMessageService _messageService;

    public AdminMessagesController(IContactMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContactSubmissionDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _messageService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContactSubmissionDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var message = await _messageService.GetAsync(id, cancellationToken);

        return message is null ? NotFound() : Ok(message);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<ActionResult<ContactSubmissionDto>> SetRead(Guid id, UpdateContactReadRequest request, CancellationToken cancellationToken)
    {
        var message = await _messageService.SetReadAsync(id, request.IsRead, cancellationToken);

        return message is null ? NotFound() : Ok(message);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _messageService.DeleteAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
