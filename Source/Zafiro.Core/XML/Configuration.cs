using System;
using Zafiro.Core.XML;

namespace Import.Mapper
{
    public class Configuration
    {
        private readonly ConfigurationExpression configuration;

        public Configuration(Action<ConfigurationExpression> configure)
        {
            configuration = new ConfigurationExpression();
            configure(configuration);
        }

        public XPathMapper CreateMapper()
        {
            return new(configuration);
        }
    }
}