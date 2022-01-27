using System;
using System.ComponentModel;
using System.Xml.XPath;
using Zafiro.Mapping;

namespace Zafiro.XPath
{
    public class Mapper : IMapper
    {
        private readonly ConfigurationExpression configurationExpression;

        public Mapper(ConfigurationExpression configurationExpression)
        {
            this.configurationExpression = configurationExpression;
        }

        public object Map(Type type, IXPathNavigable document)
        {
            if (IsBuiltIn(type))
            {
                return MapBuiltIn(type, document);
            }

            return MapConfigured(type, document);
        }

        private static object Parse(Type type, string value)
        {
            return TypeDescriptor.GetConverter(type).ConvertFromString(value);
        }

        private object MapConfigured(Type type, IXPathNavigable document)
        {
            var mapping = configurationExpression.Maps[type];
            return Map(type, document, mapping);
        }

        private static object MapBuiltIn(Type type, IXPathNavigable document)
        {
            var content = document.CreateNavigator().Value;
            return Parse(type, content);
        }

        private static bool IsBuiltIn(Type type)
        {
            return type.IsPrimitive || type == typeof(string);
        }

        private object Map(Type type, IXPathNavigable document, IMap mapping)
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