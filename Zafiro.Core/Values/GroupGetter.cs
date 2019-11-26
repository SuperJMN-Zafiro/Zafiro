using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog;

namespace Zafiro.Core.Values
{
    public class GroupGetter
    {
        private readonly PropertyInfo property;

        public GroupGetter(PropertyInfo property)
        {
            this.property = property;
        }

        public object GetValue(IEnumerable<object> targets)
        {
            var query = from target in targets
                from prop in target.GetType().GetRuntimeProperties().Where(x => string.Equals(x.Name, property.Name))
                select new { target, prop };

            var values = query.Select(x =>
            {
                try
                {
                    var value = x.prop.GetValue(x.target);
                    return value;
                }
                catch (Exception e)
                {
                    Log.Warning(e, "Could not get values of property {Property}", x);
                    return null;
                }
            }).ToList();

            if (values.Distinct().Count() == 1)
            {
                return values.First();
            }

            return property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;
        }
    }
}