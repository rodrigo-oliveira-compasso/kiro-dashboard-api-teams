using System.ComponentModel.DataAnnotations;

namespace TeamsApi.Models.Entities;

public class Professional
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The AWS Identity Store UserId (GUID from Kiro CSV reports)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string AwsUserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public List<SquadProfessional> SquadProfessionals { get; set; } = [];
}
