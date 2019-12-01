using System;
using System.Collections.Generic;
using System.Reflection;
using Zafiro.Core;
using Zafiro.PropertySystem.Stores;

namespace Zafiro.PropertySystem.Standard
{
    public class ExtendedPropertyEngine : IExtendedPropertyEngine
    {
        private readonly MetadataStore<ExtendedProperty, PropertyMetadata> metadatas = new MetadataStore<ExtendedProperty, PropertyMetadata>();
        private readonly ValueStore valueStore;
        private readonly IDictionary<(string, Type), ExtendedProperty> registeredProperties = new AutoKeyDictionary<(string, Type), ExtendedProperty>(tuple => (tuple.Item1, tuple.Item2.GetTypeInfo().BaseType), tuple => tuple.Item2 != null);


        public ExtendedPropertyEngine()
        {
            valueStore = new ValueStore(TryGetDefaultValue);
        }

        private object TryGetDefaultValue(ExtendedProperty property, object instance)
        {
            if (metadatas.Contains(instance.GetType(), property))
            {
                return metadatas.GetMetadata(instance.GetType(), property).DefaultValue;
            }

            throw new InvalidOperationException();
        }

        public ExtendedProperty RegisterProperty(string name, Type ownerType, Type propertyType, PropertyMetadata metadata)
        {
            var extendedProperty = new ExtendedProperty(propertyType, name);
            metadatas.RegisterMetadata(ownerType, extendedProperty, metadata);
            registeredProperties.Add((name, ownerType), extendedProperty);
            return extendedProperty;
        }

        public void SetValue(ExtendedProperty property, object instance, object value)
        {
            valueStore.SetValue(property, instance, value);
        }

        public object GetValue(ExtendedProperty extendedProperty, object instance)
        {
            return valueStore.GetValue(extendedProperty, instance);
        }

        public ExtendedProperty GetProperty(string propertyName, Type type)
        {
            return registeredProperties[(propertyName, type)];
        }

        public IObservable<object> GetChangedObservable(ExtendedProperty property, object instance)
        {
            return valueStore.GetObservable(property, instance);
        }

        public IObserver<object> GetObserver(ExtendedProperty property, object instance)
        {
            return valueStore.GetObserver(property, instance);
        }
    }
}