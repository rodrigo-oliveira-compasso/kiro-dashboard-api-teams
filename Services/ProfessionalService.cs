using TeamsApi.Models.Requests;
using TeamsApi.Models.Responses;
using TeamsApi.Models.Entities;
using TeamsApi.Repositories;

namespace TeamsApi.Services;

public class ProfessionalService : IProfessionalService
{
    private readonly IProfessionalRepository _repository;

    public ProfessionalService(IProfessionalRepository repository) => _repository = repository;

    public async Task<List<ProfessionalResponse>> GetAllAsync(CancellationToken ct)
    {
        var professionals = await _repository.GetAllAsync(ct);
        return professionals.Select(ToResponse).ToList();
    }

    public async Task<ProfessionalResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var professional = await _repository.GetByIdAsync(id, ct);
        return professional is null ? null : ToResponse(professional);
    }

    public async Task<ProfessionalResponse> CreateAsync(CreateProfessionalRequest request, CancellationToken ct)
    {
        var professional = new Professional
        {
            Id = Guid.NewGuid(),
            AwsUserId = request.AwsUserId.Trim(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(professional, ct);
        return ToResponse(professional);
    }

    public async Task<ProfessionalResponse?> UpdateAsync(Guid id, UpdateProfessionalRequest request, CancellationToken ct)
    {
        var professional = await _repository.GetByIdAsync(id, ct);
        if (professional is null) return null;

        if (!string.IsNullOrWhiteSpace(request.AwsUserId))
            professional.AwsUserId = request.AwsUserId.Trim();
        if (!string.IsNullOrWhiteSpace(request.Name))
            professional.Name = request.Name.Trim();
        if (!string.IsNullOrWhiteSpace(request.Email))
            professional.Email = request.Email.Trim().ToLowerInvariant();

        professional.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(professional, ct);
        return ToResponse(professional);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct) =>
        await _repository.DeleteAsync(id, ct);

    public async Task<bool> AwsUserIdExistsAsync(string awsUserId, CancellationToken ct) =>
        await _repository.AwsUserIdExistsAsync(awsUserId, ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        await _repository.EmailExistsAsync(email, ct);

    private static ProfessionalResponse ToResponse(Professional p) => new(
        Id: p.Id,
        AwsUserId: p.AwsUserId,
        Name: p.Name,
        Email: p.Email,
        CreatedAt: p.CreatedAt,
        UpdatedAt: p.UpdatedAt,
        Squads: p.SquadProfessionals.Select(sp => new SquadBriefResponse(sp.Squad.Id, sp.Squad.Name)).ToList());
}
