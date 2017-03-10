namespace Zafiro.Core
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public class TreeExtensions
    {
        public static IObservable<T> ParentAttached<T>(object targetInstance,
            IObservable<Association> childAttached) where T : class
        {
            ISubject<T> subject = new Subject<T>();
            IDisposable subs = null;

            subs = childAttached
                .Where(relationship => relationship.Child == targetInstance)
                .Do(relationship => targetInstance = relationship.Parent)
                .Select(relationship => relationship.Parent as T)
                .Where(b => b != null)
                .Subscribe(o =>
                {
                    subject.OnNext(o);
                    subs.Dispose();
                });

            return subject;
        }
    }
}