using TeamsApi.Models.Entities;

namespace TeamsApi.Repositories;

public interface IProfessionalRepository
{
    Task<List<Professional>> GetAllAsync(CancellationToken ct);
    Task<Professional?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Professional?> GetByAwsUserIdAsync(string awsUserId, CancellationToken ct);
    Task<bool> AwsUserIdExistsAsync(string awsUserId, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task<Professional> CreateAsync(Professional professional, CancellationToken ct);
    Task<Professional> UpdateAsync(Professional professional, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}
