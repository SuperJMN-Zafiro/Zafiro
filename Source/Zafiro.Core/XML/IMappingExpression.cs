using System;
using System.Collections;
using System.Collections.Generic;

namespace Import.Mapper
{
    public interface IMappingExpression
    {
        IMappingExpression MapFrom(string xpath);
        IMappingExpression MapFrom<T>(string xpath, Func<string, T> convert);
        IMappingExpression MapFrom<T>(string one, string two, Func<string, string, T> convert);
        IEnumerable<string> Paths { get; }
        Delegate Convert { get; }
    }
}