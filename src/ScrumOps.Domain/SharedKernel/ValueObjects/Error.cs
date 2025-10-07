using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumOps.Domain.SharedKernel.ValueObjects
{
    // =========================================
    // Error Type
    // =========================================

    public record Error(string Code, string Message)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "Value cannot be null");

        public static Error NotFound(string entity, object id)
            => new("Error.NotFound", $"{entity} with id {id} was not found");

        public static Error Validation(string message)
            => new("Error.Validation", message);

        public static Error Conflict(string message)
            => new("Error.Conflict", message);

        public static Error Unauthorized(string message = "Unauthorized access")
            => new("Error.Unauthorized", message);
    }
}
