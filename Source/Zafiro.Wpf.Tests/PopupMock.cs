using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Zafiro.Core;

namespace Zafiro.Wpf.Tests
{
    public class PopupMock : IPopup
    {
        readonly TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
        private ISubject<Unit> shown = new Subject<Unit>();

        public void Close()
        {
            tcs.SetResult(true);
        }

        public Task Show()
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