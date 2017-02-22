namespace Zafiro.PropertySystem.Stores
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    internal class ValueProxy
    {
        private readonly ISubject<object> values = new Subject<object>();
        private object currentValue;

        public ValueProxy()
        {
            Changed.Subscribe(o => Value = o);
            values.Subscribe(o => { });
        }

        public object Value
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                values.OnNext(value);
            }
        }

        
        public IObservable<object> Changed => values.DistinctUntilChanged();
        public IObserver<object> Observer => values;
    }
}