namespace TeamsApi.Models.DTOs;

public record ErrorResponse(
    string ErrorCode,
    string Message,
    string CorrelationId);
