using System.Threading.Tasks;
using Optional.Unsafe;
using Zafiro.Core.Patterns.Either;

namespace Zafiro.Core.Patterns
{
    public static class AsyncExtensions
    {
        public static async Task<Either<TLeft, TRight>> RightTask<TLeft, TRight>(this Either<TLeft, Task<TRight>> either)
        {
            return await either.Right.Match(async task =>
            {
                var right = await task;
                return new Either<TLeft, TRight>(right);
            }, () => Task.FromResult(new Either<TLeft, TRight>(either.Left.ValueOrDefault())));
        }

        public static async Task<Either<TLeft, TRight>> RightTask<TLeft, TRight>(this Either<TLeft, Task<Either<TLeft, TRight>>> either)
        {
            return await either.Right.Match(async task => await task, () => Task.FromResult(new Either<TLeft, TRight>(either.Left.ValueOrDefault())));
        }
    }
}