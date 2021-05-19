using System;
using System.Collections.Generic;

namespace Import.Mapper
{
    internal class MappingExpression : IMappingExpression
    {
        public IMappingExpression MapFrom(string xpath)
        {
            Paths = new[] { xpath };
            Convert = (Func<string, string>) (s => s);
            return this;
        }

        public IMappingExpression MapFrom<T>(string xpath, Func<string, T> convert)
        {
            Paths = new[] {xpath};
            Convert = convert;
            return this;
        }

        public IMappingExpression MapFrom<T>(string one, string two, Func<string, string, T> convert)
        {
            Paths = new[] { one, two };
            Convert = convert;
            return this;
        }

        public IEnumerable<string> Paths { get; private set; }
        public Delegate Convert { get; set; }
    }
}