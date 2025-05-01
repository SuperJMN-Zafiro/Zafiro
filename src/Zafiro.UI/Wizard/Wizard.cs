using System.Reactive.Subjects;
using CSharpFunctionalExtensions;

namespace Zafiro.UI.Wizard;

public class Wizard : ReactiveObject
{
    private readonly IReadOnlyList<WizardStep> steps;
    private readonly List<object?> history = new();
    private int currentIndex;

    private object currentPage = default!;
    public  object  CurrentPage
    {
        get => currentPage;
        private set => this.RaiseAndSetIfChanged(ref currentPage, value);
    }

    public ReactiveCommand<Unit, Unit> BackCommand { get; }
    public ReactiveCommand<Unit, Unit> NextCommand { get; }
    private readonly Subject<object> completedSubject = new();
    public  IObservable<object>      WizardCompleted  => completedSubject;

    public Wizard(IEnumerable<WizardStep> steps)
    {
        this.steps = steps.ToList();
        history.Add(null);

        LoadPage(0, null);

        var canGoBack = this.WhenAnyValue(vm => vm.currentIndex, idx => idx > 0);

        BackCommand = ReactiveCommand.Create(OnBack, canGoBack);
        NextCommand = ReactiveCommand.Create(OnNext);
    }

    private void LoadPage(int index, object? prev)
    {
        CurrentPage = steps[index].PageFactory(prev);
    }

    private void OnBack()
    {
        currentIndex--;
        LoadPage(currentIndex, history[currentIndex]);
    }

    private void OnNext()
    {
        var step   = steps[currentIndex];

        // 1) Validamos el paso actual
        step.OnNext(CurrentPage)
            .Tap(nextValue =>
            {
                if (history.Count > currentIndex + 1)
                    history[currentIndex + 1] = nextValue;
                else
                    history.Add(nextValue);

                if (currentIndex + 1 < steps.Count)
                {
                    currentIndex++;
                    LoadPage(currentIndex, nextValue);
                }
                else
                {
                    completedSubject.OnNext(nextValue);
                    completedSubject.OnCompleted();
                }
            });
    }
}