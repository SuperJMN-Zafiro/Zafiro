using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using Zafiro.Core;

namespace Zafiro.Wpf.Services
{
    public class WpfWindow : Window, IPopup
    {
        private ISubject<Unit> shown = new Subject<Unit>();
        
        public new Task Show()
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