using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Zafiro.Mixins;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Navigation
{
    public class Navigator : INavigator
    {
        private readonly IEnhancedCommand<Unit, Result<Unit>> back;
        private readonly BehaviorSubject<bool> canGoBackSubject = new(false);
        private readonly BehaviorSubject<object?> contentSubject = new(null);
        private readonly Maybe<ILogger> logger;
        private readonly Stack<object> navigationStack = new();
        private readonly IServiceProvider serviceProvider;

        public Navigator(IServiceProvider serviceProvider, Maybe<ILogger> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            var reactiveCommand = ReactiveCommand.CreateFromTask<Unit, Result<Unit>>(_ => GoBack(), canGoBackSubject);
            back = reactiveCommand.Enhance();
        }

        public IObservable<object?> Content => contentSubject;

        public IEnhancedCommand<Unit, Result<Unit>> Back => back;

        public async Task<Result<Unit>> Go(Func<object> factory)
        {
            try
            {
                var instance = factory();
                navigationStack.Push(instance);
                contentSubject.OnNext(instance);
                canGoBackSubject.OnNext(navigationStack.Count > 1);

                return Result.Success(Unit.Default);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Result.Failure<Unit>(e.ToString());
            }
        }

        public Task<Result<Unit>> Go(Type type)
        {
            return Result.Try(() => serviceProvider.GetRequiredService(type))
                .Map(instance => Go(() => instance))
                .TapError(e => logger.Error(e));
        }

        public Task<Result<Unit>> GoBack()
        {
            if (navigationStack.Count <= 1)
            {
                return Task.FromResult(Result.Failure<Unit>("No previous entries in the navigation stack"));
            }

            navigationStack.Pop();

            if (navigationStack.Count > 0)
            {
                var previous = navigationStack.Peek();
                contentSubject.OnNext(previous);
            }
            else
            {
                contentSubject.OnNext(null);
            }

            canGoBackSubject.OnNext(navigationStack.Count > 1);

            return Task.FromResult(Result.Success(Unit.Default));
        }
    }
}