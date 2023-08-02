using System;
using System.Collections.Generic;
using System.Reactive;
using Microsoft.Reactive.Testing;

namespace Zafiro.Zafiro.Testing;

public class NotificationBuilder<T>
{
    private readonly List<Recorded<Notification<T>>> notifications = new();

    public NotificationBuilder<T> OnNext(T value, long time)
    {
        notifications.Add(new Recorded<Notification<T>>(time, Notification.CreateOnNext(value)));
        return this;
    }

    public NotificationBuilder<T> OnCompleted(long time)
    {
        notifications.Add(new Recorded<Notification<T>>(time, Notification.CreateOnCompleted<T>()));
        return this;
    }

    public NotificationBuilder<T> OnError(Exception exception, long time)
    {
        notifications.Add(new Recorded<Notification<T>>(time, Notification.CreateOnError<T>(exception)));
        return this;
    }

    public IEnumerable<Recorded<Notification<T>>> Build()
    {
        return notifications.AsReadOnly();
    }
}