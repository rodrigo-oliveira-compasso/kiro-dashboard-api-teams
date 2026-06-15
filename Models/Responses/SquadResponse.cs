namespace TeamsApi.Models.Responses;

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
