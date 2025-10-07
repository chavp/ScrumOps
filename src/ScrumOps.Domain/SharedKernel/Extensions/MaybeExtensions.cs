using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Domain.SharedKernel.Extensions
{
    // =========================================
    // Maybe Extension Methods
    // =========================================

    public static class MaybeExtensions
    {
        // Bind: เชื่อมต่อ operations ที่ return Maybe
        public static Maybe<TOut> Bind<TIn, TOut>(
            this Maybe<TIn> maybe,
            Func<TIn, Maybe<TOut>> func)
        {
            return maybe.HasValue
                ? func(maybe.Value)
                : Maybe<TOut>.None;
        }

        // Async Bind
        public static async Task<Maybe<TOut>> BindAsync<TIn, TOut>(
            this Maybe<TIn> maybe,
            Func<TIn, Task<Maybe<TOut>>> func)
        {
            return maybe.HasValue
                ? await func(maybe.Value)
                : Maybe<TOut>.None;
        }

        public static async Task<Maybe<TOut>> BindAsync<TIn, TOut>(
            this Task<Maybe<TIn>> maybeTask,
            Func<TIn, Task<Maybe<TOut>>> func)
        {
            var maybe = await maybeTask;
            return maybe.HasValue
                ? await func(maybe.Value)
                : Maybe<TOut>.None;
        }

        // Map: แปลง value
        public static Maybe<TOut> Map<TIn, TOut>(
            this Maybe<TIn> maybe,
            Func<TIn, TOut> mapper)
        {
            return maybe.HasValue
                ? Maybe<TOut>.Some(mapper(maybe.Value))
                : Maybe<TOut>.None;
        }

        public static async Task<Maybe<TOut>> MapAsync<TIn, TOut>(
            this Task<Maybe<TIn>> maybeTask,
            Func<TIn, TOut> mapper)
        {
            var maybe = await maybeTask;
            return maybe.Map(mapper);
        }

        // Where: Filter
        public static Maybe<T> Where<T>(
            this Maybe<T> maybe,
            Func<T, bool> predicate)
        {
            return maybe.HasValue && predicate(maybe.Value)
                ? maybe
                : Maybe<T>.None;
        }

        // GetValueOrDefault
        public static T GetValueOrDefault<T>(this Maybe<T> maybe, T defaultValue)
        {
            return maybe.HasValue ? maybe.Value : defaultValue;
        }

        public static T GetValueOrDefault<T>(this Maybe<T> maybe, Func<T> defaultValueFactory)
        {
            return maybe.HasValue ? maybe.Value : defaultValueFactory();
        }

        // OrElse: ลอง Maybe ตัวอื่นถ้าไม่มีค่า
        public static Maybe<T> OrElse<T>(this Maybe<T> maybe, Maybe<T> alternative)
        {
            return maybe.HasValue ? maybe : alternative;
        }

        public static Maybe<T> OrElse<T>(this Maybe<T> maybe, Func<Maybe<T>> alternativeFactory)
        {
            return maybe.HasValue ? maybe : alternativeFactory();
        }

        // Tap: Side effect
        public static Maybe<T> Tap<T>(this Maybe<T> maybe, Action<T> action)
        {
            if (maybe.HasValue)
                action(maybe.Value);
            return maybe;
        }

        public static async Task<Maybe<T>> TapAsync<T>(
            this Task<Maybe<T>> maybeTask,
            Action<T> action)
        {
            var maybe = await maybeTask;
            return maybe.Tap(action);
        }

        // Async Pattern Matching for Task<Maybe<T>>
        public static async Task<TResult> MatchAsync<T, TResult>(
            this Task<Maybe<T>> maybeTask,
            Func<T, TResult> some,
            Func<TResult> none)
        {
            var maybe = await maybeTask;
            return maybe.Match(some, none);
        }

        public static async Task<TResult> MatchAsync<T, TResult>(
            this Task<Maybe<T>> maybeTask,
            Func<T, Task<TResult>> some,
            Func<Task<TResult>> none)
        {
            var maybe = await maybeTask;
            return await maybe.MatchAsync(some, none);
        }

        // Do: ทำงานเมื่อมีค่า
        public static Maybe<T> Do<T>(this Maybe<T> maybe, Action<T> action)
        {
            if (maybe.HasValue)
                action(maybe.Value);
            return maybe;
        }

        // DoWhenNone: ทำงานเมื่อไม่มีค่า
        public static Maybe<T> DoWhenNone<T>(this Maybe<T> maybe, Action action)
        {
            if (maybe.HasNoValue)
                action();
            return maybe;
        }

        // ToList: แปลง Maybe เป็น List
        public static List<T> ToList<T>(this Maybe<T> maybe)
        {
            return maybe.HasValue ? new List<T> { maybe.Value } : new List<T>();
        }

        // ToEnumerable
        public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe)
        {
            if (maybe.HasValue)
                yield return maybe.Value;
        }
    }
}
