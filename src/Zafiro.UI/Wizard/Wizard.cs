using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;

namespace Zafiro.UI.Wizard;

public partial class Wizard<T> : ReactiveObject, IWizard<T>
{
    private readonly BehaviorSubject<IObservable<bool>> canGoNextSubject = new(Observable.Return(true));
    private readonly ReplaySubject<T> completedSubject = new();
    private readonly List<object?> history = new();
    private readonly IReadOnlyList<WizardStep> steps;
    [Reactive] private object currentPage = null!;
    [Reactive] private int currentPageIndex;
    [ObservableAsProperty] private MaybeViewModel<string> currentTitle = new(Maybe<string>.None);
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

        nextTextHelper = this.WhenAnyValue(x => x.CurrentPageIndex)
            .WhereNotNull()
            .Select(i => this.steps[i].NextText)
            .ToProperty(this, x => x.NextText);

        BackCommand = ReactiveCommand.Create(OnBack, canGoBack);
        NextCommand = ReactiveCommand.CreateFromTask(OnNext, canGoNextSubject.Switch());

        history.Add(null);
        LoadPage(0, null);
    }

    public T CurrentPageOfT => (T)CurrentPage;

    T IWizard<T>.CurrentPage => (T)CurrentPage;

    public IObservable<T> Finished => completedSubject;

    public int TotalPages => steps.Count;
    public ReactiveCommand<Unit, Unit> BackCommand { get; }
    public ReactiveCommand<Unit, Unit> NextCommand { get; }

    IObservable<object?> IWizard.Finished => Finished.Select(arg => (object?)arg);

    private void OnBack()
    {
        CurrentPageIndex--;
        LoadPage(CurrentPageIndex, history[CurrentPageIndex]);
    }

    private async Task OnNext()
    {
        var step = steps[CurrentPageIndex];

        await step.OnNext(CurrentPage)
            .Tap(nextValue =>
            {
                if (history.Count > CurrentPageIndex + 1)
                {
                    history[CurrentPageIndex + 1] = nextValue;
                }
                else
                {
                    history.Add(nextValue);
                }

                if (CurrentPageIndex + 1 < steps.Count)
                {
                    CurrentPageIndex++;
                    LoadPage(CurrentPageIndex, nextValue);
                }
                else
                {
                    completedSubject.OnNext((T)nextValue);
                    canGoNextSubject.OnNext(Observable.Return(false));
                    completedSubject.OnCompleted();
                }
            });
    }

    private void LoadPage(int index, object? prev)
    {
        CurrentPage = steps[index].PageFactory(prev);
        var canGoNextObservable = steps[index].CanGoNext(CurrentPage);
        canGoNextSubject.OnNext(canGoNextObservable);
    }
}