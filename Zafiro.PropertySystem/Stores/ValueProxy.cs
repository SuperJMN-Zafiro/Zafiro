namespace Zafiro.PropertySystem.Stores
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    internal class ValueProxy
    {
        private readonly ISubject<object> values = new Subject<object>();
        private object currentValue;

        public ValueProxy(object value)
        {
            Value = value;
            values.Subscribe(o => Value = o);
        }

        public ValueProxy()
        {
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

        
        public IObservable<object> Changed => values.Distinct();
        public IObserver<object> Observer => values;
    }
}