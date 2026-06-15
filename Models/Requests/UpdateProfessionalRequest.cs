using System.ComponentModel.DataAnnotations;

namespace TeamsApi.Models.Requests;

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
