using System;
using System.Reactive;
using Microsoft.Reactive.Testing;

namespace Zafiro.Testing;

public class Notify
{
    public static Recorded<Notification<T>> OnNext<T>(T value, long time)
    {
        return new Recorded<Notification<T>>(time, Notification.CreateOnNext(value));
    }

    public static Recorded<Notification<T>> OnCompleted<T>(long time)
    {
        return new Recorded<Notification<T>>(time, Notification.CreateOnCompleted<T>());
    }

    public static Recorded<Notification<T>> OnError<T>(Exception exception, long time)
    {
        return new Recorded<Notification<T>>(time, Notification.CreateOnError<T>(exception));
    }
}