using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Serilog;

namespace Zafiro.Core.Values
{
    public class GroupSetter
    {
        private readonly PropertyInfo property;

        public GroupSetter(PropertyInfo property)
        {
            this.property = property;
        }

        public void Set(IEnumerable<object> objects, object value)
        {
            foreach (var target in objects)
            {
                try
                {
                    Set(target, value);
                }
                catch (Exception e)
                {
                    Log.Warning(e, "Could not set property {Object}.{PropName} to {Value}", target, property, value);
                }
            }
        }

        private void Set(object target, object value)
        {
            object finalValue;
            if (!property.PropertyType.IsInstanceOfType(value))
            {
                var typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                if (typeConverter.CanConvertFrom(value.GetType()))
                {
                    finalValue = typeConverter.ConvertFrom(value);
                }
                else
                {
                    return;
                }
            }
            else
            {
                finalValue = value;
            }

            property.SetValue(target, finalValue);
        }
    }
}