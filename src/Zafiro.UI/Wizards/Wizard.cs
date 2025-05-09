using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Reactive;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public partial class Wizard<TResult> : ReactiveObject, INewWizard<TResult>
{
    private readonly Subject<TResult> finished = new();
    private readonly Stack<object> previousValues = new();
    [ObservableAsProperty] private IPage currentPage;
    [ObservableAsProperty] private (int, IWizardStep) currentStep;
    [Reactive] private int currentStepIndex;
    [ObservableAsProperty] private Page currentTypedPage;
    [ObservableAsProperty] private IEnhancedCommand next;
    [ObservableAsProperty] private IEnhancedCommand<Result<object>> typedNext;

    public Wizard(IList<IWizardStep> steps)
    {
        TotalPages = steps.Count;

        currentStepHelper = this.WhenAnyValue(w => w.CurrentStepIndex)
            .Select(index => (index, steps[index]))
            .ToProperty(this, w => w.CurrentStep);

        var canGoNext = finished.Any().Not().StartWith(true);

        currentTypedPageHelper = this.WhenAnyValue<Wizard<TResult>, (int, IWizardStep)>(w => w.CurrentStep)
            .Select(step =>
            {
                var param = previousValues.TryPeek(out var p) ? p : null;
                var page = step.Item2.CreatePage(param);
                var finalNext = ReactiveCommand.CreateFromObservable(() => step.Item2.GetNextCommand(page)!.Execute(), canGoNext).Enhance();
                var value = new Page(step.Item1, page, finalNext, "Title");
                return value;
            })
            .ToProperty(this, w => w.CurrentTypedPage);

        typedNextHelper = this.WhenAnyValue(wizard => wizard.CurrentTypedPage.NextCommand!)
            .ToProperty(this, x => x.TypedNext);

        this.WhenAnyValue(wizard => wizard.TypedNext)
            .Switch()
            .Successes()
            .Subscribe(value =>
            {
                if (CurrentStepIndex == steps.Count - 1)
                {
                    finished.OnNext((TResult)value);
                }
                else
                {
                    previousValues.Push(value);
                    CurrentStepIndex++;
                }
            });

        Back = ReactiveCommand.Create(() =>
            {
                previousValues.Pop();
                CurrentStepIndex--;
            }, this.WhenAnyValue(wizard => wizard.CurrentStepIndex, i => i > 0))
            .Enhance();

        nextHelper = this.WhenAnyValue(x => x.TypedNext, command => new CommandAdapter<Result<object>, Unit>(command, _ => Unit.Default))
            .ToProperty(this, x => x.Next);

        currentPageHelper = this.WhenAnyValue<Wizard<TResult>, IPage, Page>(x => x.CurrentTypedPage, page => page).ToProperty(this, wizard => wizard.CurrentTypedPage);
    }

    public IObservable<TResult> Finished => finished.AsObservable();
    public IEnhancedCommand Back { get; }
    public int TotalPages { get; }
}