using Microsoft.EntityFrameworkCore;
using TeamsApi.Models.Entities;

namespace TeamsApi.Repositories;

public class SquadRepository : ISquadRepository
{
    private readonly AppDbContext _context;

    public SquadRepository(AppDbContext context) => _context = context;

    public async Task<List<Squad>> GetAllAsync(CancellationToken ct) =>
        await _context.Squads
            .Include(s => s.SquadProfessionals).ThenInclude(sp => sp.Professional)
            .OrderBy(s => s.Name)
            .ToListAsync(ct);

    public async Task<Squad?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _context.Squads
            .Include(s => s.SquadProfessionals).ThenInclude(sp => sp.Professional)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<bool> NameExistsAsync(string name, CancellationToken ct) =>
        await _context.Squads.AnyAsync(s => s.Name == name, ct);

    public async Task<Squad> CreateAsync(Squad squad, CancellationToken ct)
    {
        _context.Squads.Add(squad);
        await _context.SaveChangesAsync(ct);
        return squad;
    }

    public async Task<Squad> UpdateAsync(Squad squad, CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
        return squad;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var squad = await _context.Squads.FindAsync([id], ct);
        if (squad is null) return false;
        _context.Squads.Remove(squad);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task AddProfessionalAsync(Guid squadId, Guid professionalId, CancellationToken ct)
    {
        var exists = await _context.SquadProfessionals
            .AnyAsync(sp => sp.SquadId == squadId && sp.ProfessionalId == professionalId, ct);
        if (exists) return;

        _context.SquadProfessionals.Add(new SquadProfessional
        {
            SquadId = squadId,
            ProfessionalId = professionalId,
            AssignedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveProfessionalAsync(Guid squadId, Guid professionalId, CancellationToken ct)
    {
        var entry = await _context.SquadProfessionals
            .FirstOrDefaultAsync(sp => sp.SquadId == squadId && sp.ProfessionalId == professionalId, ct);
        if (entry is not null)
        {
            _context.SquadProfessionals.Remove(entry);
            await _context.SaveChangesAsync(ct);
        }
    }
}
