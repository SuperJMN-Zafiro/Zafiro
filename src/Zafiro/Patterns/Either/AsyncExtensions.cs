using System.Threading.Tasks;
using Optional.Unsafe;

namespace Zafiro.Zafiro.Patterns.Either
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

        public static async Task<Either<TLeft, TRight>> LeftTask<TLeft, TRight>(this Either<Task<TLeft>, TRight> either)
        {
            return await either.Left.Match(async task =>
            {
                var left = await task;
                return new Either<TLeft, TRight>(left);
            }, () => Task.FromResult(new Either<TLeft, TRight>(either.Right.ValueOrDefault())));
        }

        public static async Task<Either<TLeft, TRight>> LeftTask<TLeft, TRight>(this Either<Task<Either<TLeft, TRight>>, TRight> either)
        {
            return await either.Left.Match(async task => await task, () =>
            {
                var valueOrDefault = either.Right.ValueOrDefault();
                return Task.FromResult(new Either<TLeft, TRight>(valueOrDefault));
            });
        }
    }
}