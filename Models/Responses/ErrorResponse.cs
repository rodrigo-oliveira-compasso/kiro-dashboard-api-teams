namespace TeamsApi.Models.Responses;

public record ErrorResponse(
    string ErrorCode,
    string Message,
    string CorrelationId);
