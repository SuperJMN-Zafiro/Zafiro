using System;
using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.UI;

namespace Zafiro.Wpf.Tests
{
    public class TestHaveDataContext : IHaveDataContext
    {
        public void SetDataContext(object dataContext)
        {
        }

        public object Object { get; }
    }


    public class SampleViewModel : ReactiveObject
    {
        public SampleViewModel()
        {
            IsEven = this.WhenAnyValue(m => m.Number).Select(i => i % 2 == 0);
        }

        public IObservable<bool> IsEven { get; }

        private int number;

        public int Number
        {
            get => number;
            set => this.RaiseAndSetIfChanged(ref number, value);
        }

        public void Executed()
        {
            IsExecuted = true;
        }

        public bool IsExecuted { get; set; }
    }
}
