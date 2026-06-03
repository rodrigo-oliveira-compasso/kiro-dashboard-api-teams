using TeamsApi.Models.Entities;

namespace TeamsApi.Repositories;

public interface ISquadRepository
{
    Task<List<Squad>> GetAllAsync(CancellationToken ct);
    Task<Squad?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> NameExistsAsync(string name, CancellationToken ct);
    Task<Squad> CreateAsync(Squad squad, CancellationToken ct);
    Task<Squad> UpdateAsync(Squad squad, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task AddProfessionalAsync(Guid squadId, Guid professionalId, CancellationToken ct);
    Task RemoveProfessionalAsync(Guid squadId, Guid professionalId, CancellationToken ct);
}
