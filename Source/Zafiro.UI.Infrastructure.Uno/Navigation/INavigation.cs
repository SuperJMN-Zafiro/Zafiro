using System;
using System.Threading.Tasks;

namespace Zafiro.UI.Infrastructure.Uno.Navigation
{
    public interface INavigation
    {
        Task Go(Type viewModelType, object parameter = null);
        Task GoBack();
        IObservable<bool> CanGoBack { get; }
    }
}