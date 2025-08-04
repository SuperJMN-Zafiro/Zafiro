using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim;

public sealed class SlimWizard<TResult> : ReactiveObject, ISlimWizard<TResult>
{
    private sealed record WizardState(int CurrentIndex, ImmutableStack<object> History, bool Finished, TResult FinalResult);
    private abstract record Intent;
    private sealed record NextIntent(object Value) : Intent;
    private sealed record BackIntent : Intent;

    private readonly IObservable<WizardState> state;
    private readonly IObservable<(int Index, IWizardStep Step)> currentStepObservable;
    private readonly IObservable<Page> currentTypedPageObservable;
    private readonly IObservable<bool> hasFinished;

    private readonly ObservableAsPropertyHelper<(int Index, IWizardStep Step)> currentStepHelper;
    private readonly ObservableAsPropertyHelper<int> currentStepIndexHelper;
    private readonly ObservableAsPropertyHelper<Page> currentTypedPageHelper;
    private readonly ObservableAsPropertyHelper<IPage> currentPageHelper;
    private readonly ObservableAsPropertyHelper<IEnhancedCommand<Result<object>>> typedNextHelper;
    private readonly ObservableAsPropertyHelper<IEnhancedCommand> nextHelper;

    public SlimWizard(IList<IWizardStep> steps)
    {
        EnsureValidSteps(steps);
        TotalPages = steps.Count;

        var intents = Subject.Synchronize(new Subject<Intent>());

        var initial = new WizardState(0, ImmutableStack<object>.Empty, false, default!);
        state = intents
            .ObserveOn(RxApp.MainThreadScheduler)
            .Scan(initial, (s, intent) => intent switch
            {
                NextIntent ni when !s.Finished => s.CurrentIndex == TotalPages - 1
                    ? s with { Finished = true, FinalResult = (TResult)ni.Value }
                    : s with
                    {
                        CurrentIndex = s.CurrentIndex + 1,
                        History = s.History.Push(ni.Value)
                    },
                BackIntent when s.History.TryPeek(out _) => s with
                {
                    CurrentIndex = s.CurrentIndex - 1,
                    History = s.History.Pop()
                },
                _ => s
            })
            .StartWith(initial)
            .Replay(1)
            .RefCount();

    hasFinished = state.Select(s => s.Finished).StartWith(false);

        currentStepObservable = state.Select(s =>
        {
            var step = steps[s.CurrentIndex];
            if (step is null)
            {
                throw new InvalidOperationException($"Wizard step at index {s.CurrentIndex} is null.");
            }
            return (s.CurrentIndex, step);
        });

        currentTypedPageObservable = state.Select(s =>
        {
            var step = steps[s.CurrentIndex];
            if (step is null)
            {
                throw new InvalidOperationException($"Wizard step at index {s.CurrentIndex} is null.");
            }
            var param = s.History.TryPeek(out var p) ? p : null;
            var pageInstance = step.CreatePage(param) ??
                               throw new InvalidOperationException($"Wizard step at index {s.CurrentIndex} returned a null page.");
            var nextCommand = CreateNextCommand((s.CurrentIndex, step), pageInstance, hasFinished);
            return new Page(s.CurrentIndex, pageInstance, nextCommand, step.Title);
        })
        .Replay(1)
        .RefCount();

        currentStepObservable.ToProperty(this, x => x.CurrentStep, out currentStepHelper);
        currentStepObservable.Select(x => x.Index).ToProperty(this, x => x.CurrentStepIndex, out currentStepIndexHelper);

        currentTypedPageObservable.ToProperty(this, x => x.CurrentTypedPage, out currentTypedPageHelper);
        currentTypedPageObservable.Select(p => (IPage)p).ToProperty(this, x => x.CurrentPage, out currentPageHelper);
        currentTypedPageObservable.Select(p => p.NextCommand).ToProperty(this, x => x.TypedNext, out typedNextHelper);

        currentTypedPageObservable
            .Select(p => new CommandAdapter<Result<object>, Unit>(p.NextCommand, _ => Unit.Default))
            .ToProperty(this, x => x.Next, out nextHelper);

        currentTypedPageObservable
            .Select(p => p.NextCommand)
            .Switch()
            .Successes()
            .Subscribe(value => intents.OnNext(new NextIntent(value)));

        var canGoBack = currentStepObservable
            .CombineLatest(hasFinished, (step, finished) =>
            {
                bool validIndex = step.Index > 0 && !(step.Index == TotalPages - 1 && step.Step.Kind == StepKind.Completion);
                return validIndex && !finished;
            });

        Back = ReactiveCommand.Create(() => intents.OnNext(new BackIntent()), canGoBack).Enhance();

        Finished = state
            .Where(s => s.Finished)
            .Select(s => s.FinalResult)
            .Take(1);
    }

    public IObservable<TResult> Finished { get; }
    public IEnhancedCommand Back { get; }
    public int TotalPages { get; }

    public (int Index, IWizardStep Step) CurrentStep => currentStepHelper.Value;
    public int CurrentStepIndex => currentStepIndexHelper.Value;
    public Page CurrentTypedPage => currentTypedPageHelper.Value;
    public IPage CurrentPage => currentPageHelper.Value;
    public IEnhancedCommand<Result<object>> TypedNext => typedNextHelper.Value;
    public IEnhancedCommand Next => nextHelper.Value;

    private static void EnsureValidSteps(IList<IWizardStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);
        if (steps.Count == 0)
        {
            throw new ArgumentException("steps must contain at least one element.", nameof(steps));
        }
        for (int i = 0; i < steps.Count; i++)
        {
            if (steps[i] is null)
            {
                throw new ArgumentException($"steps[{i}] is null.", nameof(steps));
            }
        }
    }

    private static IEnhancedCommand<Result<object>> CreateNextCommand((int Index, IWizardStep Step) step, object page, IObservable<bool> hasFinished)
    {
        var command = step.Step.GetNextCommand(page) ??
                      throw new InvalidOperationException($"Wizard step at index {step.Index} returned a null Next command.");
        var canExecute = hasFinished.CombineLatest(((IReactiveCommand)command).CanExecute, (finished, canExec) => !finished && canExec);
        return command.Enhance(canExecute: canExecute);
    }
}

public static class ImmutableStackExtensions
{
    public static bool TryPeek<T>(this ImmutableStack<T> stack, out T value)
    {
        if (stack.IsEmpty)
        {
            value = default!;
            return false;
        }

        value = stack.Peek();
        return true;
    }
}
