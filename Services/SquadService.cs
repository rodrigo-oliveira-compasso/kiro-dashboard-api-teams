using TeamsApi.Models.DTOs;
using TeamsApi.Models.Entities;
using TeamsApi.Repositories;

namespace TeamsApi.Services;

public class SquadService : ISquadService
{
    private readonly ISquadRepository _repository;

    public SquadService(ISquadRepository repository) => _repository = repository;

    public async Task<List<SquadResponse>> GetAllAsync(CancellationToken ct)
    {
        var squads = await _repository.GetAllAsync(ct);
        return squads.Select(ToResponse).ToList();
    }

    public async Task<SquadResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var squad = await _repository.GetByIdAsync(id, ct);
        return squad is null ? null : ToResponse(squad);
    }

    public async Task<SquadResponse> CreateAsync(CreateSquadRequest request, CancellationToken ct)
    {
        var squad = new Squad
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(squad, ct);
        return ToResponse(squad);
    }

    public async Task<SquadResponse?> UpdateAsync(Guid id, UpdateSquadRequest request, CancellationToken ct)
    {
        var squad = await _repository.GetByIdAsync(id, ct);
        if (squad is null) return null;

        if (!string.IsNullOrWhiteSpace(request.Name))
            squad.Name = request.Name.Trim();
        if (request.Description is not null)
            squad.Description = request.Description.Trim();

        squad.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(squad, ct);
        return ToResponse(squad);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct) =>
        await _repository.DeleteAsync(id, ct);

    public async Task<bool> NameExistsAsync(string name, CancellationToken ct) =>
        await _repository.NameExistsAsync(name, ct);

    public async Task AddProfessionalAsync(Guid squadId, Guid professionalId, CancellationToken ct) =>
        await _repository.AddProfessionalAsync(squadId, professionalId, ct);

    public async Task RemoveProfessionalAsync(Guid squadId, Guid professionalId, CancellationToken ct) =>
        await _repository.RemoveProfessionalAsync(squadId, professionalId, ct);

    private static SquadResponse ToResponse(Squad s) => new(
        Id: s.Id,
        Name: s.Name,
        Description: s.Description,
        CreatedAt: s.CreatedAt,
        UpdatedAt: s.UpdatedAt,
        Professionals: s.SquadProfessionals.Select(sp => new ProfessionalBriefResponse(
            sp.Professional.Id, sp.Professional.AwsUserId, sp.Professional.Name, sp.Professional.Email)).ToList());
}
