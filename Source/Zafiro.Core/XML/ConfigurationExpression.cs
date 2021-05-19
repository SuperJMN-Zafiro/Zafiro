using System;
using System.Collections.Generic;

namespace Import.Mapper
{
    public class ConfigurationExpression
    {
        public IDictionary<Type, IMapping> Maps { get; } = new Dictionary<Type, IMapping>();

        public Mapping<T> CreateMap<T>() where T : new()
        {
            var mapping = new Mapping<T>();
            Maps.Add(typeof(T), mapping);
            return mapping;
        }
    }
}