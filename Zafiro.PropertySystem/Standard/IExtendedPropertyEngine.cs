namespace Zafiro.PropertySystem.Standard
{
    using System;

    public interface IExtendedPropertyEngine
    {
        ExtendedProperty RegisterProperty(string name, Type ownerType, Type propertyType,
            PropertyMetadata metadata);

        void SetValue(ExtendedProperty property, object instance, object value);
        object GetValue(ExtendedProperty extendedProperty, object instance);
        ExtendedProperty GetProperty(string propertyName, Type type);
        IObservable<object> GetChangedObservable(ExtendedProperty property, object instance);
        IObserver<object> GetObserver(ExtendedProperty property, object instance);
    }
}