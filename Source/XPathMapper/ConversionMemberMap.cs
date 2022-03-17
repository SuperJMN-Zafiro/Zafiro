using System;
using System.Reflection;
using CSharpFunctionalExtensions;

namespace Mapping
{
    public class ConversionMemberMap<T> : MemberMap
    {
        public ConversionMemberMap(MemberInfo member) : base(member)
        {

        }

        public ConversionMemberMap<T> MapFrom(string xpath, Func<string, T> conversion)
        {
            SourcesCore.Add(xpath);
            MapCore = conversion;
            return this;
        }

        public ConversionMemberMap<T> MapFrom(string a, string b, Func<string, string, T> conversion)
        {
            SourcesCore.UnionWith(new[] { a, b });
            MapCore = conversion;
            return this;
        }

        public ConversionMemberMap<T> MapFrom(string a, string b, string c, Func<string, string, string, T> conversion)
        {
            SourcesCore.UnionWith(new[] { a, b, c });
            MapCore = conversion;
            return this;
        }

        public ConversionMemberMap<T> MapFrom(string a, string b, string c, string d, Func<string, string, string, string, T> conversion)
        {
            SourcesCore.UnionWith(new[] { a, b, c, d });
            MapCore = conversion;
            return this;
        }

        public ConversionMemberMap<T> MapFrom(string a, string b, string c, string d, string e, Func<string, string, string, string, string, T> conversion)
        {
            SourcesCore.UnionWith(new[] { a, b, c, d, e });
            MapCore = conversion;
            return this;
        }

        public ConversionMemberMap<T> UseFallbackValue(T value)
        {
            DefaultValue = Maybe<object>.From(value);
            return this;
        }

        public ConversionMemberMap<T> UseFallbackValue(Func<T> valueFactory)
        {
            DefaultValue = Maybe<object>.From(valueFactory());
            return this;
        }
    }
}