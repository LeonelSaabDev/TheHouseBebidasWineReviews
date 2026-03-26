using Microsoft.AspNetCore.Mvc;

namespace TheHouseBebidas.WineReviews.Api.Middleware;

public sealed class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail, logLevel) = exception switch
        {
            ArgumentException argumentException =>
                (StatusCodes.Status400BadRequest, "Bad Request", argumentException.Message, LogLevel.Warning),
            KeyNotFoundException =>
                (StatusCodes.Status404NotFound, "Not Found", "The requested resource was not found.", LogLevel.Warning),
            UnauthorizedAccessException unauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized, "Unauthorized", unauthorizedAccessException.Message, LogLevel.Warning),
            OperationCanceledException when context.RequestAborted.IsCancellationRequested =>
                (499, "Client Closed Request", "The request was canceled by the client.", LogLevel.Information),
            OperationCanceledException =>
                (StatusCodes.Status408RequestTimeout, "Request Timeout", "The operation timed out.", LogLevel.Warning),
            _ =>
                (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.", LogLevel.Error)
        };

        if (logLevel == LogLevel.Error)
        {
            _logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else if (logLevel == LogLevel.Information)
        {
            _logger.LogInformation("Request canceled by client for {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
        }

        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (context.RequestAborted.IsCancellationRequested)
        {
            return;
        }

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: context.RequestAborted);
    }
}
