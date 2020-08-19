using System;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Core.Patterns
{
    public static class EitherMixin
    {
        public static Either<IEnumerable<TError>, T> Combine<T, TError>(this Either<IEnumerable<TError>, T> ea, Either<IEnumerable<TError>, T> eb,
            Func<T, T, Either<IEnumerable<TError>, T>> map)
        {
            return ea
                .MapError(el1 => eb
                    .MapError(el1.Concat)
                    .MapSuccess(_ => Either.Error<IEnumerable<TError>, T>(el1)))
                .MapSuccess(x => eb
                    .MapSuccess(y => map(x, y))
                    .MapError(el => el));
        }
    }
}