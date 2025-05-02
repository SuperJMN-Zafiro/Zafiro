using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using ReactiveUI.SourceGenerators;

namespace Zafiro.UI.Wizard;

public partial class Wizard<T> : ReactiveObject, IWizard
{
    private readonly BehaviorSubject<IObservable<bool>> canGoNextSubject = new(Observable.Return(true));
    private readonly Subject<T> completedSubject = new();
    private readonly List<object?> history = new();
    private readonly IReadOnlyList<WizardStep> steps;
    [Reactive] private int currentIndex;

    [Reactive] private object currentPage = null!;

    public Wizard(IEnumerable<WizardStep> steps)
    {
        this.steps = steps.ToList();
        history.Add(null);

        LoadPage(0, null);

        var hasFinished = Finished.Any().StartWith(false);
        var canGoBack = this.WhenAnyValue(vm => vm.CurrentIndex, idx => idx > 0).CombineLatest(hasFinished, (canGoBack, hasFinished) => canGoBack && !hasFinished);

        BackCommand = ReactiveCommand.Create(OnBack, canGoBack);
        NextCommand = ReactiveCommand.Create(OnNext, canGoNextSubject.Switch());
    }

    public IObservable<T> Finished => completedSubject;

    public ReactiveCommand<Unit, Unit> BackCommand { get; }
    public ReactiveCommand<Unit, Unit> NextCommand { get; }

    private void LoadPage(int index, object? prev)
    {
        CurrentPage = steps[index].PageFactory(prev);
        var canGoNextObservable = steps[index].CanGoNext(CurrentPage);
        canGoNextSubject.OnNext(canGoNextObservable);
    }

    private void OnBack()
    {
        CurrentIndex--;
        LoadPage(CurrentIndex, history[CurrentIndex]);
    }

    private void OnNext()
    {
        var step = steps[CurrentIndex];

        step.OnNext(CurrentPage)
            .Tap(nextValue =>
            {
                if (history.Count > CurrentIndex + 1)
                    history[CurrentIndex + 1] = nextValue;
                else
                    history.Add(nextValue);

                if (CurrentIndex + 1 < steps.Count)
                {
                    CurrentIndex++;
                    LoadPage(CurrentIndex, nextValue);
                }
                else
                {
                    completedSubject.OnNext((T)nextValue);
                    canGoNextSubject.OnNext(Observable.Return(false));
                    completedSubject.OnCompleted();
                }
            });
    }
}