namespace Zafiro.PropertySystem.Standard
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Subjects;
    using System.Reflection;
    using Core;
    using Stores;

    public class ExtendedPropertyEngine : IExtendedPropertyEngine
    {
        private readonly MetadataStore<ExtendedProperty, PropertyMetadata> metadataStore =
            new MetadataStore<ExtendedProperty, PropertyMetadata>();

        private readonly ValueStore valueStore = new ValueStore();
        private readonly IDictionary<Tuple<string, Type>, ExtendedProperty> registeredProperties = new AutoKeyDictionary<Tuple<string, Type>, ExtendedProperty>(tuple => new Tuple<string, Type>(tuple.Item1, tuple.Item2.GetTypeInfo().BaseType), tuple => tuple.Item2 != null);

        public ExtendedProperty RegisterProperty(string name, Type ownerType, Type propertyType,
            PropertyMetadata metadata)
        {
            var prop = new ExtendedProperty(propertyType, name);
            metadataStore.RegisterMetadata(ownerType, prop, metadata);
            registeredProperties.Add(new Tuple<string, Type>(name, ownerType), prop);
            return prop;
        }

        public void SetValue(ExtendedProperty property, object instance, object value)
        {
            if (!metadataStore.Contains(instance.GetType(), property))
            {
                throw new InvalidOperationException();
            }

            valueStore.SetValue(property, instance, value);
        }

        public object GetValue(ExtendedProperty extendedProperty, object instance)
        {
            object retVal;
            if (valueStore.TryGetValue(extendedProperty, instance, out retVal))
            {
                return retVal;
            }

            PropertyMetadata metadata;
            if (metadataStore.TryGetMetadata(instance.GetType(), extendedProperty, out metadata))
            {
                return metadata.DefaultValue;
            }

            throw new InvalidOperationException();
        }

        public ExtendedProperty GetProperty(string propertyName, Type type)
        {
            return registeredProperties[new Tuple<string, Type>(propertyName, type)];
        }

        public IObservable<object> GetChangedObservable(ExtendedProperty property, object instance)
        {
            return valueStore.GetChangedObservable(property, instance);
        }

        public IObserver<object> GetObserver(ExtendedProperty property, object instance)
        {
            return valueStore.GetObserver(property, instance);
        }
    }
}