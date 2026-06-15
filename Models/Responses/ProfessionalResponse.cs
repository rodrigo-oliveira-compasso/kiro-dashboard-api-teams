namespace TeamsApi.Models.Responses;

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
