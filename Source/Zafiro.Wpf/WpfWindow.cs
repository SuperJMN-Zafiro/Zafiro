using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;

namespace Zafiro.UI.Wpf
{
    public class WpfWindow : Window, IView
    {
        private ISubject<Unit> shown = new Subject<Unit>();
        
        public Task ShowAsModal()
        {
            shown.OnNext(Unit.Default);
            return this.ShowDialogAsync();
        }

        public IObservable<Unit> Shown => shown;

        public void SetContext(object o)
        {
            this.DataContext = o;
        }

        public object Object => this.Content;
    }
}