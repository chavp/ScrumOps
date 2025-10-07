
using Microsoft.AspNetCore.SignalR.Protocol;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Api.Controllers
{
    public static class Errors
    {
        public record RequiredError : Error
        {
            public string PropertyName { get; }

            public RequiredError(string propertyName, string message, string errorCode)
                : base(errorCode, message)
            {
                PropertyName = propertyName;
            }
        }

        public record RangeError : Error
        {
            public string PropertyName1 { get; }
            public string PropertyName2 { get; }

            public RangeError(string propertyName1, string propertyName2, string message, string errorCode)
                : base(errorCode, message)
            {
                PropertyName1 = propertyName1;
                PropertyName2 = propertyName2;
            }
        }

    }
}
