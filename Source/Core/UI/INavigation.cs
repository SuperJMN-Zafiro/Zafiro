using System;
using System.Threading.Tasks;

namespace Core.UI
{
    public interface INavigation
    {
        Task Go(Type viewModelType, object parameter = null);
        Task GoBack();
        IObservable<bool> CanGoBack { get; }
    }
}