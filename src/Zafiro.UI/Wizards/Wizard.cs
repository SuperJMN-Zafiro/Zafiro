using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Reactive;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public partial class Wizard<TResult> : ReactiveObject
{
    private readonly Subject<TResult> finished = new();
    private readonly Stack<object> previousValues = new();
    [ObservableAsProperty] private Page currentPage;
    [ObservableAsProperty] private IWizardStep currentStep;
    [Reactive] private int currentStepIndex;
    [ObservableAsProperty] private IEnhancedCommand<Result<object>> nextCommand;

    public Wizard(IList<IWizardStep> steps)
    {
        currentStepHelper = this.WhenAnyValue(w => w.CurrentStepIndex)
            .Select(index => steps[index])
            .ToProperty(this, w => w.CurrentStep);

        var canGoNext = finished.Any().Not().StartWith(true);

        currentPageHelper = this.WhenAnyValue<Wizard<TResult>, IWizardStep>(w => w.CurrentStep)
            .Select(step =>
            {
                var param = previousValues.TryPeek(out var p) ? p : null;
                var page = step.CreatePage(param);
                var finalNext = ReactiveCommand.CreateFromObservable(() => step.GetNextCommand(page)!.Execute(), canGoNext).Enhance();
                var value = new Page(page, finalNext, "Title");
                return value;
            })
            .ToProperty(this, w => w.CurrentPage);

        nextCommandHelper = this.WhenAnyValue(wizard => wizard.CurrentPage.NextCommand!).ToProperty(this, x => x.NextCommand);

        NextCommand.Subscribe(result => { });

        this.WhenAnyValue(wizard => wizard.NextCommand)
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

        BackCommand = ReactiveCommand.Create(() =>
        {
            previousValues.Pop();
            return CurrentStepIndex--;
        }, this.WhenAnyValue(wizard => wizard.CurrentStepIndex, i => i > 0));
    }

    public ReactiveCommand<Unit, int> BackCommand { get; set; }
}