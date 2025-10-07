using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Api.Extensions;

/// <summary>
/// Extension methods for creating standardized Problem Details responses.
/// </summary>
public static class ProblemDetailsExtensions
{
    /// <summary>
    /// Creates a Problem Details response for resource not found errors.
    /// </summary>
    public static ProblemDetails NotFoundProblem(this ControllerBase controller, string resourceType, object resourceId, string? detail = null)
    {
        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Resource Not Found",
            Status = StatusCodes.Status404NotFound,
            Detail = detail ?? $"The {resourceType} with ID '{resourceId}' was not found.",
            Instance = controller.HttpContext.Request.Path
        };

        // Add tracing information if available
        var traceId = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;
        problem.Extensions["traceId"] = traceId;
        
        return problem;
    }

    /// <summary>
    /// Creates a Problem Details response for validation errors.
    /// </summary>
    public static ValidationProblemDetails ValidationProblem(this ControllerBase controller, IDictionary<string, string[]> errors, string? detail = null)
    {
        var problem = new ValidationProblemDetails(errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Validation Error",
            Status = StatusCodes.Status400BadRequest,
            Detail = detail ?? "One or more validation errors occurred.",
            Instance = controller.HttpContext.Request.Path
        };

        // Add tracing information if available
        var traceId = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;
        problem.Extensions["traceId"] = traceId;
        
        return problem;
    }

    /// <summary>
    /// Creates a Problem Details response for business rule violations.
    /// </summary>
    public static ProblemDetails BusinessRuleProblem(this ControllerBase controller, string title, string detail, string? type = null)
    {
        var problem = new ProblemDetails
        {
            Type = type ?? "https://scrumops.com/problems/business-rule-violation",
            Title = title,
            Status = StatusCodes.Status409Conflict,
            Detail = detail,
            Instance = controller.HttpContext.Request.Path
        };

        // Add tracing information if available
        var traceId = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;
        problem.Extensions["traceId"] = traceId;
        
        return problem;
    }

    /// <summary>
    /// Creates a Problem Details response for internal server errors.
    /// </summary>
    public static ProblemDetails InternalServerErrorProblem(this ControllerBase controller, string? detail = null)
    {
        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = detail ?? "An unexpected error occurred while processing your request.",
            Instance = controller.HttpContext.Request.Path
        };

        // Add tracing information if available
        var traceId = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;
        problem.Extensions["traceId"] = traceId;
        
        return problem;
    }

    /// <summary>
    /// Creates a Problem Details response for bad request errors.
    /// </summary>
    public static ProblemDetails BadRequestProblem(this ControllerBase controller, string title, string detail, string? type = null)
    {
        var problem = new ProblemDetails
        {
            Type = type ?? "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = title,
            Status = StatusCodes.Status400BadRequest,
            Detail = detail,
            Instance = controller.HttpContext.Request.Path
        };

        // Add tracing information if available
        var traceId = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;
        problem.Extensions["traceId"] = traceId;
        
        return problem;
    }

    /// <summary>
    /// Creates a Problem Details response for unauthorized access.
    /// </summary>
    public static ProblemDetails UnauthorizedProblem(this ControllerBase controller, string? detail = null)
    {
        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Title = "Unauthorized",
            Status = StatusCodes.Status401Unauthorized,
            Detail = detail ?? "Authentication is required to access this resource.",
            Instance = controller.HttpContext.Request.Path
        };

        // Add tracing information if available
        var traceId = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;
        problem.Extensions["traceId"] = traceId;
        
        return problem;
    }

    /// <summary>
    /// Creates a Problem Details response for forbidden access.
    /// </summary>
    public static ProblemDetails ForbiddenProblem(this ControllerBase controller, string? detail = null)
    {
        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            Title = "Forbidden",
            Status = StatusCodes.Status403Forbidden,
            Detail = detail ?? "You do not have permission to access this resource.",
            Instance = controller.HttpContext.Request.Path
        };

        // Add tracing information if available
        var traceId = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;
        problem.Extensions["traceId"] = traceId;
        
        return problem;
    }

    /// <summary>
    /// Creates a generic Problem Details response.
    /// </summary>
    public static ProblemDetails CreateProblem(this ControllerBase controller, int statusCode, string title, string detail, string? type = null)
    {
        var problem = new ProblemDetails
        {
            Type = type ?? GetDefaultTypeForStatusCode(statusCode),
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = controller.HttpContext.Request.Path
        };

        // Add tracing information if available
        var traceId = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;
        problem.Extensions["traceId"] = traceId;
        
        return problem;
    }

    private static string GetDefaultTypeForStatusCode(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        401 => "https://tools.ietf.org/html/rfc7235#section-3.1",
        403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        422 => "https://tools.ietf.org/html/rfc4918#section-11.2",
        500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        _ => "about:blank"
    };

    /// <summary>
    /// Converts a Result to an ActionResult, mapping success to Ok and failure to BadRequest.
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        return result.IsSuccess
            ? controller.Ok(result.Value)
            : controller.BadRequest(result.Error);
    }

    /// <summary>
    /// Converts a Maybe to an ActionResult, mapping Some to Ok and None to NotFound.
    /// </summary>
    public static IActionResult ToActionResult<T>(this Maybe<T> maybe, ControllerBase controller, string resourceType, object resourceId)
    {
        return maybe.Match<IActionResult>(
            value => controller.Ok(value),
            () => controller.NotFound(controller.NotFoundProblem(resourceType, resourceId))
        );
    }

    /// <summary>
    /// Converts a Task Result to an ActionResult asynchronously.
    /// </summary>
    public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Result<T>> resultTask, ControllerBase controller)
    {
        var result = await resultTask;
        return result.ToActionResult(controller);
    }

    /// <summary>
    /// Converts a Task Maybe to an ActionResult asynchronously.
    /// </summary>
    public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Maybe<T>> maybeTask, ControllerBase controller, string resourceType, object resourceId)
    {
        var maybe = await maybeTask;
        return maybe.ToActionResult(controller, resourceType, resourceId);
    }
}