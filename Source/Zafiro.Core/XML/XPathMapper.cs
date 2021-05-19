using System;
using System.Collections.Generic;
using System.Xml.XPath;
using Import.Mapper;

namespace Zafiro.Core.XML
{
    public class XPathMapper
    {
        private readonly IDictionary<Type, IMapping> mappings;

        public XPathMapper(ConfigurationExpression configuration)
        {
            mappings = configuration.Maps;
        }

        public T Map<T>(IXPathNavigable document)
        {
            return (T)Map(typeof(T), document);
        }

        public object Map(Type type, IXPathNavigable document)
        {
            var mapping = mappings[type];
            return Map(type, document, mapping);
        }

        private object Map(Type type, IXPathNavigable document, IMapping mapping)
        {
            var instance = Activator.CreateInstance(type);
            foreach (var m in mapping.MemberConfigurations)
            {
                m.Apply(instance, document, this);
            }

            return instance;
        }
    }
}