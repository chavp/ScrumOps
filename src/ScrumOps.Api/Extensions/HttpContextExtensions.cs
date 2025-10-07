using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using ScrumOps.Api.Controllers;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static ProblemDetails CreateProblemDetails(this HttpContext httpContext,
            string? title = null,
            string? type = "https://datatracker.ietf.org/doc/html/rfc7231#sectio-6.6.1",
            int statusCode = 0,
            string? detail = null,
            IReadOnlyList<Error>? errors = null)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#sectio-6.6.1",
                Detail = detail,
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            problemDetails.Instance =
            $"{httpContext.Request.Method} {httpContext.Request.Path}";

            problemDetails.Extensions.TryAdd("requestId", httpContext.TraceIdentifier);

            var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            problemDetails.Extensions.TryAdd("traceId", activity?.Id);

            if (errors != null && errors.Any())
            {
                problemDetails.Extensions.TryAdd("errors", errors);
            }

            return problemDetails;
        }
    }
}
