using System;
using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Core;

namespace Zafiro.Wpf.Tests
{
    public class Contextualized : IContextualizable
    {
        public Contextualized(object o)
        {
        }

        public void SetContext(object o)
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
