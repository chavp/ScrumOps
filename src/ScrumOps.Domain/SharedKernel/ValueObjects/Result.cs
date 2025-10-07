using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScrumOps.Domain.SharedKernel.ValueObjects
{
    // =========================================
    // Result Type - สำหรับ Success/Failure
    // =========================================

    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public Error Error { get; }

        private Result(bool isSuccess, T value, Error error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new(true, value, Error.None);
        public static Result<T> Failure(Error error) => new(false, default!, error);

        // Pattern Matching
        public TResult Match<TResult>(
            Func<T, TResult> onSuccess,
            Func<Error, TResult> onFailure)
        {
            return IsSuccess ? onSuccess(Value) : onFailure(Error);
        }

        // Async Pattern Matching
        public async Task<TResult> MatchAsync<TResult>(
            Func<T, Task<TResult>> onSuccess,
            Func<Error, Task<TResult>> onFailure)
        {
            return IsSuccess ? await onSuccess(Value) : await onFailure(Error);
        }

        // Convert to Maybe
        public Maybe<T> ToMaybe()
        {
            return IsSuccess ? Maybe<T>.Some(Value) : Maybe<T>.None;
        }
    }
}
