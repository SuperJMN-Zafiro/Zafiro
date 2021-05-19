using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using CSharpFunctionalExtensions;
using Import.Reflection;
using Zafiro.Core.XML;

namespace Import.Mapper
{
    public class MemberConfiguration
    {
        public MemberInfo Member { get; }
        public IMappingExpression MappingExpression { get; }

        public MemberConfiguration(MemberInfo member, IMappingExpression mappingExpression)
        {
            Member = member;
            MappingExpression = mappingExpression;
        }

        public void Apply<T>(T obj, IXPathNavigable document, XPathMapper xPathMapper)
        {
            var reflectedMember = ReflectedMember.From(obj, Member);
            
            if (reflectedMember.IsCollection)
            {
                ApplyToCollection(document, xPathMapper, reflectedMember.AsCollection().Value);
            }
            else
            {
                var values = MappingExpression.Paths.Select(s => GetXpathValue(s, document));
                var processed = MappingExpression.Convert.DynamicInvoke(values.Cast<object>().ToArray());
                reflectedMember.Set(processed);
            }
        }

        private void ApplyToCollection(IXPathNavigable document, XPathMapper xPathMapper, ReflectedCollection reflectedCollection)
        {
            var childrenRoot = MappingExpression.Paths.First();
            var children = GetChildren(document, childrenRoot).ToList();

            foreach (var child in children)
            {
                var mapped = xPathMapper.Map(reflectedCollection.ItemType, child);
                reflectedCollection.Add(mapped);
            }
        }

        private static IEnumerable<XPathNavigator> GetChildren(IXPathNavigable document, string childrenRoot)
        {
            var navigator = document.CreateNavigator();
            var nodes = navigator.Select(childrenRoot);
            while (nodes.MoveNext())
            {
                yield return nodes.Current.Clone();
            }
        }

        private static string GetXpathValue(string xpath, IXPathNavigable document)
        {
            var nav = document.CreateNavigator();
            var selectSingleNode = nav.SelectSingleNode(xpath);
            return selectSingleNode.Value;
        }
    }
}