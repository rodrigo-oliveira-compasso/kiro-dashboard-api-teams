using System.ComponentModel.DataAnnotations;

namespace TeamsApi.Models.Requests;

public class CreateSquadRequest
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
