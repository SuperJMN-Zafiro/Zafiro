using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Reactive;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public partial class SlimWizard<TResult> : ReactiveObject, ISlimWizard<TResult>
{
    private readonly ReplaySubject<TResult> finishedSubject = new();
    private readonly Stack<object> previousValues = new();
    [ObservableAsProperty] private IPage currentPage;
    [ObservableAsProperty] private (int, IWizardStep) currentStep;
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

        var hasFinished = finishedSubject.Any().Not().StartWith(true);

        currentTypedPageHelper = this.WhenAnyValue<SlimWizard<TResult>, (int, IWizardStep)>(w => w.CurrentStep)
            .Select(step =>
            {
                var param = previousValues.TryPeek(out var p) ? p : null;
                var page = step.Item2.CreatePage(param);
                var finalNext = CreateNextCommand(step, page, hasFinished);
                var value = new Page(step.Item1, page, finalNext, step.Item2.NextText);
                return value;
            })
            .ToProperty(this, w => w.CurrentTypedPage);

        typedNextHelper = this.WhenAnyValue(wizard => wizard.CurrentTypedPage.NextCommand!)
            .Select(command => command)
            .ToProperty(this, x => x.TypedNext);

        this.WhenAnyValue(wizard => wizard.TypedNext)
            .Switch()
            .Successes()
            .Subscribe(value =>
            {
                if (CurrentStepIndex == steps.Count - 1)
                {
                    finishedSubject.OnNext((TResult)value);
                }
                else
                {
                    previousValues.Push(value);
                    CurrentStepIndex++;
                }
            });

        var canGoBack = this.WhenAnyValue(wizard => wizard.CurrentStepIndex, i => i > 0).CombineLatest(hasFinished, (validIndex, finished) => validIndex && !finished);
        Back = ReactiveCommand.Create(() =>
            {
                previousValues.Pop();
                CurrentStepIndex--;
            }, canGoBack)
            .Enhance();

        this.WhenAnyValue(wizard => wizard.Next).Subscribe(command => { });

        nextHelper = this.WhenAnyValue(x => x.TypedNext, command => new CommandAdapter<Result<object>, Unit>(command, _ => Unit.Default))
            .ToProperty(this, x => x.Next);

        currentPageHelper = this.WhenAnyValue<SlimWizard<TResult>, IPage, Page>(x => x.CurrentTypedPage, page => page).ToProperty(this, wizard => wizard.CurrentPage);
    }

    public IObservable<TResult> Finished => finishedSubject.AsObservable();
    public IEnhancedCommand Back { get; }
    public int TotalPages { get; }

    private static EnhancedCommand<Result<object>> CreateNextCommand((int, IWizardStep) step, object page, IObservable<bool> canGoNext)
    {
        var command = step.Item2.GetNextCommand(page);
        var canExecute = canGoNext.CombineLatest(((IReactiveCommand)command).CanExecute, (a, b) => a && b);
        var finalCommand = command.Enhance(canExecute: canExecute);
        var finalNext = finalCommand;
        return finalNext;
    }
}