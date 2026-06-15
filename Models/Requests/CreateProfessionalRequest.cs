using System.ComponentModel.DataAnnotations;

namespace TeamsApi.Models.Requests;

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
