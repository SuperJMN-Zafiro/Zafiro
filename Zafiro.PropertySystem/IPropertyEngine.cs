namespace Zafiro.PropertySystem
{
    using System;
    using Attached;
    using Standard;

    public interface IPropertyEngine
    {
        void SetValue(ExtendedProperty property, object instance, object value);
        void SetValue(AttachedProperty property, object instance, object value);
        object GetValue(ExtendedProperty property, object instance);
        object GetValue(AttachedProperty property, object instance);
        AttachedProperty RegisterProperty(string name, Type owner, Type propertyType, AttachedPropertyMetadata metadata);
        ExtendedProperty RegisterProperty(string name, Type owner, Type propertyType, PropertyMetadata metadata);
        ExtendedProperty GetProperty(string propertyName, Type type);
        IObservable<object> GetChangedObservable(ExtendedProperty property, object instance);
        IObserver<object> GetObserver(ExtendedProperty property, object instance);
    }
}