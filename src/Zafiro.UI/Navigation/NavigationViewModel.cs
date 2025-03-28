using System.Reactive;
using ReactiveUI;

namespace Zafiro.UI.Navigation;

public class NavigationViewModel : ReactiveObject
{
    public NavigationViewModel(INavigator navigator, Func<object> viewModel)
    {
        Navigator = navigator;
        Create = ReactiveCommand.CreateFromTask(() => Navigator.Go(viewModel));
        Create.Execute().Subscribe();
    }

    public INavigator Navigator { get; }
   
    public ReactiveCommand<Unit, Unit> Create { get; }
}