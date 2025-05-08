using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;
using Zafiro.UI.Commands;

namespace Zafiro.UI.Wizard;

public partial class Wizard<T> : ReactiveObject, IWizard<T>
{
    private readonly ReplaySubject<T> completedSubject = new();
    private readonly List<object?> history = new();
    private readonly IReadOnlyList<WizardStep> steps;
    [Reactive] private object currentPage = null!;
    [Reactive] private int currentPageIndex;
    [ObservableAsProperty] private WizardStep currentStep;
    [ObservableAsProperty] private MaybeViewModel<string> currentTitle = new(Maybe<string>.None);
    [ObservableAsProperty] private IEnhancedCommand<Result<object>> nextCommand;
    [ObservableAsProperty] private string? nextText;


    public Wizard(IEnumerable<WizardStep> steps)
    {
        this.steps = steps.ToList();

        var hasFinished = Finished.Any().StartWith(false);
        var canGoBack = this.WhenAnyValue(vm => vm.CurrentPageIndex, idx => idx > 0).CombineLatest(hasFinished, (canGoBack, hasFinished) => canGoBack && !hasFinished);

        currentTitleHelper = this.WhenAnyValue(x => x.CurrentPage)
            .WhereNotNull()
            .Select(o => o switch
            {
                ITitled title => new MaybeViewModel<string>(title.Title.AsMaybe()),
                _ => new MaybeViewModel<string>(Maybe<string>.None)
            })
            .ToProperty(this, x => x.CurrentTitle);

        nextTextHelper = this.WhenAnyValue(x => x.CurrentStep)
            .WhereNotNull()
            .Select(x => x.NextText)
            .ToProperty(this, x => x.NextText);

        currentStepHelper = this.WhenAnyValue(x => x.CurrentPageIndex)
            .WhereNotNull()
            .Select(i => this.steps[i])
            .ToProperty(this, x => x.CurrentStep);

        history.Add(null);

        BackCommand = EnhancedCommand.Create(ReactiveCommand.Create(OnBack, canGoBack));
        var whenAnyObservable =
            this.WhenAnyValue(
                w => w.CurrentStep.NextCommand,
                w => w.CurrentPageIndex,
                (func, idx) => func?.Invoke(history[idx])
            );

        nextCommandHelper = whenAnyObservable.ToProperty(this, wizard => wizard.NextCommand);

        this.WhenAnyValue(wizard => wizard.NextCommand).Subscribe(command => { });

        LoadPage(0, null);
    }

    T IWizard<T>.CurrentPage => (T)CurrentPage;

    public IObservable<T> Finished => completedSubject;

    public int TotalPages => steps.Count;
    public IEnhancedUnitCommand BackCommand { get; }

    IObservable<object?> IWizard.Finished => Finished.Select(arg => (object?)arg);

    private void OnBack()
    {
        CurrentPageIndex--;
        LoadPage(CurrentPageIndex, history[CurrentPageIndex]);
    }

    // private async Task OnNext()
    // {
    //     var step = steps[CurrentPageIndex];
    //
    //     step.NextCommand.Successes()
    //         .Do(nextValue =>
    //         {
    //             if (history.Count > CurrentPageIndex + 1)
    //             {
    //                 history[CurrentPageIndex + 1] = nextValue;
    //             }
    //             else
    //             {
    //                 history.Add(nextValue);
    //             }
    //
    //             if (CurrentPageIndex + 1 < steps.Count)
    //             {
    //                 CurrentPageIndex++;
    //                 LoadPage(CurrentPageIndex, nextValue);
    //             }
    //             else
    //             {
    //                 completedSubject.OnNext((T)nextValue);
    //                 completedSubject.OnCompleted();
    //             }
    //         })
    //         .Subscribe();
    // }

    private void LoadPage(int index, object? prev)
    {
        CurrentPage = steps[index].PageFactory(prev);
    }
}