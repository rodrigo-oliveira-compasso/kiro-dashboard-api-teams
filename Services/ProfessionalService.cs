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

    public async Task<PaginatedResponse<ProfessionalResponse>> GetPaginatedAsync(
        int page, int pageSize, string? sortKey, string? sortDirection, string? search, CancellationToken ct)
    {
        var professionals = await _repository.GetAllAsync(ct);
        var items = professionals.Select(ToResponse).ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLowerInvariant();
            items = items.Where(p =>
                p.Name.ToLowerInvariant().Contains(term) ||
                p.Email.ToLowerInvariant().Contains(term) ||
                p.AwsUserId.ToLowerInvariant().Contains(term)
            ).ToList();
        }

        if (!string.IsNullOrWhiteSpace(sortKey))
        {
            var prop = typeof(ProfessionalResponse).GetProperty(sortKey,
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

        return new PaginatedResponse<ProfessionalResponse>(paged, new PaginationMetadata(totalItems, page, pageSize, totalPages));
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
