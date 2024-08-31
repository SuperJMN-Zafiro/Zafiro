using System;
using System.Threading.Tasks;

namespace Zafiro.UI;

public interface INavigation
{
    IObservable<bool> CanGoBack { get; }
    Task Go(Type viewModelType, object parameter = null);
    Task GoBack();
}