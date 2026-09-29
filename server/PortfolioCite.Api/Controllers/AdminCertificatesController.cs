using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCite.Application.Services.Content;
using PortfolioCite.Contracts.Administration;

namespace PortfolioCite.Api.Controllers;

[ApiController]
[Authorize(Policy = "PortfolioAdmin")]
[Route("api/admin/certificates")]
public class AdminCertificatesController : ControllerBase
{
    private readonly IReferenceManagementService _referenceService;

    public AdminCertificatesController(IReferenceManagementService referenceService)
    {
        _referenceService = referenceService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CertificateAdminDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _referenceService.GetCertificatesAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<CertificateAdminDto>> Create(SaveCertificateRequest request, CancellationToken cancellationToken)
    {
        var certificate = await _referenceService.CreateCertificateAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, certificate);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CertificateAdminDto>> Update(int id, SaveCertificateRequest request, CancellationToken cancellationToken)
    {
        var certificate = await _referenceService.UpdateCertificateAsync(id, request, cancellationToken);

        return certificate is null ? NotFound() : Ok(certificate);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _referenceService.DeleteCertificateAsync(id, cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
}
