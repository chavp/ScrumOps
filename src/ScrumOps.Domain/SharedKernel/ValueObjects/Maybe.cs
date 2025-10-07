using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumOps.Domain.SharedKernel.ValueObjects
{
    // =========================================
    // Maybe Type - สำหรับค่าที่อาจมีหรือไม่มี
    // =========================================

    public sealed class Maybe<T>
    {
        private readonly T? _value;
        public bool HasValue { get; }
        public bool HasNoValue => !HasValue;

        private Maybe(T? value, bool hasValue)
        {
            _value = value;
            HasValue = hasValue;
        }

        public T Value => HasValue
            ? _value!
            : throw new InvalidOperationException("Maybe has no value");

        // Factory Methods
        public static Maybe<T> Some(T value) => new(value, true);
        public static Maybe<T> None => new(default, false);
        public static Maybe<T> From(T? value) => value != null ? Some(value) : None;

        // Pattern Matching
        public TResult Match<TResult>(
            Func<T, TResult> some,
            Func<TResult> none)
        {
            return HasValue ? some(_value!) : none();
        }

        public void Match(Action<T> some, Action none)
        {
            if (HasValue)
                some(_value!);
            else
                none();
        }

        // Async Pattern Matching
        public async Task<TResult> MatchAsync<TResult>(
            Func<T, Task<TResult>> some,
            Func<Task<TResult>> none)
        {
            return HasValue ? await some(_value!) : await none();
        }

        // Convert to Result
        public Result<T> ToResult(Error error)
        {
            return HasValue
                ? Result<T>.Success(_value!)
                : Result<T>.Failure(error);
        }

        // Implicit conversion from T
        public static implicit operator Maybe<T>(T? value) => From(value);
    }
}
