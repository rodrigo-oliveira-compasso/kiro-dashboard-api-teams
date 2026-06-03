using System.ComponentModel.DataAnnotations;

namespace TeamsApi.Models.Entities;

public class Squad
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public List<SquadProfessional> SquadProfessionals { get; set; } = [];
}
