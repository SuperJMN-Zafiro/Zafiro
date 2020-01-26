using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Grace.DependencyInjection;
using ReactiveUI;
using Serilog;

namespace SampleApp.Infrastructure.Navigation
{
    public class Navigation : INavigation
    {
        private readonly IExportLocatorScope locatorScope;
        private Frame innerFrame;

        private readonly IDictionary<Type, Type> mappings;

        private readonly ISubject<bool> canGoBack = new BehaviorSubject<bool>(false);
        private IDisposable subscription;

        public Navigation(IExportLocatorScope locatorScope, IDictionary<Type, Type> mappings)
        {
            this.locatorScope = locatorScope;
            this.mappings = mappings;
            MessageBus.Current.Listen<NavigationFrameMessage>().Subscribe(x =>
            {
                subscription?.Dispose();
                innerFrame = x.InnerFrame;

                var obs = Observable
                    .FromEventPattern<NavigatedEventHandler, NavigationEventArgs>(h => innerFrame.Navigated += h,
                        h => innerFrame.Navigated -= h)
                    .Select(_ => innerFrame.CanGoBack);

                subscription = obs.Subscribe(canGoBack);
            });
        }

        public Task Go(Type viewModelType, object parameter = null)
        {
            return GoCore(viewModelType, innerFrame, parameter);
        }

        private Task GoCore(Type viewModelType, Frame frame, object extraData)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));

            if (!mappings.TryGetValue(viewModelType, out var controlType))
            {
                Log.Warning("ViewModel of type {Type} is not mapped", viewModelType);
                return Task.CompletedTask;
            }

            var vm = locatorScope.Locate(viewModelType, extraData);
            var control = locatorScope.Locate(controlType);
            frame.Navigate(typeof(NavigationPage), ((object)control, (object)vm));
            return Task.CompletedTask;
        }

        public Task GoBack()
        {
            innerFrame.GoBack();
            return Task.CompletedTask;
        }

        public IObservable<bool> CanGoBack => canGoBack;
    }
}