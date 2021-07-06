using System;
using System.Collections.Generic;

namespace Zafiro.Mapping
{
    public class ConfigurationExpression
    {
        public Dictionary<Type, IMap> Maps { get; } = new();

        public TypeMap<T> CreateMap<T>() where T : new()
        {
            var mapping = new TypeMap<T>();
            Maps.Add(typeof(T), mapping);
            return mapping;
        }
    }
}