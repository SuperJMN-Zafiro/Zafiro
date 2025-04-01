using System.Reactive;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Navigation
{
    public class Navigator : INavigator
    {
        private readonly ITypeResolver typeResolver;
        private readonly Stack<object> navigationStack = new();
        private readonly BehaviorSubject<object?> contentSubject = new(null);
        private readonly IEnhancedCommand<Unit, Result<Unit>> back;
        private readonly BehaviorSubject<bool> canGoBackSubject = new(false);

        public Navigator(ITypeResolver typeResolver)
        {
            this.typeResolver = typeResolver;
            
            var reactiveCommand = ReactiveCommand.CreateFromTask<Unit, Result<Unit>>(
                _ => GoBack(),
                canGoBackSubject);
            
            back = EnhancedCommand.Create(reactiveCommand);
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
                return Result.Failure<Unit>(e.ToString());
            }
        }
        
        public Task<Result<Unit>> Go(Type type)
        {
            return Go(type, null);
        }
        
        public Task<Result<Unit>> Go(Type type, object? parameter)
        {
            try
            {
                // Create instance using type resolver
                object instance;
                
                if (parameter == null)
                {
                    // Simple resolve without parameter
                    instance = typeResolver.Resolve(type);
                }
                else 
                {
                    // Resolve with parameter if supported
                    if (typeResolver is ITypeWithParametersResolver parameterResolver)
                    {
                        instance = parameterResolver.Resolve(type, parameter);
                    }
                    else
                    {
                        return Task.FromResult(Result.Failure<Unit>(
                            $"The current type resolver doesn't support parameterized resolution. " +
                            $"Use a resolver that implements ITypeWithParametersResolver."));
                    }
                }
                
                // Navigate to the created instance
                return Go(() => instance);
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result.Failure<Unit>(ex.ToString()));
            }
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