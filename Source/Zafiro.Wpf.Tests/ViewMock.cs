using System;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Zafiro.UI;

namespace Zafiro.Wpf.Tests
{
    public class ViewMock : IView
    {
        readonly TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
        private ISubject<Unit> shown = new Subject<Unit>();

        public void Close()
        {
            tcs.SetResult(true);
        }

        public Task ShowAsModal()
        {
            shown.OnNext(Unit.Default);
            return tcs.Task;
        }

        public string Title { get; set; }
        public IObservable<Unit> Shown => shown;

        public void SetContext(object o)
        {
        }

        public object Object { get; }
    }
}