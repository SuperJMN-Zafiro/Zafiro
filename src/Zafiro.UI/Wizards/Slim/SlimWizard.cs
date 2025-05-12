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
    [ObservableAsProperty] private IPage currentPage;
    [ObservableAsProperty] private (int Index, IWizardStep Step) currentStep;
    [Reactive] private int currentStepIndex;
    [ObservableAsProperty] private Page currentTypedPage;
    [ObservableAsProperty] private IEnhancedCommand next;
    [ObservableAsProperty] private IEnhancedCommand<Result<object>> typedNext;

    public SlimWizard(IList<IWizardStep> steps)
    {
        TotalPages = steps.Count;

        currentStepHelper = this.WhenAnyValue(w => w.CurrentStepIndex)
            .Select(index => (index, steps[index]))
            .ToProperty(this, w => w.CurrentStep);

        var hasFinished = finishedSubject.Any().StartWith(false);

        currentTypedPageHelper = this.WhenAnyValue<SlimWizard<TResult>, (int Id, IWizardStep Step)>(w => w.CurrentStep)
            .Select(step =>
            {
                var param = previousValues.TryPeek(out var p) ? p : null;
                var page = step.Step.CreatePage(param);
                var finalNext = CreateNextCommand(step, page, hasFinished);
                var value = new Page(step.Id, page, finalNext, step.Step.Title);
                return value;
            })
            .ToProperty(this, w => w.CurrentTypedPage);

        typedNextHelper = this.WhenAnyValue<SlimWizard<TResult>, IEnhancedCommand<Result<object>>>(wizard => wizard.CurrentTypedPage.NextCommand!)
            .Select(command => command)
            .ToProperty(this, x => x.TypedNext);

        this.WhenAnyValue<SlimWizard<TResult>, IEnhancedCommand<Result<object>>>(wizard => wizard.TypedNext)
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

        nextHelper = this.WhenAnyValue<SlimWizard<TResult>, CommandAdapter<Result<object>, Unit>, IEnhancedCommand<Result<object>>>(x => x.TypedNext, command => new CommandAdapter<Result<object>, Unit>(command, _ => Unit.Default))
            .ToProperty<SlimWizard<TResult>, IEnhancedCommand>(this, x => x.Next);

        currentPageHelper = this.WhenAnyValue<SlimWizard<TResult>, IPage, Page>(x => x.CurrentTypedPage, page => page).ToProperty(this, wizard => wizard.CurrentPage);
    }

    public IObservable<TResult> Finished => finishedSubject.AsObservable();
    public IEnhancedCommand Back { get; }
    public int TotalPages { get; }

    private static EnhancedCommand<Result<object>> CreateNextCommand((int, IWizardStep) step, object page, IObservable<bool> hasFinished)
    {
        var command = step.Item2.GetNextCommand(page);
        var canExecute = hasFinished.CombineLatest(((IReactiveCommand)command).CanExecute, (finished, canExecute) => !finished && canExecute);
        var finalCommand = command.Enhance(canExecute: canExecute);
        var finalNext = finalCommand;
        return finalNext;
    }
}