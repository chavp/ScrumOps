using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Domain.SharedKernel.Extensions
{
    // =========================================
    // Result Extension Methods
    // =========================================

    public static class ResultExtensions
    {
        // Bind
        public static Result<TOut> Bind<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, Result<TOut>> func)
        {
            return result.IsSuccess
                ? func(result.Value)
                : Result<TOut>.Failure(result.Error);
        }

        // BindAsync for Result<T> with async functions
        public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, Task<Result<TOut>>> func)
        {
            return result.IsSuccess
                ? await func(result.Value)
                : Result<TOut>.Failure(result.Error);
        }

        // BindAsync for Result<T> bridging to Maybe<T> 
        public static async Task<Maybe<TOut>> BindAsync<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, Task<Maybe<TOut>>> func)
        {
            return result.IsSuccess
                ? await func(result.Value)
                : Maybe<TOut>.None;
        }

        public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
            this Task<Result<TIn>> resultTask,
            Func<TIn, Task<Result<TOut>>> func)
        {
            var result = await resultTask;
            return result.IsSuccess
                ? await func(result.Value)
                : Result<TOut>.Failure(result.Error);
        }

        // BindAsync that bridges Result to Maybe
        public static async Task<Maybe<TOut>> BindAsync<TIn, TOut>(
            this Task<Result<TIn>> resultTask,
            Func<TIn, Task<Maybe<TOut>>> func)
        {
            var result = await resultTask;
            return result.IsSuccess
                ? await func(result.Value)
                : Maybe<TOut>.None;
        }

        // Map
        public static Result<TOut> Map<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, TOut> mapper)
        {
            return result.IsSuccess
                ? Result<TOut>.Success(mapper(result.Value))
                : Result<TOut>.Failure(result.Error);
        }

        public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
            this Task<Result<TIn>> resultTask,
            Func<TIn, TOut> mapper)
        {
            var result = await resultTask;
            return result.Map(mapper);
        }

        // Tap
        public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
        {
            if (result.IsSuccess)
                action(result.Value);
            return result;
        }

        // Ensure
        public static Result<T> Ensure<T>(
            this Result<T> result,
            Func<T, bool> predicate,
            Error error)
        {
            if (result.IsFailure)
                return result;

            return predicate(result.Value)
                ? result
                : Result<T>.Failure(error);
        }

        public static async Task<Result<T>> EnsureAsync<T>(
            this Task<Result<T>> resultTask,
            Func<T, bool> predicate,
            Error error)
        {
            var result = await resultTask;
            return result.Ensure(predicate, error);
        }

        // Match for Result with three outcomes (success, validation error, not found)
        public static TOut Match<TIn, TOut>(
            this Result<TIn> result,
            Func<TIn, TOut> onSuccess,
            Func<Error, TOut> onFailure)
        {
            return result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.Error);
        }

        public static async Task<TOut> MatchAsync<TIn, TOut>(
            this Task<Result<TIn>> resultTask,
            Func<TIn, TOut> onSuccess,
            Func<Error, TOut> onFailure)
        {
            var result = await resultTask;
            return result.Match(onSuccess, onFailure);
        }

        public static async Task<TOut> MatchAsync<TIn, TOut>(
            this Task<Result<TIn>> resultTask,
            Func<TIn, Task<TOut>> onSuccess,
            Func<Error, Task<TOut>> onFailure)
        {
            var result = await resultTask;
            return result.IsSuccess
                ? await onSuccess(result.Value)
                : await onFailure(result.Error);
        }
    }
}
