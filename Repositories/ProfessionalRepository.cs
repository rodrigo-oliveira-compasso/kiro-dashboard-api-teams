using Microsoft.EntityFrameworkCore;
using TeamsApi.Data;
using TeamsApi.Models.Entities;

namespace TeamsApi.Repositories;

public class ProfessionalRepository : IProfessionalRepository
{
    private readonly AppDbContext _context;

    public ProfessionalRepository(AppDbContext context) => _context = context;

    public async Task<List<Professional>> GetAllAsync(CancellationToken ct) =>
        await _context.Professionals
            .AsNoTracking()
            .Include(p => p.SquadProfessionals).ThenInclude(sp => sp.Squad)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

    public async Task<Professional?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _context.Professionals
            .Include(p => p.SquadProfessionals).ThenInclude(sp => sp.Squad)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Professional?> GetByAwsUserIdAsync(string awsUserId, CancellationToken ct) =>
        await _context.Professionals
            .FirstOrDefaultAsync(p => p.AwsUserId == awsUserId, ct);

    public async Task<bool> AwsUserIdExistsAsync(string awsUserId, CancellationToken ct) =>
        await _context.Professionals.AnyAsync(p => p.AwsUserId == awsUserId, ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        await _context.Professionals.AnyAsync(p => p.Email == email.ToLowerInvariant(), ct);

    public async Task<Professional> CreateAsync(Professional professional, CancellationToken ct)
    {
        _context.Professionals.Add(professional);
        await _context.SaveChangesAsync(ct);
        return professional;
    }

    public async Task<Professional> UpdateAsync(Professional professional, CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
        return professional;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var professional = await _context.Professionals.FindAsync([id], ct);
        if (professional is null) return false;
        _context.Professionals.Remove(professional);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
