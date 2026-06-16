using TeamsApi.Models.Requests;
using TeamsApi.Models.Responses;
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

    public async Task<PaginatedResponse<SquadResponse>> GetPaginatedAsync(
        int page, int pageSize, string? sortKey, string? sortDirection, string? search, CancellationToken ct)
    {
        var squads = await _repository.GetAllAsync(ct);
        var items = squads.Select(ToResponse).ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLowerInvariant();
            items = items.Where(s =>
                s.Name.ToLowerInvariant().Contains(term) ||
                (s.Description ?? "").ToLowerInvariant().Contains(term)
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(sortKey))
        {
            var prop = typeof(SquadResponse).GetProperty(sortKey,
                System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (prop is not null)
            {
                items = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase)
                    ? items.OrderByDescending(x => prop.GetValue(x)).ToList()
                    : items.OrderBy(x => prop.GetValue(x)).ToList();
            }
        }

        var totalItems = items.Count;
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / pageSize);
        var paged = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedResponse<SquadResponse>(paged, new PaginationMetadata(totalItems, page, pageSize, totalPages));
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
