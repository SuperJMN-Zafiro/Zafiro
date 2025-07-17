using System.Reactive.Subjects;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<string, NavigationBookmark> namedBookmarks = new();
        private readonly IServiceProvider serviceProvider;

        public Navigator(IServiceProvider serviceProvider, Maybe<ILogger> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            var reactiveCommand = ReactiveCommand.CreateFromTask<Unit, Result<Unit>>(_ => GoBack(), canGoBackSubject);
            back = reactiveCommand.Enhance();
        }

        public NavigationBookmark CreateBookmark()
        {
            return new NavigationBookmark(navigationStack.Count);
        }

        public void CreateBookmark(string name)
        {
            namedBookmarks[name] = new NavigationBookmark(navigationStack.Count);
        }

        public IObservable<object?> Content => contentSubject;

        public IEnhancedCommand<Unit, Result<Unit>> Back => back;

        public Task<Result<Unit>> Go(Func<object> factory)
        {
            return Task.FromResult(
                Result.Try(factory)
                    .Bind(instance => NavigateToInstance(instance))
                    .TapError(error => logger.Error(error, "Navigation error - factory execution failed"))
            );
        }

        public Task<Result<Unit>> Go(Type type)
        {
            return Task.FromResult(
                Result.Try(() => serviceProvider.GetRequiredService(type))
                    .Bind(instance => NavigateToInstance(instance))
                    .TapError(error => logger.Error(error, "Navigation error - service resolution failed for type {Type}", type.Name))
            );
        }

        private Result<Unit> NavigateToInstance(object instance)
        {
            try
            {
                navigationStack.Push(instance);
                contentSubject.OnNext(instance);
                canGoBackSubject.OnNext(navigationStack.Count > 1);
                return Result.Success(Unit.Default);
            }
            catch (Exception e)
            {
                // This should be very rare since we're just updating internal state
                logger.Error(e, "Navigation error - failed to update navigation state");
                return Result.Failure<Unit>($"Failed to update navigation state: {e.Message}");
            }
        }

        public Task<Result<Unit>> GoBack()
        {
            return Task.FromResult(
                Result.Try(() => ExecuteGoBack())
                    .TapError(error => logger.Error(error, "Navigation error - failed to go back"))
            );
        }

        public Task<Result<Unit>> GoBackTo(NavigationBookmark bookmark)
        {
            return Task.FromResult(
                Result.Try(() => ExecuteGoBackTo(bookmark))
                    .TapError(error => logger.Error(error, "Navigation error - failed to go back to bookmark"))
            );
        }

        public Task<Result<Unit>> GoBackTo(string name)
        {
            return Task.FromResult(
                Result.Try(() =>
                    {
                        if (!namedBookmarks.TryGetValue(name, out var bookmark))
                        {
                            throw new InvalidOperationException($"Bookmark '{name}' not found");
                        }

                        var result = ExecuteGoBackTo(bookmark);
                        namedBookmarks.Remove(name);
                        return result;
                    })
                    .TapError(error => logger.Error(error, "Navigation error - failed to go back to named bookmark {Name}", name))
            );
        }

        private Unit ExecuteGoBack()
        {
            if (navigationStack.Count <= 1)
            {
                throw new InvalidOperationException("No previous entries in the navigation stack");
            }

            navigationStack.Pop();
            RemoveBookmarksAfter(navigationStack.Count);

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

            return Unit.Default;
        }

        private Unit ExecuteGoBackTo(NavigationBookmark bookmark)
        {
            if (bookmark.Index < 0 || bookmark.Index > navigationStack.Count)
            {
                throw new InvalidOperationException("Bookmark out of range");
            }

            while (navigationStack.Count > bookmark.Index)
            {
                navigationStack.Pop();
            }

            RemoveBookmarksAfter(navigationStack.Count);

            var content = navigationStack.Count > 0 ? navigationStack.Peek() : null;
            contentSubject.OnNext(content);
            canGoBackSubject.OnNext(navigationStack.Count > 1);
            return Unit.Default;
        }

        private void RemoveBookmarksAfter(int index)
        {
            var keysToRemove = namedBookmarks.Where(kv => kv.Value.Index > index).Select(kv => kv.Key).ToList();
            foreach (var key in keysToRemove)
            {
                namedBookmarks.Remove(key);
            }
        }
    }
}