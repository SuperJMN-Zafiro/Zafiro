using System;
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
            var mapping = configurationExpression.Maps[type];
            return Map(type, document, mapping);
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