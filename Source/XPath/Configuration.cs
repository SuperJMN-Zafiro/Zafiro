using System;
using Mapping;

namespace XPath
{
    public class Configuration : IConfiguration
    {
        private readonly ConfigurationExpression configurationExpression;

        public Configuration(Action<ConfigurationExpression> configure)
        {
            configurationExpression = new ConfigurationExpression();
            configure(configurationExpression);
        }

        public IMapper CreateMapper()
        {
            return new Mapper(configurationExpression);
        }
    }
}