using TeamsApi.Models.Requests;
using TeamsApi.Models.Responses;

namespace TeamsApi.Services;

public interface IProfessionalService
{
    Task<List<ProfessionalResponse>> GetAllAsync(CancellationToken ct);
    Task<ProfessionalResponse?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<ProfessionalResponse> CreateAsync(CreateProfessionalRequest request, CancellationToken ct);
    Task<ProfessionalResponse?> UpdateAsync(Guid id, UpdateProfessionalRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> AwsUserIdExistsAsync(string awsUserId, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
}
