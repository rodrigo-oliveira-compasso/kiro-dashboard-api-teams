using TeamsApi.Models.Requests;
using TeamsApi.Models.Responses;

namespace TeamsApi.Services;

public interface ISquadService
{
    Task<List<SquadResponse>> GetAllAsync(CancellationToken ct);
    Task<PaginatedResponse<SquadResponse>> GetPaginatedAsync(int page, int pageSize, string? sortKey, string? sortDirection, string? search, CancellationToken ct);
    Task<SquadResponse?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<SquadResponse> CreateAsync(CreateSquadRequest request, CancellationToken ct);
    Task<SquadResponse?> UpdateAsync(Guid id, UpdateSquadRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> NameExistsAsync(string name, CancellationToken ct);
    Task AddProfessionalAsync(Guid squadId, Guid professionalId, CancellationToken ct);
    Task RemoveProfessionalAsync(Guid squadId, Guid professionalId, CancellationToken ct);
}
