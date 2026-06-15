using System.ComponentModel.DataAnnotations;

namespace TeamsApi.Models.Requests;

public class UpdateSquadRequest
{
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
