using InventoryManagementSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            _logger.LogWarning(
                exception,
                "The response has already started, so the global exception handler will not run.");
            return false;
        }

        var statusCode = exception switch
        {
            NotFoundException or KeyNotFoundException =>
                StatusCodes.Status404NotFound,

            BadRequestException or ArgumentException or FormatException or
                BadHttpRequestException or System.Text.Json.JsonException =>
                StatusCodes.Status400BadRequest,

            UnauthorizedException =>
                StatusCodes.Status401Unauthorized,

            ForbiddenException or UnauthorizedAccessException =>
                StatusCodes.Status403Forbidden,

            ConflictException or DbUpdateException or DbUpdateConcurrencyException =>
                StatusCodes.Status409Conflict,

            _ => StatusCodes.Status500InternalServerError
        };

        var title = statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status409Conflict => "Conflict",
            _ => "Internal Server Error"
        };

        var detail = statusCode switch
        {
            StatusCodes.Status500InternalServerError =>
                "An unexpected error occurred.",

            StatusCodes.Status409Conflict when exception is DbUpdateException =>
                "The request conflicts with existing data.",

            _ => exception.Message
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "An unhandled exception occurred.");
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Request failed with status code {StatusCode}.",
                statusCode);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}
