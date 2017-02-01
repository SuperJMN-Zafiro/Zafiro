namespace Zafiro.PropertySystem.Stores
{
    using System;
    using System.Collections.Generic;
    using Standard;
    using Core;

    public class ValueStore
    {
        private readonly IDictionary<PropertyEntry, ValueProxy> values = new Dictionary<PropertyEntry, ValueProxy>();

        public void SetValue(ExtendedProperty property, object instance, object value)
        {
            var proxy = values.GetCreate(new PropertyEntry(property, instance), () => new ValueProxy());
            proxy.Value = value;
        }

        public bool TryGetValue(ExtendedProperty key, object instance, out object value)
        {
            var found = values.TryGetValue(new PropertyEntry(key, instance), out var proxy);
            if (found)
            {
                value = proxy.Value;
                return true;
            }

            value = null;
            return false;
        }

        public object GetValue(ExtendedProperty property, object instance)
        {
            return values[new PropertyEntry(property, instance)].Value;
        }

        public IObservable<ValueChange> GetObservable(ExtendedProperty property, object instance)
        {
            var valueProxy = values.GetCreate(new PropertyEntry(property, instance), () => new ValueProxy());
            return valueProxy.Observable;
        }
    }   
}