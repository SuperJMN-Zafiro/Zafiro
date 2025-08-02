using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards.Slim;

public partial class SlimWizard<TResult> : ReactiveObject, ISlimWizard<TResult>
{
    private readonly ReplaySubject<TResult> finishedSubject = new();
    private readonly Stack<object> previousValues = new();
    [ObservableAsProperty] private IPage currentPage = default!;
    [ObservableAsProperty] private (int Index, IWizardStep Step) currentStep;
    [Reactive] private int currentStepIndex;
    [ObservableAsProperty] private Page currentTypedPage = default!;
    [ObservableAsProperty] private IEnhancedCommand next = default!;
    [ObservableAsProperty] private IEnhancedCommand<Result<object>> typedNext = default!;

    public SlimWizard(IList<IWizardStep> steps)
    {
        EnsureValidSteps(steps);

        TotalPages = steps.Count;

        currentStepHelper = this.WhenAnyValue(x => x.CurrentStepIndex)
            .Select(index =>
            {
                var step = steps[index];
                if (step is null)
                    throw new InvalidOperationException($"Wizard step at index {index} is null.");
                return (index, step);
            })
            .ToProperty(this, w => w.CurrentStep);

        var hasFinished = finishedSubject.Any().StartWith(false);

        currentTypedPageHelper = this.WhenAnyValue(x => x.CurrentStep)
            .Select(step =>
            {
                var param = previousValues.TryPeek(out var p) ? p : null;
                var page = step.Step.CreatePage(param) ??
                           throw new InvalidOperationException($"Wizard step at index {step.Index} returned a null page.");
                var finalNext = CreateNextCommand(step, page, hasFinished);
                return new Page(step.Index, page, finalNext, step.Step.Title);
            })
            .ToProperty(this, w => w.CurrentTypedPage);

        typedNextHelper = this.WhenAnyValue(x => x.CurrentTypedPage.NextCommand)
            .ToProperty(this, x => x.TypedNext);

        this.WhenAnyValue(x => x.TypedNext)
            .Switch()
            .Successes()
            .Subscribe(value =>
            {
                if (CurrentStepIndex == TotalPages - 1)
                {
                    finishedSubject.OnNext((TResult)value);
                }
                else
                {
                    previousValues.Push(value);
                    CurrentStepIndex++;
                }
            });

        var canGoBack = this.WhenAnyValue(wizard => wizard.CurrentStep, s =>
            {
                if (s.Index == TotalPages - 1)
                {
                    return s.Step.Kind != StepKind.Completion;
                }

                return s.Index > 0;
            })
            .CombineLatest(hasFinished, (validIndex, finished) => validIndex && !finished);

        Back = ReactiveCommand.Create(() =>
            {
                previousValues.Pop();
                CurrentStepIndex--;
            }, canGoBack)
            .Enhance();

        nextHelper = this.WhenAnyValue(x => x.TypedNext, command => new CommandAdapter<Result<object>, Unit>(command, _ => Unit.Default))
            .ToProperty<SlimWizard<TResult>, IEnhancedCommand>(this, x => x.Next);

        currentPageHelper = this.WhenAnyValue(x => x.CurrentTypedPage)
            .ToProperty(this, wizard => wizard.CurrentPage);
    }

    public IObservable<TResult> Finished => finishedSubject.AsObservable();
    public IEnhancedCommand Back { get; }
    public int TotalPages { get; }

    private static void EnsureValidSteps(IList<IWizardStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);
        if (steps.Count == 0)
            throw new ArgumentException("steps must contain at least one element.", nameof(steps));
        for (int i = 0; i < steps.Count; i++)
            if (steps[i] is null)
                throw new ArgumentException($"steps[{i}] is null.", nameof(steps));
    }

    private static EnhancedCommand<Result<object>> CreateNextCommand((int Index, IWizardStep Step) step, object page, IObservable<bool> hasFinished)
    {
        var command = step.Step.GetNextCommand(page) ??
                      throw new InvalidOperationException($"Wizard step at index {step.Index} returned a null Next command.");
        var canExecute = hasFinished.CombineLatest(((IReactiveCommand)command).CanExecute, (finished, canExecute) => !finished && canExecute);
        return command.Enhance(canExecute: canExecute);
    }
}