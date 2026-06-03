using System.ComponentModel.DataAnnotations;

namespace TeamsApi.Models.DTOs;

public class CreateProfessionalRequest
{
    [Required]
    [MaxLength(100)]
    public string AwsUserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
}

public class UpdateProfessionalRequest
{
    [MaxLength(100)]
    public string? AwsUserId { get; set; }

    [MaxLength(255)]
    public string? Name { get; set; }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }
}

public record ProfessionalResponse(
    Guid Id,
    string AwsUserId,
    string Name,
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<SquadBriefResponse> Squads);

public record ProfessionalBriefResponse(
    Guid Id,
    string AwsUserId,
    string Name,
    string Email);
