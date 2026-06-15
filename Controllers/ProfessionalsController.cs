using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamsApi.Models.Requests;
using TeamsApi.Models.Responses;
using TeamsApi.Services;

namespace TeamsApi.Controllers;

[ApiController]
[Route("api/professionals")]
[Authorize(Roles = "Admin")]
public class ProfessionalsController : ControllerBase
{
    private readonly IProfessionalService _service;

    public ProfessionalsController(IProfessionalService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ProfessionalResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProfessionalResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        if (result is null)
            return NotFound(new ErrorResponse("NOT_FOUND", $"Professional '{id}' not found.", Guid.NewGuid().ToString()));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProfessionalResponse>> Create([FromBody] CreateProfessionalRequest request, CancellationToken ct)
    {
        if (await _service.AwsUserIdExistsAsync(request.AwsUserId, ct))
            return Conflict(new ErrorResponse("DUPLICATE", "A professional with this AWS User ID already exists.", Guid.NewGuid().ToString()));

        if (await _service.EmailExistsAsync(request.Email, ct))
            return Conflict(new ErrorResponse("DUPLICATE", "A professional with this email already exists.", Guid.NewGuid().ToString()));

        var result = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProfessionalResponse>> Update(Guid id, [FromBody] UpdateProfessionalRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        if (result is null)
            return NotFound(new ErrorResponse("NOT_FOUND", $"Professional '{id}' not found.", Guid.NewGuid().ToString()));
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new ErrorResponse("NOT_FOUND", $"Professional '{id}' not found.", Guid.NewGuid().ToString()));
        return NoContent();
    }
}
