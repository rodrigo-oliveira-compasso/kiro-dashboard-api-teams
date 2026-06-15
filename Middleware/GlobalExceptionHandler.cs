using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using TeamsApi.Models.Responses;

namespace TeamsApi.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString();
        _logger.LogError(exception, "Unhandled exception. CorrelationId: {CorrelationId}, Path: {Path}",
            correlationId, httpContext.Request.Path);

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(
            new ErrorResponse("INTERNAL_ERROR", "An unexpected error occurred.", correlationId), cancellationToken);
        return true;
    }
}
