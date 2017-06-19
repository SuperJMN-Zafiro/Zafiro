using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Zafiro.Core;
using Zafiro.PropertySystem.Standard;

namespace Zafiro.PropertySystem.Stores
{
    internal class ValueStore
    {
        private readonly Func<ExtendedProperty, object, object> getDefaultValue;
        private readonly IDictionary<PropertyEntry, BehaviorSubject<object>> values = new Dictionary<PropertyEntry, BehaviorSubject<object>>();

        public ValueStore(Func<ExtendedProperty, object, object> getDefaultValue)
        {
            this.getDefaultValue = getDefaultValue;
        }
    
        public object GetValue(ExtendedProperty property, object instance)
        {
            var propertyEntry = GetEntry(property, instance);
            var subject = values.GetCreate(propertyEntry, () =>
            {
                var defaultValue = getDefaultValue(property, instance);
                return new BehaviorSubject<object>(defaultValue);
            });
            return subject.Value;
        }

        private static PropertyEntry GetEntry(ExtendedProperty extendedProperty, object instance)
        {
            return new PropertyEntry(extendedProperty, instance);
        }

        public void SetValue(ExtendedProperty property, object instance, object value)
        {
            var subject = GetSubject(property, instance);
            subject.OnNext(value);
        }

        private BehaviorSubject<object> GetSubject(ExtendedProperty property, object instance)
        {
            var propertyEntry = GetEntry(property, instance);
            var subject = values.GetCreate(propertyEntry,
                () => new BehaviorSubject<object>(getDefaultValue(property, instance)));
            return subject;
        }

        public IObservable<object> GetObservable(ExtendedProperty property, object instance)
        {
            return GetSubject(property, instance).DistinctUntilChanged().AsObservable();
        }

        public IObserver<object> GetObserver(ExtendedProperty property, object instance)
        {
            return GetSubject(property, instance).AsObserver();
        }
    }
}