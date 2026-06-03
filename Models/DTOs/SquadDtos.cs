using System.ComponentModel.DataAnnotations;

namespace TeamsApi.Models.DTOs;

public class CreateSquadRequest
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateSquadRequest
{
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

public record SquadResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<ProfessionalBriefResponse> Professionals);

public record SquadBriefResponse(
    Guid Id,
    string Name);
