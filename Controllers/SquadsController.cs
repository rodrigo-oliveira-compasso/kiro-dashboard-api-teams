using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamsApi.Models.DTOs;
using TeamsApi.Services;

namespace TeamsApi.Controllers;

[ApiController]
[Route("api/squads")]
[Authorize(Roles = "Admin")]
public class SquadsController : ControllerBase
{
    private readonly ISquadService _service;

    public SquadsController(ISquadService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<SquadResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SquadResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        if (result is null)
            return NotFound(new ErrorResponse("NOT_FOUND", $"Squad '{id}' not found.", Guid.NewGuid().ToString()));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SquadResponse>> Create([FromBody] CreateSquadRequest request, CancellationToken ct)
    {
        if (await _service.NameExistsAsync(request.Name, ct))
            return Conflict(new ErrorResponse("DUPLICATE", "A squad with this name already exists.", Guid.NewGuid().ToString()));

        var result = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SquadResponse>> Update(Guid id, [FromBody] UpdateSquadRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct);
        if (result is null)
            return NotFound(new ErrorResponse("NOT_FOUND", $"Squad '{id}' not found.", Guid.NewGuid().ToString()));
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound(new ErrorResponse("NOT_FOUND", $"Squad '{id}' not found.", Guid.NewGuid().ToString()));
        return NoContent();
    }

    [HttpPost("{squadId:guid}/professionals/{professionalId:guid}")]
    public async Task<ActionResult> AddProfessional(Guid squadId, Guid professionalId, CancellationToken ct)
    {
        var squad = await _service.GetByIdAsync(squadId, ct);
        if (squad is null)
            return NotFound(new ErrorResponse("NOT_FOUND", $"Squad '{squadId}' not found.", Guid.NewGuid().ToString()));

        await _service.AddProfessionalAsync(squadId, professionalId, ct);
        return NoContent();
    }

    [HttpDelete("{squadId:guid}/professionals/{professionalId:guid}")]
    public async Task<ActionResult> RemoveProfessional(Guid squadId, Guid professionalId, CancellationToken ct)
    {
        await _service.RemoveProfessionalAsync(squadId, professionalId, ct);
        return NoContent();
    }
}
