﻿using System.Xml.XPath;

namespace Zafiro.Mapping
{
    public static class MapperExtensions
    {
        public static T Map<T>(this IMapper mapper, IXPathNavigable document)
        {
            return (T) mapper.Map(typeof(T), document);
        }
    }
}