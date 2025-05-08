using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizards;

public partial class Wizard<TResult> : ReactiveObject
{
    private readonly Stack<object> previousValues = new();
    [ObservableAsProperty] private Page currentPage;
    [ObservableAsProperty] private IWizardStep currentStep;
    [Reactive] private int currentStepIndex;
    [ObservableAsProperty] private IEnhancedCommand<Result<object>> nextCommand;

    public Wizard(IList<IWizardStep> steps)
    {
        currentStepHelper = this.WhenAnyValue<Wizard<TResult>, int>(w => w.CurrentStepIndex)
            .Select(index => steps[index])
            .ToProperty(this, w => w.CurrentStep);

        currentPageHelper = this.WhenAnyValue<Wizard<TResult>, IWizardStep>(wizardo => wizardo.CurrentStep)
            .Select(step =>
            {
                var param = previousValues.TryPeek(out var p) ? p : null;
                var page = step.CreatePage(param);
                var value = new Page(page, step.GetNextCommand(page), "Title");
                return value;
            })
            .ToProperty(this, w => w.CurrentPage);

        nextCommandHelper = this.WhenAnyValue<Wizard<TResult>, IEnhancedCommand<Result<object>>>(wizard => wizard.CurrentPage.NextCommand!).ToProperty(this, x => x.NextCommand);

        ObservableExtensions.Subscribe<object>(NextCommand.Successes(), value =>
        {
            if (CurrentStepIndex == steps.Count - 1)
            {
                // finished.OnNext((TResult)value);
            }

            previousValues.Push(value);
            CurrentStepIndex++;
        });

        BackCommand = ReactiveCommand.Create<int>(() =>
        {
            previousValues.Pop();
            return CurrentStepIndex--;
        });
    }

    public ReactiveCommand<Unit, int> BackCommand { get; set; }
}