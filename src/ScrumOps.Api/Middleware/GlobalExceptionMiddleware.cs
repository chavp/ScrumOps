using Microsoft.AspNetCore.Mvc;
using ScrumOps.Domain.SharedKernel.Exceptions;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace ScrumOps.Api.Middleware;

/// <summary>
/// Global exception handling middleware that converts exceptions to Problem Details responses.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request processing");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = CreateProblemDetails(context, exception);

        var response = context.Response;
        response.ContentType = "application/problem+json";
        response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problemDetails, JsonOptions);
        await response.WriteAsync(json);
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        return exception switch
        {
            DomainException domainEx => new ProblemDetails
            {
                Type = "https://scrumops.com/problems/domain-error",
                Title = "Domain Rule Violation",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = domainEx.Message,
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            },

            ArgumentNullException nullEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Missing Required Parameter",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = $"The parameter '{nullEx.ParamName}' is required but was not provided.",
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            },

            ArgumentException argEx => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Invalid Argument",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = argEx.Message,
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            },

            InvalidOperationException invalidOpEx when invalidOpEx.Message.Contains("not found") => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Resource Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = invalidOpEx.Message,
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            },

            InvalidOperationException invalidOpEx => new ProblemDetails
            {
                Type = "https://scrumops.com/problems/business-rule-violation",
                Title = "Business Rule Violation",
                Status = (int)HttpStatusCode.Conflict,
                Detail = invalidOpEx.Message,
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            },

            UnauthorizedAccessException => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Unauthorized",
                Status = (int)HttpStatusCode.Unauthorized,
                Detail = "Authentication is required to access this resource.",
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            },

            NotImplementedException => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.2",
                Title = "Not Implemented",
                Status = (int)HttpStatusCode.NotImplemented,
                Detail = "This functionality is not yet implemented.",
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            },

            TimeoutException => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.5",
                Title = "Request Timeout",
                Status = (int)HttpStatusCode.RequestTimeout,
                Detail = "The request timed out while processing.",
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId }
            },

            _ => new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred while processing your request.",
                Instance = context.Request.Path,
                Extensions = 
                {
                    ["traceId"] = traceId,
                    ["exceptionType"] = exception.GetType().Name
                }
            }
        };
    }
}