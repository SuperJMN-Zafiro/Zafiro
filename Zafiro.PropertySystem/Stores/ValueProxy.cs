namespace Zafiro.PropertySystem.Stores
{
    using System;
    using System.Reactive.Subjects;

    internal class ValueProxy
    {
        private object value;
        private readonly Subject<ValueChange> subject = new Subject<ValueChange>();

        public ValueProxy(object @value)
        {
            Value = value;
        }

        public ValueProxy()
        {
        }

        public object Value
        {
            get { return value; }
            set
            {
                subject.OnNext(new ValueChange(this.value, value));
                this.value = value;
            }
        }

        public IObservable<ValueChange> Observable => subject;
    }
}