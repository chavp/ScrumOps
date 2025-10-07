
using Microsoft.AspNetCore.Mvc;
using ScrumOps.Api.Extensions;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Api.Controllers
{
    public abstract class ApiBase : ControllerBase
    {
        public static Error UnProcessableRequest => new Error(
                "General.UnProcessableRequest", "The server could not process the request.");

        protected IActionResult BadRequest<IError>(IReadOnlyList<IError> errors)
        {
            var details = new List<string>();
            var codeErrors = new List<Error>();
            foreach (var error in errors)
            {
                if (error is Error cError)
                {
                    details.Add($"{cError.Code}:{cError.Message}");
                    codeErrors.Add(cError);
                }
            }

            var prob1 = base.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                type: "https://datatracker.ietf.org/doc/html/rfc7231#sectio-6.6.1",
                detail: string.Join(", ", details)
                );

            var problemDetails = HttpContext.CreateProblemDetails(
                title: "Bad Request",
                statusCode: StatusCodes.Status400BadRequest,
                errors: codeErrors
                );

            return new ObjectResult(problemDetails);
        }

        protected IActionResult BadRequest<IError>(IError error)
        {
            var details = new List<string>();
            var codeErrors = new List<Error>();
            if (error is Error cError)
            {
                details.Add($"{cError.Code}:{cError.Message}");
                codeErrors.Add(cError);
            }

            var prob1 = base.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                type: "https://datatracker.ietf.org/doc/html/rfc7231#sectio-6.6.1",
                detail: string.Join(", ", details)
                );

            var problemDetails = HttpContext.CreateProblemDetails(
                title: "Bad Request",
                statusCode: StatusCodes.Status400BadRequest,
                errors: codeErrors
                );

            return new ObjectResult(problemDetails);
        }

        protected new IActionResult Ok(object value) => base.Ok(value);

        protected new IActionResult NotFound() => base.NotFound();

    }
}
