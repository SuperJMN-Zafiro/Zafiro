using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using DynamicData;
using DynamicData.Binding;

namespace Zafiro.Reactive;

public static class DynamicDataMixin
{
    public static IObservable<IChangeSet<T>> ToObservableChangeSetIfPossible<T>(
        this IEnumerable<T> source
    )
        where T : notnull
    {
        if (source is INotifyCollectionChanged)
        {
            Type sourceType = source.GetType();

            var method = typeof(ObservableCollectionEx)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m =>
                    m.Name == "ToObservableChangeSet" &&
                    m.IsGenericMethod &&
                    m.GetGenericArguments().Length == 2 &&
                    m.GetParameters().Length == 1); // ¡Filtro clave!

            if (method == null)
                throw new InvalidOperationException("Method not found");

            MethodInfo genericMethod = method.MakeGenericMethod(sourceType, typeof(T));

            var observable = (IObservable<IChangeSet<T>>?)genericMethod.Invoke(null, [source]);
            if (observable is null)
            {
                throw new InvalidOperationException("ToObservableChangeSetIfPossible returned null");
            }
            
            return observable;
        }

        return source.AsObservableChangeSet();
    }
}