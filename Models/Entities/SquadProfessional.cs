namespace TeamsApi.Models.Entities;

/// <summary>
/// Join table for N:N relationship between Squads and Professionals.
/// </summary>
public class SquadProfessional
{
    public Guid SquadId { get; set; }
    public Squad Squad { get; set; } = null!;

    public Guid ProfessionalId { get; set; }
    public Professional Professional { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}
